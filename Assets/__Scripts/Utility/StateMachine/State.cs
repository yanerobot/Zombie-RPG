using System;
using System.Collections.Generic;

namespace KK
{
    public class State
    {
        public enum On
        {
            Enter,
            Update,
            Exit
        }
        public string name;
        public Action OnEnterAction, OnUpdateAction, OnExitAction;

        public List<Transition> transitions;
        public State(string name = null, Action OnEnterAction = null, Action OnUpdateAction = null, Action OnExitAction = null) 
        {
            transitions = new List<Transition>();

            this.OnEnterAction = OnEnterAction;
            this.OnUpdateAction = OnUpdateAction;
            this.OnExitAction = OnExitAction;
            this.name = (name == null) ?  GetType().ToString() : name;
        }
        public virtual void OnEnter()
        {
            OnEnterAction?.Invoke();
        }
        public virtual void OnUpdate()
        {
            OnUpdateAction?.Invoke();
        }
        public virtual void OnExit()
        {
            OnExitAction?.Invoke();
        }

        public State AddActions(On actionType, params Action[] actions)
        {
            switch (actionType)
            {
                case On.Enter:
                    foreach (var action in actions)
                        OnEnterAction += action;
                    break;
                case On.Update:
                    foreach (var action in actions)
                        OnUpdateAction += action;
                    break;
                case On.Exit:
                    foreach (var action in actions)
                        OnExitAction += action;
                    break;
            }

            return this;
        }

        public Transition AddTransition(State to, params Func<bool>[] conditions)
        {
            var transition = new Transition(to, conditions);
            transitions.Add(transition);
            return transition;
        }
    }
}
