using UnityEngine;
namespace KK
{
    public class Searching : State
    {
        EnemyAI AI;
        public Searching(StateMachine fsm)
        {
            AI = fsm as EnemyAI;
        }

        public override void OnEnter()
        {
            AI.SetUpdateMode(StateMachine.UpdateMode.Custom, newTime: AI.searchingUpdateTime);
        }

        public override void OnExit()
        {
            AI.SetUpdateMode(default);
        }
    }
}
