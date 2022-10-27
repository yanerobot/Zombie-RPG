using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using KK.Items;
using KK.Utility;
using System.Collections;
using System;

namespace KK
{
    public class MeleeWeaponController : MonoBehaviour
    {
        [SerializeField] int maxComboLength;
        [SerializeField] public Animator animator;
        [SerializeField] Stamina stamina;
        [SerializeField] CCIsoMovement3D playerMovement;
        [SerializeField] Transform sheathSwordHolder;

        public LayerMask opposingLayers;

        [Header("Damages")]
        [SerializeField] int fastNormalDamage;
        [SerializeField] int fastFinisherDamage;
        [SerializeField] int holdNormalDamage;
        [SerializeField] int heavyNormalDamage;
        [SerializeField] int heavyFinisherDamage;

        //Events
        [SerializeField] UnityEvent onStartUsing, onStopUsing, onSwing, OnFinishWithdraw, OnStartSheath;

        bool staminaFrozen;
        bool maxComboReached;
        bool isUsing;
        bool isWithdrawn;
        int comboLength;

        public UnityAction OnAttackMiss;

        public bool missedAttack;

        bool isPointerOverGameobject;

        Melee currentMeleeWeapon;

        public const string FAST_ATTACK = "Attacked";
        public const string HEAVY_ATTACK = "Attacked2";
        public const string HOLD_ATTACK = "AttackedHold";
        public const string HEAVY_HOLD_ATTACK = "AttackedHold2";
        public const string HOLDING_PARAMETER_NAME = "isHoldingMeleeOneHanded";
        public const string TOGGLE_WITHDRAW = "toggleWithdraw";
        public const int ANIMATOR_LAYER_INDEX = 3;
        public enum AttackType
        {
            Attack,
            Attack2,
            AttackHold
        }

        InputMaster input;

        void Awake()
        {
            input = new InputMaster();

            input.Items.Toss.performed += _ => SheathSword();

            input.Items.Use.performed += _ => PeformAttack(FAST_ATTACK);
            input.Items.Use2.performed += _ => PeformAttack(HEAVY_ATTACK);
            input.Items.UseHold.performed += _ => PeformAttack(HOLD_ATTACK);
        }

        void Update()
        {
            isPointerOverGameobject = EventSystem.current.IsPointerOverGameObject();
        }
/*
        void LateUpdate()
        {
            isPreparingAttack = animator.GetCurrentAnimatorStateInfo(ANIMATOR_LAYER_INDEX).tagHash == Animator.StringToHash("Start");
            if (isPreparingAttack)
                playerMovement.LookAtMouse();
        }*/

        void OnDestroy()
        {
            input.Dispose();
        }

        void OnDisable()
        {
            input.Disable();
        }

        void OnEnable()
        {
            if (currentMeleeWeapon != null)
                input.Enable();
        }

        Coroutine swing;
        void PeformAttack(string attackType)
        {
            if (isPointerOverGameobject)
                return;

            if (currentMeleeWeapon == null ||
                staminaFrozen ||
                maxComboReached)
                return;

            if (comboLength >= maxComboLength)
                maxComboReached = true;

            if (!isWithdrawn)
            {
                WithdrawSword();
                return;
            }

            animator.SetTrigger(attackType);

            if (swing != null)
                StopCoroutine(swing);
            swing = StartCoroutine(Co_OnSwingCallback(1f, attackType, () => playerMovement.Dash()));
        }

        IEnumerator Co_OnSwingCallback(float maxTime, string attackType, Action Callback)
        {
            var currentTime = 0f;
            while (currentTime < maxTime)
            {
                if (currentMeleeWeapon == null)
                    break;

                if (animator.GetBool("IsSwing"))
                {
                    //SWING
                    onSwing?.Invoke();
                    int damage = GetDamage(attackType, animator.GetBool("IsFinisher"));
                    currentMeleeWeapon.OnStartSwing(damage);
                    comboLength++;
                    Callback();
                    break;
                }

                currentTime += Time.deltaTime;

                yield return null;
            }

            yield return new WaitWhile(() => animator.GetBool("IsSwing"));
            currentMeleeWeapon?.OnEndSwing();
        }

        int GetDamage(string attackType, bool isFinisher)
        {
            switch (attackType)
            {
                case FAST_ATTACK:
                    if (isFinisher)
                        return fastFinisherDamage;
                    return fastNormalDamage;
                case HEAVY_ATTACK:
                    if (isFinisher)
                        return heavyFinisherDamage;
                    return heavyNormalDamage;
                case HOLD_ATTACK:
                    return holdNormalDamage;
            }
            return 0;
        }

        void DisableWeapon()
        {
            isWithdrawn = false;
            input.Items.Use2.Disable();
            input.Items.UseHold.Disable();
        }

        void EnableAttacks()
        {
            input.Items.Use2.Enable();
            input.Items.UseHold.Enable();
        }

        void SheathSword()
        {
            OnStartSheath?.Invoke();
            DisableWeapon();
            animator.SetBool(TOGGLE_WITHDRAW, false);
            if (isUsing)
                onStopUsing?.Invoke();
        }
        void WithdrawSword()
        {
            EnableAttacks();
            animator.SetBool(TOGGLE_WITHDRAW, true);
        }

        void AELOnSheathSword()
        {
            animator.SetBool(HOLDING_PARAMETER_NAME, false);
            currentMeleeWeapon.transform.SetParent(sheathSwordHolder);
            currentMeleeWeapon.transform.rotation = sheathSwordHolder.rotation;
        }

        void AELOnPickSword()
        {
            currentMeleeWeapon.SetRightHandTransformSettings();
        }

        void AELOnFinishWithdraw()
        {
            animator.SetBool(HOLDING_PARAMETER_NAME, true);
            isWithdrawn = true;
            OnFinishWithdraw?.Invoke();
        }


        public void OnStartCombo()
        {
            isUsing = true;
            onStartUsing?.Invoke();
        }

        public void OnEndCombo()
        {
            isUsing = false;
            onStopUsing?.Invoke();
            comboLength = 0;
            maxComboReached = false;
        }
        public void FreezeStaminaDependentActions()
        {
            staminaFrozen = true;
            this.Co_DelayedExecute(() => staminaFrozen = false, () => stamina.currentStamina == stamina.maxStamina);
        }

        public void SetController(Melee currentMeleeWeapon, Transform itemHolder)
        {
            this.currentMeleeWeapon = currentMeleeWeapon;
            animator.SetLayerWeight(ANIMATOR_LAYER_INDEX, 1);
            input.Enable();
            DisableWeapon();
            animator.SetBool(HOLDING_PARAMETER_NAME, false);
        }

        public void UnsetController()
        {
            input.Disable();
            animator.SetLayerWeight(ANIMATOR_LAYER_INDEX, 0);
            if (isUsing)
                onStopUsing?.Invoke();
            animator.SetBool(HOLDING_PARAMETER_NAME, false);
            currentMeleeWeapon = null;
        }
    }
}
