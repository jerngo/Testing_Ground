using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    public static CharacterSelectionUI Instance { get; private set; }

    [Header("Panel & Buttons")]
    public GameObject panel;
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        buttonA.onClick.AddListener(() => OnSelect(0));
        buttonB.onClick.AddListener(() => OnSelect(1));
        buttonC.onClick.AddListener(() => OnSelect(2));

        panel.SetActive(false);
    }

    void OnSelect(int index)
    {
        CharacterManager.Instance.SelectCharacter(index);
        // panel akan di-hide dari CharacterManager setelah swap selesai
    }

    public void Toggle()
    {
        if (panel.activeSelf) Hide();
        else Show();
    }

    public void Show()
    {
        panel.SetActive(true);
        // Pause gameplay saat panel terbuka (opsional)
        // GameStateManager.Instance.Set(GameState.Menu);
    }

    public void Hide()
    {
        panel.SetActive(false);
        // GameStateManager.Instance.Set(GameState.Gameplay);
    }
}