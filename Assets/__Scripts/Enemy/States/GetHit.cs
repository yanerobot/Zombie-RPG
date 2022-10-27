namespace KK
{
    public class GetHit : State
    {
        EnemyAI AI;
        
        public GetHit(StateMachine fsm)
        {
            AI = fsm as EnemyAI;
        }

        public override void OnEnter()
        {
            AI.PlayHitAnimation();
        }
    }
}
