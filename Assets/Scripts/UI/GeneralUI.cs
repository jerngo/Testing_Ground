using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUI : MonoBehaviour
{
    public void OpenTab(GameObject openTab) { 
        openTab.SetActive(true);
    }

    public void CloseThisTab(GameObject openTab) {
        openTab.SetActive(false);
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("LanguageScene");
    }
}
