using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Won, Lost }

/// Game state, rescue count, score, lives, and win/lose logic.

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rules (tune these for your design goal)")]
    [SerializeField] private int creaturesToWin = 5;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int pointsPerRescue = 100;

    public GameState State { get; private set; } = GameState.Playing;
    public int RescuedCount { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int CreaturesToWin => creaturesToWin;
    public string EndMessage { get; private set; } = "";

    public event Action<GameState> StateChanged;

    private void Awake()
    {
        Instance = this;
        Lives = startingLives;
        State = GameState.Playing;
    }

    private void Update()
    {
        if (State != GameState.Playing && SimpleInput.RestartPressed)
            Restart();
    }

    /// Called when a creature reaches the safe zone.
    public void RegisterRescue()
    {
        if (State != GameState.Playing) return;

        RescuedCount++;
        Score += pointsPerRescue;
        Debug.Log($"Rescued {RescuedCount}/{creaturesToWin}  Score: {Score}");

        if (RescuedCount >= creaturesToWin)
            SetState(GameState.Won, "You brought every creature to safety!");
    }

    /// Called when the player dies.
    public void LoseLife(string reason)
    {
        if (State != GameState.Playing) return;

        Lives--;
        Debug.Log($"Lost a life ({reason}). Lives left: {Lives}");

        if (Lives <= 0)
        {
            Lives = 0;
            SetState(GameState.Lost, $"Out of lives! Cause: {reason}");
        }
    }

    private void SetState(GameState newState, string message)
    {
        State = newState;
        EndMessage = message;
        Debug.Log($"Game state -> {State}. {message}");
        StateChanged?.Invoke(State);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
