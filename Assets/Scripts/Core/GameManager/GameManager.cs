using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Image Fade;
    public float fadeDuration = 1f;
    Coroutine fadeCoroutine;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void FadeIn() {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f));
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f));
    }

    IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        float time = 0f;

        Color color = Fade.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            Fade.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        Fade.color = color;

        fadeCoroutine = null;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f));

        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return StartCoroutine(FadeRoutine(1f, 0f));
    }

}
