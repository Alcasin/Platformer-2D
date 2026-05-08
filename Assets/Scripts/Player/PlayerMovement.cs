using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    [Header("Movement Parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Jump Parameters")]
    [SerializeField] private float wallPushX = 10f;
    [SerializeField] private float wallPushY = 4f;
    [SerializeField] private float wallJumpX = 3f;
    [SerializeField] private float wallJumpY = 6f;

    private float wallJumpCooldown;
    private float originalGravity;
    private float horizontalInput;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalGravity = body.gravityScale;
    }

    private void Update()
    {
        horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f;
        }

        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded())
            {
                body.linearVelocity = Vector2.zero;
                body.gravityScale = 0;
            }
            else
            {
                body.gravityScale = originalGravity;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                Jump(); 
                if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded())
                {
                    SoundManager.instance.PlaySound(jumpSound);
                }
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            SoundManager.instance.PlaySound(jumpSound);

            if (horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallPushX, wallPushY);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY);
            }

            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}