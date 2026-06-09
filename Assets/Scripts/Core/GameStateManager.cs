using System;
using UnityEngine;

public enum GameState
{
    Gameplay,
    Dialogue,
    Menu,
    Cutscene
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Gameplay;

    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameState] {newState}");
    }

    public bool Is(GameState state) => CurrentState == state;
}