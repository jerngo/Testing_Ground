using UnityEngine;

public class InteractPromptSpawner : MonoBehaviour
{
    [Header("Prompt Settings")]
    public InteractPromptUI promptPrefab;
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public string promptText = "E - Interact";
    public Sprite promptIcon;

    private InteractPromptUI activePrompt;

    public void ShowPrompt()
    {
        if (activePrompt != null) return;

        activePrompt = Instantiate(promptPrefab, transform.position + offset, Quaternion.identity);
        activePrompt.Setup(promptText, promptIcon);
    }

    public void HidePrompt()
    {
        if (activePrompt == null) return;

        Destroy(activePrompt.gameObject);
        activePrompt = null;
    }

    void OnDisable()
    {
        HidePrompt();
    }
}