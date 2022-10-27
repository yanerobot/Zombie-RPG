using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using KK.Utility;
using KK.Player;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KK
{
    [RequireComponent(typeof(CharacterController))]
    public class CCIsoMovement3D : StateMachine
    {
        [Header("Movement Stats")]
        [SerializeField] float runSpeed;
        [SerializeField] float sprintSpeed;
        [SerializeField] float gravityScale;
        [SerializeField] bool turnSmoothly;
        [SerializeField] float turnSpeed;
        [SerializeField] UnityEvent onMove;
        float currentSpeed;
        float vertical;
        Vector3 movementDir;
        Vector3 rotationDir;
        bool freezeRun;
        bool staminaDependentActionsFrozen;

        [Header("Animation")]
        [SerializeField, Min(1)] float animationInterpolationSpeed;

        [Header("Roll")]
        [SerializeField] float rollSpeed;
        [SerializeField] float rollTime;
        float curRollSpeed;

        [Header("Dash")]
        [SerializeField] float dashSpeed;
        [SerializeField] float dashTime;
        float curDashSpeed;

        [Header("Stamina")]
        [SerializeField] Stamina stamina;
        [SerializeField] float rollReduceAmount;
        //[SerializeField] float timeToFreezeStaminaActions;

        CharacterController characterController;
        InputMaster input;
        static string currentDeviceName = "";
        Animator animator;

        bool DashInput;
        bool isDead;
        bool gotHit;


        [SerializeField] PlayerInput playerInput;

        Vector2 RunInput => input.Movement.Run.ReadValue<Vector2>();
        Vector2 LookInput => input.Movement.Look.ReadValue<Vector2>();
        bool IsSprinting => input.Movement.Sprint.inProgress && RunInput != Vector2.zero;
        bool RollInput => input.Movement.Roll.inProgress;

        protected override void Awake()
        {
            base.Awake();
            input = new InputMaster();
            characterController = GetComponent<CharacterController>();

            animator = GetComponent<Animator>();

            currentDeviceName = playerInput.currentControlScheme;
            playerInput.onControlsChanged += OnContolsChanged;
        }

        void Start()
        {
            SetupStates();

            curDashSpeed = dashSpeed;
            curRollSpeed = rollSpeed;
        }

        void OnContolsChanged(PlayerInput input)
        {
            print(input.currentControlScheme);
            currentDeviceName = input.currentControlScheme;
        }

        void OnDestroy()
        {
            input.Dispose();
        }

        void OnDisable()
        {
            input.Movement.Disable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            input.Movement.Enable();
        }

        void SetupStates()
        {
            var running = new State("Running").AddActions(State.On.Enter, () => currentSpeed = runSpeed, () => onMove?.Invoke()).AddActions(State.On.Update, Running, Move);
            var sprinting = new State("Sprinting", Sprint, () => { Running(); Move(); }, OnEndSprint);
            var dashing = new State("Dashing", SetupDash, Move, ResetDash);
            var rolling = new State("Rolling", SetupRoll, Move, ResetRoll);
            var dead = new State("Dead");
            var getHit = new State("Get Hit", () => currentSpeed = 0, null, ()=> currentSpeed = runSpeed);

            var rollTimeCondition = new TimeCondition(rollTime);

            running.AddTransition(sprinting, () => IsSprinting, () => !staminaDependentActionsFrozen);
            sprinting.AddTransition(running, () => !IsSprinting);
            sprinting.AddTransition(running, () => staminaDependentActionsFrozen);
            rolling.AddTransition(running, rollTimeCondition.HasTimePassed);
            rolling.AddTransition(sprinting, rollTimeCondition.HasTimePassed, () => IsSprinting, () => !staminaDependentActionsFrozen);
            dashing.AddTransition(running, new TimeCondition(dashTime).HasTimePassed);
            getHit.AddTransition(running, () => !animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"), new TimeCondition(0.01f).HasTimePassed);

            AddAnyTransition(dashing, () => DashInput).Except(dashing).AddTransitionCallBack(() => DashInput = false);
            AddAnyTransition(rolling, () => RollInput, () => !staminaDependentActionsFrozen).Except(rolling);
            AddAnyTransition(dead, () => isDead).Except(dead).AddTransitionCallBack(() => input.Dispose());
            AddAnyTransition(getHit, () => gotHit).AddTransitionCallBack(() => gotHit = false);

            SetState(running);
        }

        void SetAnimatorFloat()
        {
            var interpolated = Mathf.Lerp(animator.GetFloat("MoveSpeed"), (movementDir * currentSpeed).magnitude, animationInterpolationSpeed * Time.deltaTime);

            if (Mathf.Abs((movementDir * currentSpeed).magnitude - interpolated) < 0.001f)
                interpolated = (movementDir * currentSpeed).magnitude;
           
            animator.SetFloat("MoveSpeed", interpolated);
        }

        void Sprint()
        {
            currentSpeed = sprintSpeed;
            stamina.SetReducing(true);
        }
        
        void OnEndSprint()
        {
            currentSpeed = runSpeed;
            stamina.SetReducing(false);
        }

        void Running()
        {
            movementDir = ConvertInputToIsoDirection(freezeRun ? Vector2.zero : RunInput);

            SetAnimatorFloat();

            if (movementDir != Vector3.zero)
                rotationDir = movementDir;

            transform.RotateTowards(rotationDir, turnSmoothly, turnSpeed * Time.deltaTime);
        }

        public void LookAtMouse()
        {
            transform.RotateTowards(GetLookDirection(LookInput), turnSmoothly, turnSpeed * Time.deltaTime);
        }

        void Move()
        {
            ComputeGravity();

            characterController.Move(movementDir.AddTo(y: vertical) * currentSpeed * Time.deltaTime);
        }

        void SetupDash()
        {
            currentSpeed = curDashSpeed;
            movementDir = RunInput == Vector2.zero ? (LookInput == Vector2.zero ? transform.forward : GetLookDirection(LookInput)) : ConvertInputToIsoDirection(RunInput);
            rotationDir = movementDir;
            transform.RotateTowards(rotationDir);
        }

        void ResetDash()
        {
            curDashSpeed = dashSpeed;
        }

        void SetupRoll()
        {
            gameObject.layer = LayerMask.NameToLayer("NoEnemyCollision");
            currentSpeed = curRollSpeed;
            movementDir = RunInput == Vector2.zero ? transform.forward : ConvertInputToIsoDirection(RunInput);
            rotationDir = movementDir;
            transform.RotateTowards(rotationDir);
            animator.SetTrigger("Rolled");
            stamina.InstantReduceStamina(rollReduceAmount);
        }

        void ResetRoll()
        {
            curRollSpeed = rollSpeed;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

 
        public void Dash()
        {
            DashInput = true;
        }

        Vector3 GetLookDirection(Vector2 input)
        {
            if (currentDeviceName.Contains("Mouse"))
            {
                input -= new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            }
            return ConvertInputToIsoDirection(input).normalized;
        }

        Vector3 ConvertInputToIsoDirection(Vector2 input)
        {
            var cameraYRotation = Camera.main.transform.localEulerAngles.y;
            var rotatedMatrix = Matrix4x4.Rotate(Quaternion.Euler(Vector3.zero.Where(y: cameraYRotation)));
            return (rotatedMatrix * new Vector3(input.x, 0, input.y));
        }

        void ComputeGravity()
        {
            if (characterController.isGrounded && vertical < 0)
                vertical = -15f;

            vertical += Physics.gravity.y * Time.deltaTime * gravityScale;
        }

        public void FreezeStaminaDependentActions()
        {
            staminaDependentActionsFrozen = true;
            this.Co_DelayedExecute(() => staminaDependentActionsFrozen = false, () => stamina.currentStamina == stamina.maxStamina);
        }

        public void FreezeRun()
        {
            freezeRun = true;
        }

        public void UnFreezeRun()
        {
            freezeRun = false;
        }

        #region Unity events for health component
        public void OnDie()
        {
            isDead = true;
        }

        public void OnGetHit()
        {
            gotHit = true;
        }
        #endregion
/*
        #region
#if UNITY_EDITOR
        [CustomEditor(typeof(CCIsoMovement3D))]
        public class CCIsoMovement3DEditor : Editor
        {
            SerializedProperty turnSmoothly, turnSpeed;
            void OnEnable()
            {
                turnSmoothly = serializedObject.FindProperty("turnSmoothly");
                turnSpeed = serializedObject.FindProperty("turnSpeed");

            }
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DrawPropertiesExcluding(serializedObject, turnSpeed.name, turnSmoothly.name);


                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
        #endregion*/
    }
}