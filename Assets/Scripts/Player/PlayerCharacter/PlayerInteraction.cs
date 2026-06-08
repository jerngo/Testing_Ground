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
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (
            interactable != null &&
            interactable == currentInteractable
        )
        {
            currentInteractable = null;
        }
    }
}