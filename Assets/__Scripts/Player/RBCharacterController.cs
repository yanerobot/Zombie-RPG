using UnityEngine;
using UnityEngine.InputSystem;
using KK.Utility;
using DG.Tweening;

namespace KK
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RBCharacterController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] float runSpeed;
        [SerializeField] float rotationSpeed = 0.3f;
        [SerializeField] Animator animator;

        Rigidbody rb;
        CapsuleCollider coll;
        InputMaster input;

        static string currentDeviceName;

        void Awake()
        {
            coll = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            input = new InputMaster();
        }

        void Start()
        {
            input.Enable();
            input.Movement.Run.performed += ctx => Move(ctx.ReadValue<Vector2>());
            input.Movement.Run.canceled += ctx => Move(Vector2.zero);

            InputSystem.onDeviceChange += (device, change) => currentDeviceName = device.name; 
        }

        void Move(Vector2 input)
        {
            var movementDirection = ConvertInputToIsometric(input);

            if (input != Vector2.zero)
                transform.DORotateQuaternion(Quaternion.LookRotation(movementDirection, Vector3.up), rotationSpeed);

            rb.velocity = movementDirection * runSpeed;
            

            animator.SetFloat("MoveSpeed", Vector3.Distance(Vector3.zero, rb.velocity));
        }

        Vector3 ConvertInputToIsometric(Vector2 input)
        {
            var cameraYRotation = Camera.main.transform.localEulerAngles.y;
            var rotatedMatrix = Matrix4x4.Rotate(Quaternion.Euler(Vector3.zero.Where(y: cameraYRotation)));
            return (rotatedMatrix * new Vector3(input.x, 0, input.y)).normalized;
        }

        Vector3 GetDirectionFromLookInput(Vector2 input)
        {
            if (currentDeviceName.Contains("Mouse"))
            {
                input -= new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            }
            return ConvertInputToIsometric(input);
        }

    }
}
