using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectionInput : MonoBehaviour
{
    PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");
    }

    // --- Character Selection ---
    public void OnSelectCharacter(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            CharacterSelectionUI.Instance?.Toggle();
    }

    // --- Forward ke karakter aktif ---
    public void OnMove(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnMove(ctx);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnJump(ctx);
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnRun(ctx);
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnAttack(ctx);
    }

    public void OnShield(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnShield(ctx);
    }

    public void OnOpenMenu(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnOpenMenu(ctx);
    }

    public void OnOpenBag(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentController?.OnOpenBag(ctx);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentInteraction?.OnInteract(ctx);
    }

    public void OnSkillQ(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentSkillManager?.OnSkillQ(ctx);
    }

    public void OnSkillR(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentSkillManager?.OnSkillR(ctx);
    }

    public void OnSkillF(InputAction.CallbackContext ctx)
    {
        CharacterManager.Instance?.CurrentSkillManager?.OnSkillF(ctx);
    }
}