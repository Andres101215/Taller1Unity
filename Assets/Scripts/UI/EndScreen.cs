using TMPro;
using UnityEngine;

/// <summary>
/// End screen. Appears when the objective is completed and shows the final result.
/// </summary>
public class EndScreen : MonoBehaviour
{
    [Tooltip("Root panel shown when the game finishes.")]
    public GameObject panel;

    [Header("Text")]
    public TMP_Text titleText;
    public TMP_Text crystalsText;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Format")]
    public string title = "MISION COMPLETADA";
    public string crystalsFormat = "Cristales encontrados: {0} / {1}";
    public string scoreFormat = "Puntaje final: {0}";
    public string timeFormat = "Tiempo: {0}";

    GameManager game;

    void OnEnable()
    {
        game = GameManager.Instance;
        if (game == null)
        {
            return;
        }

        game.OnGameFinished += HandleGameFinished;
    }

    void OnDisable()
    {
        if (game == null)
        {
            return;
        }

        game.OnGameFinished -= HandleGameFinished;
    }

    void HandleGameFinished()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (crystalsText != null && game != null)
        {
            crystalsText.text = string.Format(crystalsFormat, game.CrystalsCollected, game.totalCrystals);
        }

        if (scoreText != null && game != null)
        {
            scoreText.text = string.Format(scoreFormat, game.Score);
        }

        if (timeText != null && game != null)
        {
            timeText.text = string.Format(timeFormat, HUDController.FormatTime(game.ElapsedTime));
        }
    }
}
