using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NewBasicPlatformerController : MonoBehaviour
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
    [SerializeField] CharacterController controller;
    [SerializeField] SkeletonAnimation skeletonAnimation;

    [Header("Moving")]
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float gravityScale = 6.6f;

    [Header("Jumping")]
    [SerializeField] float jumpSpeed = 25f;
    [SerializeField] float minimumJumpDuration = 0.5f;
    [SerializeField] float jumpInterruptFactor = 0.5f;
    [SerializeField] float forceCrouchVelocity = 25f;
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
    [SerializeField] float attackMixOutDuration = 0.1f;

    public event UnityAction OnJump;
    public event UnityAction OnLand;
    public event UnityAction OnHardLand;
    public event UnityAction OnAttackHit;

    const int LocomotionTrack = 0;
    const int ActionTrack = 1;

    Vector2 input;
    Vector3 velocity;

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

    CharacterState previousState = CharacterState.None;
    CharacterState currentState = CharacterState.None;

    Coroutine attackHitRoutine;

    void Reset()
    {
        controller = GetComponent<CharacterController>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        CacheGroundState();
        HandleLanding();
        HandleJump();
        HandleHorizontalMovement();
        ApplyGravity(dt);
        MoveCharacter(dt);
        UpdateState();
        UpdateLocomotionAnimation();
        UpdateFacing();
        InvokeGameplayEvents();
        ClearFrameInput();
    }

    void CacheGroundState()
    {
        isGrounded = controller.isGrounded;
        landed = !wasGrounded && isGrounded;
        hardLand = false;
        doJump = false;
    }

    void HandleLanding()
    {
        doCrouch = (isGrounded && input.y < -0.5f) || forceCrouchEndTime > Time.time;

        if (!landed)
            return;

        if (-velocity.y <= forceCrouchVelocity)
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
        velocity.y = jumpSpeed;
        minimumJumpEndTime = Time.time + minimumJumpDuration;
    }

    void InterruptJump()
    {
        if (velocity.y > 0)
            velocity.y *= jumpInterruptFactor;
    }

    void HandleHorizontalMovement()
    {
        velocity.x = 0;

        if (doCrouch)
            return;

        if (Mathf.Approximately(input.x, 0))
            return;

        float speed = Mathf.Abs(input.x) > 0.6f ? runSpeed : walkSpeed;
        velocity.x = speed * Mathf.Sign(input.x);
    }

    void ApplyGravity(float dt)
    {
        if (isGrounded)
            return;

        if (wasGrounded && velocity.y < 0)
        {
            velocity.y = 0;
            return;
        }

        velocity += UnityEngine.Physics.gravity * gravityScale * dt;
    }

    void MoveCharacter(float dt)
    {
        controller.Move(velocity * dt);
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

        currentState = velocity.y > 0
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

        TrackEntry entry = TryPlayAnimation(ActionTrack, attackAnim, false);

        if (entry == null)
        {
            isAttacking = false;
            return;
        }

        entry.Complete += OnAttackComplete;

        if (attackHitRoutine != null)
            StopCoroutine(attackHitRoutine);

        attackHitRoutine = StartCoroutine(AttackHitWindow());
    }

    IEnumerator AttackHitWindow()
    {
        yield return new WaitForSeconds(0.2f);

        OnAttackHit?.Invoke();

        yield return new WaitForSeconds(0.1f);

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

        skeletonAnimation.Skeleton.ScaleX = input.x > 0 ? 1 : -1;
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

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpPressed = true;

        if (context.canceled)
            jumpReleased = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        PlayAttack();
    }

}