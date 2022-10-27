using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KK
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] UpdateMode updateMode;
        [SerializeField] protected float updateTime;
        protected float deltaTime;
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            Custom
        }

        List<AnyTransition> anyTransitions = new List<AnyTransition>();

        State currentState;

        public State CurrentState => currentState;

        Coroutine stateUpdateCoroutine;

        YieldInstruction update;
        YieldInstruction initial;

        protected virtual void Awake()
        {
            initial = SetUpdateMode(updateMode, updateTime);
        }

        protected virtual void OnEnable()
        {
            if (currentState != null)
                SetState(currentState);
        }

        public void SetState(State newState)
        {
            currentState = newState;

            stateUpdateCoroutine = StartCoroutine(Co_UpdateStateMachine());
        }

        public void ExitState(bool immediate = false)
        {
            if (!immediate)
                currentState.OnExit();

            if (stateUpdateCoroutine != null)
                StopCoroutine(stateUpdateCoroutine);

            currentState = null;
        }

        IEnumerator Co_UpdateStateMachine()
        {
            currentState.OnEnter();

            yield return update;

            Transition transition;

            while (true)
            {
                if (currentState == null)
                    throw new ArgumentNullException("Current state is null but trying to update");
                
                transition = GetTransition();

                if (transition != null)
                    break;

                deltaTime = update == null ? Time.deltaTime : deltaTime;

                currentState.OnUpdate();

                yield return update;
            }

            transition.InvokeCallback();

            currentState.OnExit();

            SetState(transition.to);
        }

        Transition GetTransition()
        {
            foreach (var tranistion in currentState.transitions)
            {
                if (tranistion.ConditionTriggered())
                    return tranistion;
            }

            foreach(var transition in anyTransitions)
            {
                if (transition.ConditionTriggered(currentState))
                    return transition;
            }

            return null;
        }

        public AnyTransition AddAnyTransition(State to, params Func<bool>[] conditions)
        {
            var transition = new AnyTransition(to, conditions);
            anyTransitions.Add(transition);
            return transition;
        }

        public YieldInstruction SetUpdateMode(UpdateMode newMode, float newTime = 0)
        {
            switch (newMode)
            {
                case UpdateMode.Update:
                    update = null;
                    break;
                case UpdateMode.FixedUpdate:
                    update = new WaitForFixedUpdate();
                    deltaTime = Time.fixedDeltaTime;
                    break;
                case UpdateMode.Custom:
                    if (newTime == 0)
                        return update = initial;
                    update = new WaitForSeconds(newTime);
                    deltaTime = newTime;
                    break;
                default:
                    update = initial;
                    break;
            }
            return update;
        }
#region Editor
#if UNITY_EDITOR
        [CustomEditor(typeof(StateMachine), true), CanEditMultipleObjects]
        public class StateMachineEditor : Editor
        {
            SerializedProperty updateTime, updateMode, script;
            void OnEnable()
            {
                updateTime = serializedObject.FindProperty("updateTime");
                updateMode = serializedObject.FindProperty("updateMode");
                script = serializedObject.FindProperty("m_Script");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField(script);

                GUILayout.Label("StateMachine", EditorStyles.boldLabel);

                updateMode.enumValueIndex = (int)(UpdateMode)EditorGUILayout.EnumPopup(updateMode.displayName, (UpdateMode)updateMode.enumValueIndex);

                if ((UpdateMode)updateMode.enumValueIndex == UpdateMode.Custom)
                {
                    updateTime.floatValue = EditorGUILayout.FloatField(updateTime.displayName, updateTime.floatValue);
                }

                EditorGUILayout.Space();

                DrawPropertiesExcluding(serializedObject, script.name, updateMode.name, updateTime.name);

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
#endregion
    }
}