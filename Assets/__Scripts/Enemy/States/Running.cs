using UnityEngine;

namespace KK
{
    public class Running : State
    {
        EnemyAI AI;
        public Running(StateMachine fsm)
        {
            AI = fsm as EnemyAI;
        }

        public override void OnEnter()
        {
            AI.agent.isStopped = false;
        }

        public override void OnUpdate()
        {
            AI.Move();
        }

        public override void OnExit()
        {
            AI.agent.isStopped = true;
        }
    }
}
