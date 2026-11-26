using UnityEngine;

/// <summary>
/// Ground detection for the player. A CharacterController does not report
/// "am I standing on something" on its own, and Unity has no built-in
/// isGrounded flag on it (that lives on CharacterController.isGrounded but
/// it is unreliable on slopes and with step offsets).
///
/// This does an explicit sphere cast below the capsule so jumping and
/// slope behaviour are predictable.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class GroundChecker : MonoBehaviour
{
    [Tooltip("Extra distance below the capsule to probe for ground.")]
    public float groundCheckDistance = 0.25f;

    [Tooltip("Radius of the probe sphere. Slightly smaller than the capsule radius.")]
    public float groundCheckRadius = 0.28f;

    [Tooltip("Layers considered ground. Defaults to everything except the player layer.")]
    public LayerMask groundLayers;

    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;

    private CharacterController controller;
    private bool layersConfigured;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        ConfigureLayers();
    }

    void ConfigureLayers()
    {
        if (layersConfigured)
        {
            return;
        }

        // Default to "everything except the player layer" so the probe never
        // hits the player's own collider.
        int playerLayer = LayerMask.NameToLayer("Jugador");
        int mask = ~0;
        if (playerLayer >= 0)
        {
            mask &= ~(1 << playerLayer);
        }

        groundLayers = mask;
        layersConfigured = true;
    }

    void Update()
    {
        ConfigureLayers();
        Check();
    }

    void Check()
    {
        float radius = groundCheckRadius > 0f
            ? Mathf.Min(groundCheckRadius, controller.radius * 0.95f)
            : controller.radius * 0.95f;

        // Probe from just below the capsule centre so the sphere starts outside
        // the capsule volume (a cast that starts inside a collider is unreliable).
        Vector3 origin = transform.position
            + Vector3.up * Mathf.Max(0.05f, controller.height * 0.5f - radius - 0.02f);

        float distance = controller.height * 0.5f + groundCheckDistance;

        IsGrounded = Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, distance, groundLayers, QueryTriggerInteraction.Ignore);

        if (IsGrounded)
        {
            GroundNormal = hit.normal;
        }
        else
        {
            GroundNormal = Vector3.up;
        }
    }
}
