using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA;


public class PlayerMovementFinal : MonoBehaviour
{
    [Header("Required")] [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;

    [Header("Horizontal movement")] [SerializeField]
    private float horizontalSpeed = 10f;

    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float linearDrag = 4f;

    [Header("Jump")] [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float fallMultiplier = 1.25f;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private float gravity = 4f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask oneWayLayer;

    [SerializeField] private bool onGround = false;
    [Range(0, 10f)] public float distanceBetweenRays = 0f;
    public float raysCenterShift = 0f;

    private Vector3 originalScale;
    private bool movingRight = true; // by default player is looking right
    private Vector2 direction;
    private LayerMask lastLayer;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Jumped = Animator.StringToHash("jumped");
    
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private bool isOnGround()
    {
        var shiftFromCenter = movingRight ? raysCenterShift : -raysCenterShift;
        var transformPosition = transform.position;
        return Physics2D.Raycast(transformPosition + Vector3.right * (distanceBetweenRays - shiftFromCenter),
                   Vector2.down, groundLength, groundLayer) ||
               Physics2D.Raycast(transformPosition - Vector3.right * (distanceBetweenRays + shiftFromCenter),
                   Vector2.down, groundLength, groundLayer);
    }

    private void Update()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool wasOnGround = onGround;
        onGround = isOnGround();

        // // TODO simplify this shit, boi  
        // if (!onGround)
        // {
        //     var raycastHit2D = Physics2D.Raycast(transform.position + Vector3.right * distanceBetweenRays, Vector2.down,
        //         groundLength, oneWayLayer);
        //     if (raycastHit2D.collider != null && raycastHit2D.distance > groundLength - 0.01f)
        //     {
        //         Debug.Log(raycastHit2D.distance);
        //         onGround = true;
        //     }
        //
        //     raycastHit2D = Physics2D.Raycast(transform.position - Vector3.right * distanceBetweenRays, Vector2.down, groundLength,
        //         oneWayLayer);
        //     if (raycastHit2D.collider != null && raycastHit2D.distance > groundLength - 0.01f)
        //     {
        //         Debug.Log(raycastHit2D.distance);
        //         onGround = true;
        //     }
        // }

        if (!wasOnGround && onGround)
        {    
            anim.SetBool(Jumped, false);
            StartCoroutine(SqueezeJump(1.25f, 0.9f, 0.1f));
        }

        if (wasOnGround && !onGround)
        {
            anim.SetBool(Jumped, true);
        }
        
        if (Input.GetButton("Jump") && onGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        
        anim.SetBool(Jumped, true);
        StartCoroutine(SqueezeJump(0.5f, 1.2f, 0.1f));
    }

    private void FixedUpdate()
    {
        MovePlayer(direction.x);
        UpdatePhysics();
    }

    private void MovePlayer(float x)
    {
        var deltaX = x * horizontalSpeed; /* Time.deltaTime */

        rb.AddForce(Vector2.right * deltaX);


        if ((deltaX < 0 && movingRight) ||
            (deltaX > 0 && !movingRight))
        {
            Flip();
        }

        var xVelocity = rb.velocity.x;
        if (Mathf.Abs(xVelocity) > maxSpeed)
        {
            rb.velocity = new Vector2(Math.Sign(xVelocity) * maxSpeed, rb.velocity.y);
        }

        anim.SetFloat(Speed, Math.Abs(deltaX));
    }

    private static bool DifferentSigns(float first, float second) =>
        (first < 0 && second > 0) || (first > 0 && second < 0);

    private void UpdatePhysics()
    {
        var changingDirection = DifferentSigns(direction.x, rb.velocity.x);

        if (onGround)
        {
            if (Math.Abs(direction.x) < 0.4f || changingDirection) // TODO: when turning around
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }

            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.25f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.rotation = Quaternion.Euler(0f, movingRight ? 0 : 180, 0f);
    }

    private IEnumerator SqueezeJump(float x, float y, float seconds)
    {
        Vector3 original = originalScale;
        Vector3 newSize = new Vector3(original.x * x, original.y * y, original.z);

        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(original, newSize, t);
            yield return null;
        }


        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(newSize, original, t);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        var shiftFromCenter = movingRight ? raysCenterShift : -raysCenterShift;

        Gizmos.color = Color.red;
        var transformPosition = transform.position;
        Gizmos.DrawLine(transformPosition + Vector3.right * (distanceBetweenRays - shiftFromCenter),
            transformPosition + Vector3.down * groundLength + Vector3.right * (distanceBetweenRays - shiftFromCenter));
        Gizmos.DrawLine(transformPosition - Vector3.right * (distanceBetweenRays + shiftFromCenter),
            transformPosition + Vector3.down * groundLength - Vector3.right * (distanceBetweenRays + shiftFromCenter));
    }
}