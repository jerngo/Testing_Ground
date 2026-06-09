using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject dialoguePanel;

    [Header("Teks")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Tombol")]
    public Button nextButton;

    [Header("Choices")]
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    private List<GameObject> activeChoiceButtons = new();

    // Menyimpan referensi action agar bisa di-remove secara manual (mengatasi kelemahan RemoveAllListeners)
    private System.Action currentNextAction;

    public void Show() => dialoguePanel.SetActive(true);

    public void Hide()
    {
        dialoguePanel.SetActive(false);
        ClearChoices();
        ResetNextButton();
    }

    public void DisplayLine(DialogueLine line, System.Action<int> onChoice, System.Action onNext)
    {
        // 1. Setup Teks
        if (speakerNameText != null) speakerNameText.text = line.speakerName;
        dialogueText.text = line.text;

        // 2. Bersihkan state lama
        ClearChoices();
        ResetNextButton();

        bool hasChoices = line.choices != null && line.choices.Length > 0;

        // 3. Atur Visibilitas UI
        nextButton.gameObject.SetActive(!hasChoices);
        choicesPanel.SetActive(hasChoices);

        // 4. Handle Cabang Dialog (Choices)
        if (hasChoices)
        {
            for (int i = 0; i < line.choices.Length; i++)
            {
                GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);

                // Menggunakan GetComponentInChildren dengan aman
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = line.choices[i].choiceText;

                Button buttonComp = btn.GetComponent<Button>();
                if (buttonComp != null)
                {
                    int index = i; // Capture index untuk lambda expression (Sudah Benar!)
                    buttonComp.onClick.AddListener(() => onChoice?.Invoke(index));
                }

                activeChoiceButtons.Add(btn);
            }
        }
        // 5. Handle Dialog Linear (Next Button)
        else
        {
            if (onNext != null)
            {
                // Bungkus ke dalam variabel lokal/global agar bisa dilepas nantinya
                currentNextAction = onNext;
                nextButton.onClick.AddListener(HandleNextClick);
            }
        }
    }

    private void HandleNextClick()
    {
        // Jalankan action, lalu langsung clear agar tidak terjadi double-click bug
        var action = currentNextAction;
        ResetNextButton();
        action?.Invoke();
    }

    private void ResetNextButton()
    {
        if (currentNextAction != null)
        {
            nextButton.onClick.RemoveListener(HandleNextClick);
            currentNextAction = null;
        }
    }

    void ClearChoices()
    {
        // Karena tombol choice dihancurkan, listener di dalamnya otomatis ikut hilang dari memori
        foreach (var btn in activeChoiceButtons)
        {
            if (btn != null) Destroy(btn);
        }
        activeChoiceButtons.Clear();
    }
}