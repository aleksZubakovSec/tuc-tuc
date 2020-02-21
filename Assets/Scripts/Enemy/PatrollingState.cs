using System;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;
using UnityEngine;

public class PatrollingState : State<Enemy>
{
    private readonly float patrolRadius;
    // private readonly float speed;
    private readonly float distanceToWall;


    private StateMachine<Enemy> stateMachine;
    private int groundLayerMask = LayerMask.NameToLayer("Ground");

    private float DistanceFromStartPosition(Enemy owner) => Vector2.Distance(Vector2.right * owner.transform.position.x,
        Vector2.right * owner.startPosition.x);

    public PatrollingState(float patrolRadius, float speed)
    {
        // this.stateMachine = stateMachine;
        this.patrolRadius = patrolRadius;
        // this.speed = speed;
    }

    public override void Enter(Enemy owner)
    {
        Debug.Log("Enter Patrolling state");


        // owner.boxCollider.enabled = false;
        // TODO: mb move into State Machine to do this automatically
        // ???
        owner.animator.SetTrigger("Walk");


        var distanceFromStartPosition = DistanceFromStartPosition(owner);
        if (distanceFromStartPosition >= patrolRadius)
        {
            owner.stateMachine.SetState(owner.goingToStartPosition, owner);
        }
    }

    public override void FixedUpdate(Enemy owner)
    {
        var distance = DistanceFromStartPosition(owner);
        if (distance >= patrolRadius)
        {
            owner.Flip();
        }

        owner.rb.velocity = new Vector2(owner.movingRight ? owner.patrolingSpeed : -owner.patrolingSpeed, owner.rb.velocity.y);
    }

    public override void Exit(Enemy owner)
    {
        owner.animator.ResetTrigger("Walk");
        Debug.Log("Exit Patrolling state");
        
        // throw new System.NotImplementedException();
    }

    public override void OnCollisionEnter(Collider2D other, Enemy owner)
    {
        var otherGameObject = other.gameObject;
        if ((otherGameObject.layer & groundLayerMask) != 0)
        {
            
            // TODO: Avoid walls
            // var distanceToWall = Mathf.Abs(otherGameObject.transform.position.x -
            //                                owner.transform.position.x);
            // if (distanceToWall <= this.distanceToWall)
            // {
            //     owner.Flip();
            //     return;
            // }
            return;
        }
        
        // var playerMovementScript = otherGameObject.GetComponent<PlayerMovementFinal>();
        // print("Collided with " + other);
        // print(otherGameObject);
        //
        // // TODO: fix this via changing rigidbody2d, collider, physics settings
        // // TODO: not with fucking 'if' here you bastard 
        // if (playerMovementScript != null)
        // {
        //     owner.stateMachine.ChangeState(owner.runningTowardsPlayer, owner);
        // }
 
        if (other.gameObject.tag.Equals("Player") && !owner.playerToFollow.dead)
        {
            owner.stateMachine.ChangeState(owner.runningTowardsPlayer, owner);
        }
    }

    public override void OnAnimationEnd(string clipName, Enemy owner)
    {
        throw new NotImplementedException();
    }
}