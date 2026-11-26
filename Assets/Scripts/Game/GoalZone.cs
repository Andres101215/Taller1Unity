using UnityEngine;

/// <summary>
/// The exit portal. It stays locked until every crystal has been collected
/// (decision D1b): the trigger is always active so the player can walk up and
/// read how many crystals are missing, but only an active portal ends the run.
///
/// The two visual states (locked / active) are just a material swap plus a light,
/// so no animation is required.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour
{
    public static GoalZone Instance { get; private set; }

    [Header("Visual states")]
    [Tooltip("Renderer of the portal mesh.")]
    public Renderer portalRenderer;

    [Tooltip("Material used while the portal is locked.")]
    public Material lockedMaterial;

    [Tooltip("Material used once the portal opens.")]
    public Material activeMaterial;

    [Tooltip("Light enabled when the portal opens so it is visible from far away.")]
    public Light activeLight;

    public bool IsActive { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ApplyVisualState(false);
    }

    /// <summary>Opens the portal. Called by the GameManager when the last crystal is collected.</summary>
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        ApplyVisualState(true);
    }

    void ApplyVisualState(bool active)
    {
        if (portalRenderer != null)
        {
            Material target = active ? activeMaterial : lockedMaterial;
            if (target != null)
            {
                portalRenderer.sharedMaterial = target;
            }
        }

        if (activeLight != null)
        {
            activeLight.enabled = active;
        }
    }

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

        if (IsActive)
        {
            game.Finish();
            return;
        }

        int missing = game.totalCrystals - game.CrystalsCollected;
        Debug.Log("El portal esta sellado - faltan " + missing + " cristales.");

        if (AudioDirector.Instance != null)
        {
            AudioDirector.Instance.PlayLocked();
        }
    }
}
