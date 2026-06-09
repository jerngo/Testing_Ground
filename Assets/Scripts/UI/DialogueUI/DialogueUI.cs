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

    public void Show() => dialoguePanel.SetActive(true);
    public void Hide()
    {
        dialoguePanel.SetActive(false);
        ClearChoices();
    }

    public void DisplayLine(DialogueLine line, System.Action<int> onChoice, System.Action onNext)
    {
        speakerNameText.text = line.speakerName;
        dialogueText.text = line.text;
        ClearChoices();

        bool hasChoices = line.choices != null && line.choices.Length > 0;
        nextButton.gameObject.SetActive(!hasChoices);
        choicesPanel.SetActive(hasChoices);

        if (hasChoices)
        {
            for (int i = 0; i < line.choices.Length; i++)
            {
                GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
                btn.GetComponentInChildren<TMP_Text>().text = line.choices[i].choiceText;
                int index = i; // capture index
                btn.GetComponent<Button>().onClick.AddListener(() => onChoice?.Invoke(index));
                activeChoiceButtons.Add(btn);
            }
        }
        else
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => onNext?.Invoke());
        }
    }

    void ClearChoices()
    {
        foreach (var btn in activeChoiceButtons) Destroy(btn);
        activeChoiceButtons.Clear();
    }
}