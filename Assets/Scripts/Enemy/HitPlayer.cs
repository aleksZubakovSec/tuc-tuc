using UnityEngine;

public class HitPlayer : State<Enemy>
{
    public override void Enter(Enemy owner)
    {
        Debug.Log("Entered Hit Player state");
        if (owner.playerToFollow.dead)
        {
            owner.stateMachine.ChangeState(owner.goingToStartPosition, owner);
            return;
        }

        owner.animator.SetTrigger("Hit");
    }

    public override void FixedUpdate(Enemy owner)
    {
        // throw new System.NotImplementedException();
    }

    public override void Exit(Enemy owner)
    {    
        // owner.animator.ResetTrigger("Hit");
        Debug.Log("Exit Hit Player state");
    }

    public override void OnCollisionEnter(Collider2D other, Enemy owner)
    {
        // throw new System.NotImplementedException();
    }

    public override void OnAnimationEnd(string clipName, Enemy owner)
    {
        if (clipName.Equals("Hit"))
        {
            if (owner.playerToFollow.dead)
            {
                owner.stateMachine.ChangeState(owner.goingToStartPosition, owner);
                return;
            }
            
            var playerPosition = owner.playerToFollow.transform.position;
            var enemyPosition = owner.transform.position;

            Vector2 dir = (Vector2) playerPosition - (Vector2) enemyPosition;
            float distance = dir.magnitude;
            if (distance <= owner.howCloseShouldPlayerBe)
            {
                return;
            }

            owner.stateMachine.ChangeState(owner.runningTowardsPlayer, owner);
        }
    }
}