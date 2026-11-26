using System;
using UnityEngine;

public enum GameState
{
    Intro,
    Playing,
    Finished
}

/// <summary>
/// Single source of truth for the run. Owns the objective, the score and the
/// game state machine. The UI subscribes to its events instead of polling.
///
/// Score rule (decision D5): +100 per crystal, +1 per second survived,
/// -10 per wall impact, clamped at 0 so the final screen never shows a negative score.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objective")]
    [Tooltip("How many crystals must be collected before the portal opens.")]
    public int totalCrystals = 5;

    [Header("Scoring")]
    public int pointsPerCrystal = 100;
    public int pointsPerSecond = 1;
    public int pointsPerImpact = -10;

    [Header("References")]
    [Tooltip("The portal. If left empty it is found automatically.")]
    public GoalZone goalZone;

    public GameState State { get; private set; } = GameState.Intro;
    public int CrystalsCollected { get; private set; }
    public int Score { get; private set; }
    public int ImpactCount { get; private set; }
    public float ElapsedTime { get; private set; }

    public bool IsPlaying => State == GameState.Playing;

    /// <summary>collected, total</summary>
    public event Action<int, int> OnCrystalCollected;
    public event Action<int> OnScoreChanged;
    public event Action OnGameFinished;

    private int secondsAwarded;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (goalZone == null)
        {
            goalZone = FindAnyObjectByType<GoalZone>();
        }
    }

    void Update()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        ElapsedTime += Time.deltaTime;

        int wholeSeconds = Mathf.FloorToInt(ElapsedTime);
        while (secondsAwarded < wholeSeconds)
        {
            secondsAwarded++;
            AddScore(pointsPerSecond);
        }
    }

    /// <summary>Called by the start screen when the player presses any key.</summary>
    public void StartGame()
    {
        if (State != GameState.Intro)
        {
            return;
        }

        State = GameState.Playing;
        SetCursorLocked(true);
    }

    /// <summary>Called by the player controller on every wall impact.</summary>
    public void RegisterImpact()
    {
        ImpactCount++;
        AddScore(pointsPerImpact);
    }

    /// <summary>Called by a collectible when the player picks it up.</summary>
    public void CollectCrystal()
    {
        if (!IsPlaying)
        {
            return;
        }

        CrystalsCollected++;
        AddScore(pointsPerCrystal);
        OnCrystalCollected?.Invoke(CrystalsCollected, totalCrystals);

        if (CrystalsCollected >= totalCrystals && goalZone != null)
        {
            goalZone.Activate();
        }
    }

    /// <summary>Called by the goal zone when the player enters an active portal.</summary>
    public void Finish()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        State = GameState.Finished;
        SetCursorLocked(false);
        OnGameFinished?.Invoke();
    }

    void AddScore(int amount)
    {
        int updated = Mathf.Max(0, Score + amount);
        if (updated == Score)
        {
            return;
        }

        Score = updated;
        OnScoreChanged?.Invoke(Score);
    }

    static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
