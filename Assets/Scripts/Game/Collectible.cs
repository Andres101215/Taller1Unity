using UnityEngine;

/// <summary>
/// A collectible crystal. Detects the player through its trigger and reports
/// it to the GameManager. The crystal hides itself once collected.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other == null || !TagUtil.HasTagInHierarchy(other.transform, "Jugador"))
        {
            return;
        }

        GameManager game = GameManager.Instance;
        if (game == null || !game.IsPlaying)
        {
            return;
        }

        game.CollectCrystal();
        AudioDirector.Instance?.PlayPickup();

        gameObject.SetActive(false);
    }
}
