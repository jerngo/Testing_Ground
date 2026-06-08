using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    IInteractable currentInteractable;

    PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (
            currentInteractable != null &&
            controller.interactPressed
        )
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