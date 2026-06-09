using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    IInteractable currentInteractable;

    PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interact pressed");
        if (!ctx.performed) return;
        if (!GameStateManager.Instance.Is(GameState.Gameplay)) return;

        if (currentInteractable != null)
        {
            currentInteractable.Interact(controller);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;

            // Tampilkan prompt
            InteractPromptSpawner spawner = other.GetComponent<InteractPromptSpawner>();
            spawner?.ShowPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
        {
            // Sembunyikan prompt
            InteractPromptSpawner spawner = other.GetComponent<InteractPromptSpawner>();
            spawner?.HidePrompt();

            currentInteractable = null;
        }
    }
}