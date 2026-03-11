using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    // --- UNITY ARAYÜZÜNDEN DEÐÝÞTÝREBÝLMEN ÝÇÝN BURAYA EKLEDÝM ---
    public float wallPushX = 10f; // Sadece zýplamaya basýnca yatay fýrlatma
    public float wallPushY = 4f;  // Sadece zýplamaya basýnca dikey sekme
    public float wallJumpX = 3f;  // Duvara basarken týrmanma (Yatay)
    public float wallJumpY = 6f;  // Duvara basarken týrmanma (Dikey)
    // -------------------------------------------------------------

    private float wallJumpCooldown;
    private float originalGravity;

    // Yön bilgisini artýk Jump fonksiyonu da görsün diye buraya aldýk
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalGravity = body.gravityScale;
    }

    private void Update()
    {
        horizontalInput = 0f; // Her karede önce sýfýrlýyoruz

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f;
        }

        // YÜZÜNÜ DÖNDÜRME (FLIP)
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // --- ANÝMASYON DURUMLARI ---
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // --- DUVAR (WALL GRAB & JUMP) MANTIÐI ---
        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = originalGravity;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        // 1. DURUM: Yerdeysek normal zýpla
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
        // 2. DURUM: Duvardaysak ve Havadaysak
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                // Sadece zýplamaya basýldýysa (Push off)
                // SABÝT 10 VE 4 SAYILARINI BURADAN SÝLDÝM, YENÝ ÝSÝMLERÝ YAZDIM:
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallPushX, wallPushY);

                // Karakterin yüzünü fýrladýðý yöne (duvarýn tersine) çevir
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                // Duvara doðru basýlýrken zýplamaya basýldýysa (Wall Jump)
                // SABÝT 3 VE 6 SAYILARINI BURADAN SÝLDÝM, YENÝ ÝSÝMLERÝ YAZDIM:
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY);
            }

            // Cooldown sýfýrlamayý if-else dýþýna aldýk.
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
}