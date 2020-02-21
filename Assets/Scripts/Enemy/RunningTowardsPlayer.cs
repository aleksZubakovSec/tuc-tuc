using UnityEngine;

public class RunningTowardsPlayer : State<Enemy>
{
    private float distanceToFollow;

    public RunningTowardsPlayer(float distanceToFollow)
    {
        this.distanceToFollow = distanceToFollow;
    }

    public override void Enter(Enemy owner)
    {
        Debug.Log("Enter running towards player state");
        
        // owner.boxCollider.enabled = true;
        owner.animator.SetTrigger("Run");
    }

    public override void FixedUpdate(Enemy owner)
    {
        var playerPosition = owner.playerToFollow.transform.position;
        var enemyPosition = owner.transform.position;

        // TODO: stop chasing if player is too high ( ;) )
        Vector2 dir = (Vector2) playerPosition - (Vector2) enemyPosition;
        float distance = dir.magnitude;
        if (distance <= owner.howCloseShouldPlayerBe)
        {
            CaughtPlayer(owner);
            return;
        }

        // print("Running fixed update");

        if (dir.magnitude > owner.distanceToFollow)
        {
            owner.stateMachine.ChangeState(owner.patroling, owner);
            return;
        }

        dir.Normalize();
        float angle = Mathf.Abs(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        if (angle > 90f && owner.movingRight || angle < 90f && !owner.movingRight)
        {
            print("Flipped");
            print("angle: " + angle + " movingRgiht: " + owner.movingRight);
            owner.Flip();
        }


        var direction = Vector2.MoveTowards(enemyPosition, playerPosition,
            owner.runningSpeed * Time.deltaTime);

        owner.rb.MovePosition(new Vector2(direction.x, enemyPosition.y));
    }

    private void CaughtPlayer(Enemy owner)
    {
        owner.stateMachine.SetState(owner.hitPlayer, owner);
        owner.rb.velocity = new Vector2(0, owner.rb.velocity.y);

        Debug.Log("Ment caught koresh!");
    }

    public override void Exit(Enemy owner)
    {
        owner.animator.ResetTrigger("Run");
        Debug.Log("Exit running towards player state");
        // throw new System.NotImplementedException();
    }

    public override void OnCollisionEnter(Collider2D other, Enemy owner)
    {
        // throw new System.NotImplementedException();
    }

    public override void OnAnimationEnd(string clipName, Enemy owner)
    {
        throw new System.NotImplementedException();
    }
}