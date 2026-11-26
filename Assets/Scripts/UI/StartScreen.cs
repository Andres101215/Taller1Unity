using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Start screen. Shows the experience name, the objective and the controls, and
/// waits for any key press to begin. The player does not move until the game
/// starts (see GameManager.StartGame).
/// </summary>
public class StartScreen : MonoBehaviour
{
    [Tooltip("Root panel shown while the game is in the Intro state.")]
    public GameObject panel;

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    void Update()
    {
        GameManager game = GameManager.Instance;
        if (game == null || game.State != GameState.Intro)
        {
            return;
        }

        bool anyKey = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        if (!anyKey && (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame))
        {
            return;
        }

        game.StartGame();

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
