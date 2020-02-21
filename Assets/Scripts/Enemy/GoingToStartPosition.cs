using UnityEngine;

public class GoingToStartPosition : State<Enemy>
{
    private float speed;

    private float DistanceFromStartPosition(Enemy owner) => Vector2.Distance(Vector2.right * owner.transform.position.x,
        Vector2.right * owner.startPosition.x);

    public GoingToStartPosition(float speed)
    {
        this.speed = speed;
    }

    public override void Enter(Enemy owner)
    {
        Debug.Log("Enter Going to start position");
        owner.animator.SetTrigger("Walk");
        
        // owner.boxCollider.enabled = false;
        
        
        var startPosition = owner.startPosition;
        var currentPosition = owner.transform.position;

        Vector2 dir = (Vector2) startPosition - (Vector2) currentPosition;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle > 90f && owner.movingRight || angle < 90f && !owner.movingRight)
        {
            owner.Flip();
        }
    }

    public override void FixedUpdate(Enemy owner)
    {
        var distance = DistanceFromStartPosition(owner);
        if (distance <= 0.5f)
        {
            owner.stateMachine.ChangeState(owner.patroling, owner);
            return;
        }

        owner.rb.velocity = new Vector2(owner.movingRight ? speed : -speed, owner.rb.velocity.y);
    }

    public override void Exit(Enemy owner)
    {    
        owner.animator.ResetTrigger("Walk");
        Debug.Log("Exit Going to start position");
    }

    public override void OnCollisionEnter(Collider2D other, Enemy owner)
    {
        if (owner.playerToFollow.dead)
        {
            return;
        }
        owner.stateMachine.ChangeState(owner.runningTowardsPlayer, owner);
    }

    public override void OnAnimationEnd(string clipName, Enemy owner)
    {
        // throw new System.NotImplementedException();
    }
}