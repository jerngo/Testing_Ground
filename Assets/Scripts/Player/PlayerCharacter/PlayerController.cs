using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    public bool IsDead=false;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    Vector2 moveInput;
    bool isRunning;
    public bool isShielding;
    bool isGrounded;

    public Transform playerHit;
    float hitOffsetX = 0.1793689f;

    public bool interactPressed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!IsDead) {
            interactPressed = Keyboard.current.eKey.wasPressedThisFrame;

            CheckGround();
            Move();
            UpdateAnimation();
        }
    }

    void Move()
    {
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (isShielding) targetSpeed = 0;

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, moveInput.x * targetSpeed, 0.2f),
            rb.linearVelocity.y
        );

        // arah karakter
        if (moveInput.x < 0) { 
            sr.flipX = true;

            Vector3 pos = playerHit.localPosition;
            pos.x = -hitOffsetX;
            playerHit.localPosition = pos;
        }

        if (moveInput.x > 0) { 
            sr.flipX = false;

            Vector3 pos = playerHit.localPosition;
            pos.x = hitOffsetX;
            playerHit.localPosition = pos;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    void UpdateAnimation()
    {
        anim.SetFloat("speed", Mathf.Abs(moveInput.x));
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("shield", isShielding);
    }

    // INPUT EVENTS (Invoke Unity Events)

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            isRunning = true;
        
        if (ctx.canceled)
            isRunning = false;
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            anim.SetTrigger("attack");
        }
    }

    public void OnShield(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            isShielding = true;

        if (ctx.canceled)
            isShielding = false;
    }

    
    Coroutine hitCoroutine;
    public void PlayerHitActive()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        playerHit.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        playerHit.gameObject.SetActive(false);

        hitCoroutine = null;
    }

    public void CancelHit()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        playerHit.gameObject.SetActive(false);
    }

    public GameSaveUI gameSaveUI;
    public BagUI bagUI;
    public void OnOpenMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            gameSaveUI.ToggleMenu();
        }
    }

    public void OnOpenBag(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            bagUI.ToggleBag();
        }
    }

}
