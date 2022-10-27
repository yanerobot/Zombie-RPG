using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace KK
{
    public class Stamina : StateMachine
    {
        [Header("Stamina")]
        public float maxStamina;
        [SerializeField] float fillSpeed;
        [SerializeField] float reduceSpeed;
        [SerializeField] float waitBeforeFillDelay;
        public float currentStamina { get; private set; }

        public UnityEvent OnStaminaEmpty, OnStaminaChange, OnStaminaFull;
        public UnityAction<float> OnStaminaInstantChange;

        public bool isReducing, isInstantReduce;

        public UnityAction OnEnterWaitingState;

        protected override void OnEnable()
        {
            base.OnEnable();
            Init();
        }

        void Init()
        {
            currentStamina = maxStamina;
            SetupStates();
        }

        void SetupStates()
        {
            var tc = new TimeCondition(waitBeforeFillDelay);

            var filling = new State("Filling", OnUpdateAction: () => FillStamina(fillSpeed * deltaTime));
            var reducing = new State("Reduce", OnUpdateAction: () => ReduceStamina(reduceSpeed * deltaTime));
            var waiting = new State("Waiting", () => { tc.ResetTimer(); OnEnterWaitingState?.Invoke(); });

            filling.AddTransition(reducing, () => isReducing);
            reducing.AddTransition(waiting, () => !isReducing);

            waiting.AddTransition(filling, tc.HasTimePassed);
            waiting.AddTransition(reducing, () => isReducing);

            AddAnyTransition(waiting, () => isInstantReduce).AddTransitionCallBack(() => isInstantReduce = false);

            SetState(filling);
        }

        public void InstantReduceStamina(float amount)
        {
            isInstantReduce = true;
            
            ReduceStamina(amount);
            OnStaminaInstantChange?.Invoke(currentStamina - amount);
        }

        public void SetReducing(bool value)
        {
            isReducing = value;
        }

        void ReduceStamina(float amount)
        {
            SetStamina(currentStamina - amount);
        }

        void FillStamina(float amount)
        {
            SetStamina(currentStamina + amount);
        }

        void SetStamina(float value)
        {
            CheckValue(ref value);

            currentStamina = value;
            OnStaminaChange?.Invoke();
        }

        float previousValue;
        void CheckValue(ref float value)
        {
            if (value <= 0)
            {
                value = 0;
                if (previousValue > 0)
                    OnStaminaEmpty?.Invoke();
            }
            else if (value >= maxStamina)
            {
                value = maxStamina;
                if (previousValue < maxStamina)
                    OnStaminaFull?.Invoke();
            }
            previousValue = value;
        }
    }
}