using UnityEngine;

namespace KK
{
    public class Dead : State
    {
        EnemyAI AI;

        public Dead(StateMachine fsm)
        {
            AI = fsm as EnemyAI;
        }

        public override void OnEnter()
        {
            
        }
    }
}
