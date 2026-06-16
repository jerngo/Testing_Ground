using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Leaderboard Manager
/// - Input dipanggil dari Player Input component (Invoke Unity Events)
/// - Assign OnToggleLeaderboard ke action H di Player Input Inspector
/// - Fetch data dari https://dummyjson.com/users saat pertama kali dibuka
///
/// Setup di Inspector:
/// 1. Buat Canvas > Panel (leaderboardPanel)
/// 2. Dalam panel: ScrollView > Viewport > Content → assign ke rowContainer
/// 3. Buat prefab Row: RankText + NameText + ScoreText (TMP_Text)
/// 4. Assign loadingIndicator dan errorIndicator (GameObject di dalam panel)
/// 5. Di komponen Player Input → Events → cari action H → assign OnToggleLeaderboard
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject leaderboardPanel;

    [Header("Row")]
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowContainer;

    [Header("UI States")]
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject errorIndicator;

    private const string API_URL = "https://dummyjson.com/users?limit=20";

    private bool _isOpen = false;
    private bool _hasLoaded = false;

    private void Start()
    {
        leaderboardPanel.SetActive(false);
    }

    public void OnToggleLeaderboard(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _isOpen = !_isOpen;
        leaderboardPanel.SetActive(_isOpen);

        if (_isOpen && !_hasLoaded)
            StartCoroutine(FetchLeaderboard());
    }

    private IEnumerator FetchLeaderboard()
    {
        SetState(loading: true, error: false);

        using UnityWebRequest req = UnityWebRequest.Get(API_URL);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[Leaderboard] Gagal fetch: {req.error}");
            SetState(loading: false, error: true);
            yield break;
        }

        UsersResponse resp = JsonUtility.FromJson<UsersResponse>(req.downloadHandler.text);
        if (resp?.users == null)
        {
            SetState(loading: false, error: true);
            yield break;
        }

        SetState(loading: false, error: false);
        PopulateRows(resp.users);
        _hasLoaded = true;
    }

    private void PopulateRows(LeaderboardEntry[] users)
    {
        foreach (Transform child in rowContainer)
            Destroy(child.gameObject);

        var sorted = new List<(LeaderboardEntry u, int score)>();
        foreach (var u in users)
            sorted.Add((u, FakeScore(u.id)));
        sorted.Sort((a, b) => b.score.CompareTo(a.score));

        for (int i = 0; i < sorted.Count; i++)
        {
            var (u, score) = sorted[i];
            int rank = i + 1;

            GameObject row = Instantiate(rowPrefab, rowContainer);

            var rankText = row.transform.Find("RankText")?.GetComponent<TMP_Text>();
            var nameText = row.transform.Find("NameText")?.GetComponent<TMP_Text>();
            var scoreText = row.transform.Find("ScoreText")?.GetComponent<TMP_Text>();

            if (rankText != null) rankText.text = GetRankDisplay(rank);
            if (nameText != null) nameText.text = $"{u.firstName} {u.lastName}";
            if (scoreText != null) scoreText.text = score.ToString("N0");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rowContainer as RectTransform);
    }

    private static string GetRankDisplay(int rank) => rank switch
    {
        1 => "1#",
        2 => "2#",
        3 => "3#",
        _ => rank.ToString()
    };

    private static int FakeScore(int id) => 1000 + (id * 1234567) % 9000;

    private void SetState(bool loading, bool error)
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(loading);
        if (errorIndicator != null) errorIndicator.SetActive(error);
        if (rowContainer != null) rowContainer.gameObject.SetActive(!loading && !error);
    }
}