using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA;


public class PlayerMovementFinal : MonoBehaviour
{
    [Header("Required")] [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rigidbody;

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

    [Range(0, 10f)] public float rayOffset = 0f;

    private Vector3 originalScale;
    private bool _movingRight = true; // by default player is looking right
    private Vector2 _direction;
    private LayerMask lastLayer;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + Vector3.right * rayOffset, Vector2.down, groundLength,
                       groundLayer) ||
                   Physics2D.Raycast(transform.position - Vector3.right * rayOffset, Vector2.down, groundLength,
                       groundLayer);
        // TODO simplify this shit, boi  
        if (!onGround)
        {
            var raycastHit2D = Physics2D.Raycast(transform.position + Vector3.right * rayOffset, Vector2.down,
                groundLength, oneWayLayer);
            if (raycastHit2D.collider != null && raycastHit2D.distance > groundLength - 0.01f)
            {
                Debug.Log(raycastHit2D.distance);
                onGround = true;
            }

            raycastHit2D = Physics2D.Raycast(transform.position - Vector3.right * rayOffset, Vector2.down, groundLength,
                oneWayLayer);
            if (raycastHit2D.collider != null && raycastHit2D.distance > groundLength - 0.01f)
            {
                Debug.Log(raycastHit2D.distance);
                onGround = true;
            }
        }

        if (!wasOnGround && onGround)
        {
            StartCoroutine(SqueezeJump(1.25f, 0.9f, 0.1f));
        }

        if (Input.GetButton("Jump") && onGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        StartCoroutine(SqueezeJump(0.5f, 1.2f, 0.1f));
    }

    private void FixedUpdate()
    {
        MovePlayer(_direction.x);
        UpdatePhysics();
    }

    private void MovePlayer(float x)
    {
        var deltaX = x * horizontalSpeed; /** Time.deltaTime*/

        rigidbody.AddForce(Vector2.right * deltaX);


        if ((deltaX < 0 && _movingRight) ||
            (deltaX > 0 && !_movingRight))
        {
            Flip();
        }

        var xVelocity = rigidbody.velocity.x;
        if (Mathf.Abs(xVelocity) > maxSpeed)
        {
            rigidbody.velocity = new Vector2(Math.Sign(xVelocity) * maxSpeed, rigidbody.velocity.y);
        }

        anim.SetFloat("speed", Math.Abs(deltaX));
    }

    private static bool DifferentSigns(float first, float second) =>
        (first < 0 && second > 0) || (first > 0 && second < 0);

    private void UpdatePhysics()
    {
        var changingDirection = DifferentSigns(_direction.x, rigidbody.velocity.x);

        if (onGround)
        {
            if (Math.Abs(_direction.x) < 0.4f || changingDirection) // TODO: when turning around
            {
                rigidbody.drag = linearDrag;
            }
            else
            {
                rigidbody.drag = 0f;
            }

            rigidbody.gravityScale = 0f;
        }
        else
        {
            rigidbody.gravityScale = gravity;
            rigidbody.drag = linearDrag * 0.25f;
            if (rigidbody.velocity.y < 0)
            {
                print("Whoops");
                rigidbody.gravityScale = gravity * fallMultiplier;
            }
            else if (rigidbody.velocity.y > 0 && Input.GetButton("Jump"))
            {
                rigidbody.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    private void Flip()
    {
        _movingRight = !_movingRight;
        transform.rotation = Quaternion.Euler(0f, _movingRight ? 0 : 180, 0f);
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
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.right * rayOffset,
            transform.position + Vector3.down * groundLength + Vector3.right * rayOffset);
        Gizmos.DrawLine(transform.position - Vector3.right * rayOffset,
            transform.position + Vector3.down * groundLength - Vector3.right * rayOffset);
    }
}