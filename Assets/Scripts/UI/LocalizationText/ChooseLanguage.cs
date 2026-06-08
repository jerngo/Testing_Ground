using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLanguage : MonoBehaviour
{
    public void ChooseEng() {
        LocalizationManager.Instance.LoadLanguage("en");
        SceneManager.LoadScene("LoginScene");
    }

    public void ChooseInd()
    {
        LocalizationManager.Instance.LoadLanguage("id");
        SceneManager.LoadScene("LoginScene");
    }

    public void ChooseJP()
    {
        LocalizationManager.Instance.LoadLanguage("jp");
        SceneManager.LoadScene("LoginScene");
    }

    public void ChooseCN()
    {
        LocalizationManager.Instance.LoadLanguage("cn");
        SceneManager.LoadScene("LoginScene");
    }
}
