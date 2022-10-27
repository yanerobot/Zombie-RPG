using System;

namespace KK
{
    public class Transition
    {
        public State to;

        Func<bool>[] conditions;
        Action OnTransitionCallback;
        public Transition(State to, params Func<bool>[] conditions)
        {
            this.to = to;
            this.conditions = conditions;
        }

        public bool ConditionTriggered()
        {
            foreach (var cond in conditions)
                if (!cond())
                    return false;
            
            return true;
        }

        public Transition AddTransitionCallBack(params Action[] callbacks)
        {
            foreach (var callback in callbacks)
                OnTransitionCallback += callback;
            return this;
        }

        public void InvokeCallback()
        {
            OnTransitionCallback?.Invoke();
        }
    }

    public class AnyTransition : Transition
    {
        State[] exceptionStates = new State[0];

        public AnyTransition(State to, params Func<bool>[] conditions) : base(to, conditions) { }

        public bool ConditionTriggered(State currentState)
        {
            foreach (var exception in exceptionStates)
                if (currentState == exception)
                    return false;
            
            return ConditionTriggered();
        }

        public AnyTransition Except(params State[] exceptionStates)
        {
            this.exceptionStates = exceptionStates;
            return this;
        }
    }
}
