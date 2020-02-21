using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


public class Enemy : MonoBehaviour
{
    [Header("Patrol state parameters")] [SerializeField]
    private float patrolRadius;

    public float patrolingSpeed = 4f;


    [Header("Following state parameters")] [SerializeField]
    public float distanceToFollow = 5f;

    public float runningSpeed = 5f;
    public float howCloseShouldPlayerBe = 0.3f;


    [Header("Required components")] [SerializeField]
    public Rigidbody2D rb;

    public Animator animator;
    public Collider2D boxCollider;
    public PlayerMovementFinal playerToFollow;

    [Header("Debug")] public bool movingRight;
    public Vector3 startPosition;

    public StateMachine<Enemy> stateMachine;


    // TODO: understand where to hide this states
    public State<Enemy> patroling;
    public State<Enemy> runningTowardsPlayer;
    public State<Enemy> goingToStartPosition;
    public State<Enemy> hitPlayer;


    private void Start()
    {
        startPosition = transform.position;

        patroling = new PatrollingState(patrolRadius, patrolingSpeed);
        runningTowardsPlayer = new RunningTowardsPlayer(distanceToFollow);
        goingToStartPosition = new GoingToStartPosition(runningSpeed);
        hitPlayer = new HitPlayer();

        stateMachine = new StateMachine<Enemy>(patroling, this);
    }

    public void Flip()
    {
        movingRight = !movingRight;
        transform.rotation = Quaternion.Euler(0f, movingRight ? 0f : 180f, 0f);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        stateMachine.OnCollisionEnter(other, this);
    }

    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {    
        print(collision);
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.black);
        }
    }

    public void OnAnimationEnd(string clipName)
    {
        stateMachine.OnAnimationEnd(clipName, this);
    }
}