using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class NewBasicPlatformerController2D : MonoBehaviour
{
    public enum CharacterState
    {
        None,
        Idle,
        Walk,
        Run,
        Crouch,
        Rise,
        Fall
    }

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SkeletonAnimation skeletonAnimation;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [Header("Moving")]
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 7f;

    [Header("Jumping")]
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float minimumJumpDuration = 0.15f;
    [SerializeField] float jumpInterruptFactor = 0.5f;
    [SerializeField] float forceCrouchVelocity = 18f;
    [SerializeField] float forceCrouchDuration = 0.5f;

    [Header("Animation Names")]
    [SerializeField] string idleAnim = "idle";
    [SerializeField] string walkAnim = "walk";
    [SerializeField] string runAnim = "run";
    [SerializeField] string crouchAnim = "crouch";
    [SerializeField] string riseAnim = "rise";
    [SerializeField] string fallAnim = "fall";
    [SerializeField] string attackAnim = "attack";

    [Header("Action Settings")]
    [SerializeField] float attackHitTime = 0.1f;
    [SerializeField] float attackMixOutDuration = 0.1f;

    [SerializeField] PlayerCombat playerCombat;

    public event UnityAction OnJump;
    public event UnityAction OnLand;
    public event UnityAction OnHardLand;
    public event UnityAction OnAttackHit;

    public GameSaveUI gameSaveUI;
    public InventoryUI inventoryUI;

    const int LocomotionTrack = 0;
    const int ActionTrack = 1;

    Vector2 input;

    bool jumpPressed;
    bool jumpReleased;
    bool isAttacking;

    bool isGrounded;
    bool wasGrounded;
    bool landed;
    bool hardLand;
    bool doCrouch;
    bool doJump;

    float minimumJumpEndTime;
    float forceCrouchEndTime;
    float lastYVelocity;

    Coroutine attackHitRoutine;

    CharacterState previousState = CharacterState.None;
    CharacterState currentState = CharacterState.None;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();

        rb.freezeRotation = true;
    }

    void Update()
    {
        CacheGroundState();
        HandleLanding();
        HandleJump();
        UpdateState();
        UpdateLocomotionAnimation();
        UpdateFacing();
        InvokeGameplayEvents();
        ClearFrameInput();

        lastYVelocity = rb.linearVelocity.y;
    }

    void FixedUpdate()
    {
        HandleHorizontalMovement();
    }

    void CacheGroundState()
    {
        isGrounded = CheckGrounded();
        landed = !wasGrounded && isGrounded;
        hardLand = false;
        doJump = false;
    }

    bool CheckGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void HandleLanding()
    {
        doCrouch = (isGrounded && input.y < -0.5f) || forceCrouchEndTime > Time.time;

        if (!landed)
            return;

        if (-lastYVelocity <= forceCrouchVelocity)
            return;

        hardLand = true;
        doCrouch = true;
        forceCrouchEndTime = Time.time + forceCrouchDuration;
    }

    void HandleJump()
    {
        if (doCrouch)
            return;

        if (isGrounded)
        {
            if (jumpPressed)
                StartJump();

            return;
        }

        if (jumpReleased && Time.time < minimumJumpEndTime)
            InterruptJump();
    }

    void StartJump()
    {
        doJump = true;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpSpeed
        );

        minimumJumpEndTime = Time.time + minimumJumpDuration;
    }

    void InterruptJump()
    {
        if (rb.linearVelocity.y <= 0)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            rb.linearVelocity.y * jumpInterruptFactor
        );
    }

    void HandleHorizontalMovement()
    {
        float xVelocity = 0f;

        if (!doCrouch && !Mathf.Approximately(input.x, 0))
        {
            float speed = Mathf.Abs(input.x) > 0.6f ? runSpeed : walkSpeed;
            xVelocity = speed * Mathf.Sign(input.x);
        }

        rb.linearVelocity = new Vector2(
            xVelocity,
            rb.linearVelocity.y
        );

        wasGrounded = isGrounded;
    }

    void UpdateState()
    {
        if (isGrounded)
        {
            if (doCrouch)
                currentState = CharacterState.Crouch;
            else if (Mathf.Approximately(input.x, 0))
                currentState = CharacterState.Idle;
            else
                currentState = Mathf.Abs(input.x) > 0.6f
                    ? CharacterState.Run
                    : CharacterState.Walk;

            return;
        }

        currentState = rb.linearVelocity.y > 0
            ? CharacterState.Rise
            : CharacterState.Fall;
    }

    void UpdateLocomotionAnimation()
    {
        if (previousState == currentState)
            return;

        previousState = currentState;

        TryPlayAnimation(
            LocomotionTrack,
            GetAnimationName(currentState),
            true
        );
    }

    TrackEntry TryPlayAnimation(int trackIndex, string animName, bool loop)
    {
        if (string.IsNullOrEmpty(animName))
            return null;

        Spine.Animation animation =
            skeletonAnimation.Skeleton.Data.FindAnimation(animName);

        if (animation == null)
        {
            Debug.LogWarning($"Animation '{animName}' not found.");
            return null;
        }

        return skeletonAnimation.AnimationState.SetAnimation(
            trackIndex,
            animation,
            loop
        );
    }

    string GetAnimationName(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Idle: return idleAnim;
            case CharacterState.Walk: return walkAnim;
            case CharacterState.Run: return runAnim;
            case CharacterState.Crouch: return crouchAnim;
            case CharacterState.Rise: return riseAnim;
            case CharacterState.Fall: return fallAnim;
            default: return idleAnim;
        }
    }

    void PlayAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        TrackEntry entry = TryPlayAnimation(
            ActionTrack,
            attackAnim,
            false
        );

        if (entry == null)
        {
            isAttacking = false;
            return;
        }

        entry.Complete += OnAttackComplete;

        if (attackHitRoutine != null)
            StopCoroutine(attackHitRoutine);

        attackHitRoutine = StartCoroutine(AttackHitTimer());
    }

    IEnumerator AttackHitTimer()
    {
        yield return new WaitForSeconds(attackHitTime);

        OnAttackHit?.Invoke();

        attackHitRoutine = null;
    }

    void OnAttackComplete(TrackEntry entry)
    {
        entry.Complete -= OnAttackComplete;

        skeletonAnimation.AnimationState.SetEmptyAnimation(
            ActionTrack,
            attackMixOutDuration
        );

        isAttacking = false;
    }

    void UpdateFacing()
    {
        if (Mathf.Approximately(input.x, 0))
            return;

        float direction = input.x > 0 ? 1 : -1;

        skeletonAnimation.Skeleton.ScaleX = direction;

        if (playerCombat != null)
            playerCombat.SetFacingDirection(direction);
    }

    void InvokeGameplayEvents()
    {
        if (doJump)
            OnJump?.Invoke();

        if (!landed)
            return;

        if (hardLand)
            OnHardLand?.Invoke();
        else
            OnLand?.Invoke();
    }

    void ClearFrameInput()
    {
        jumpPressed = false;
        jumpReleased = false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        input = ctx.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            jumpPressed = true;

        if (ctx.canceled)
            jumpReleased = true;
    }

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
            inventoryUI.ToggleInventory();
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        PlayAttack();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}