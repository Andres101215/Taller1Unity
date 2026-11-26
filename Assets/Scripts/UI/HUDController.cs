using TMPro;
using UnityEngine;

/// <summary>
/// Heads-up display. Subscribes to the GameManager events instead of polling,
/// so the values on screen always match the game logic.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text scoreText;
    public TMP_Text crystalsText;
    public TMP_Text timeText;

    [Header("Optional")]
    public TMP_Text impactsText;

    [Header("Format")]
    public string scoreFormat = "Puntaje: {0}";
    public string crystalsFormat = "Cristales: {0} / {1}";
    public string timeFormat = "Tiempo: {0}";
    public string impactsFormat = "Impactos: {0}";

    GameManager game;

    void OnEnable()
    {
        game = GameManager.Instance;
        if (game == null)
        {
            return;
        }

        game.OnScoreChanged += HandleScoreChanged;
        game.OnCrystalCollected += HandleCrystalCollected;
        game.OnGameFinished += HandleGameFinished;

        HandleScoreChanged(game.Score);
        HandleCrystalCollected(game.CrystalsCollected, game.totalCrystals);
    }

    void OnDisable()
    {
        if (game == null)
        {
            return;
        }

        game.OnScoreChanged -= HandleScoreChanged;
        game.OnCrystalCollected -= HandleCrystalCollected;
        game.OnGameFinished -= HandleGameFinished;
    }

    void Update()
    {
        if (game == null)
        {
            return;
        }

        if (timeText != null)
        {
            timeText.text = string.Format(timeFormat, FormatTime(game.ElapsedTime));
        }

        if (impactsText != null)
        {
            impactsText.text = string.Format(impactsFormat, game.ImpactCount);
        }
    }

    void HandleScoreChanged(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }

    void HandleCrystalCollected(int collected, int total)
    {
        if (crystalsText != null)
        {
            crystalsText.text = string.Format(crystalsFormat, collected, total);
        }
    }

    void HandleGameFinished()
    {
        // The HUD stays visible but frozen: the GameManager already stopped
        // accepting score changes.
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }
}
