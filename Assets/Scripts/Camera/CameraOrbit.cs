using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Third-person camera that orbits the player with the mouse and follows it smoothly.
/// Replaces the old "ThirdPersonFollow" script (avance 1), which followed the player's
/// velocity direction and produced a strafe feedback loop (the camera rotated toward
/// the direction you were strafing, which rotated the camera again).
/// </summary>
public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The player root to follow (the GameObject with the CharacterController).")]
    public Transform player;

    [Header("Orbit")]
    [Tooltip("Offset from the player, in local space of the orbit orientation.")]
    public Vector3 offset = new Vector3(0f, 2.5f, -5f);

    [Tooltip("Mouse sensitivity in degrees per pixel of mouse movement.")]
    public float sensitivity = 0.12f;

    public float minPitch = -35f;
    public float maxPitch = 65f;

    [Header("Smoothing")]
    [Tooltip("Higher values make the camera stiffer. Decays exponentially, so it is frame-rate independent.")]
    public float smoothing = 12f;

    [Tooltip("Vertical offset used when looking at the player.")]
    public float lookHeight = 1.4f;

    private float yaw;
    private float pitch;

    void Start()
    {
        // Start behind the player, looking along the player's forward axis.
        if (player != null)
        {
            Vector3 euler = player.eulerAngles;
            yaw = euler.y;
        }
        pitch = 15f;
    }

    void Update()
    {
        // Only orbit while the mouse is captured, otherwise the camera spins on
        // any accidental mouse movement over the window.
        if (Cursor.lockState != CursorLockMode.Locked || Mouse.current == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        yaw += delta.x * sensitivity;
        pitch -= delta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desired = player.position + orbit * offset;

        // Exponential decay: frame-rate independent smoothing.
        // Lerp(a, b, smoothing * Time.deltaTime) is NOT equivalent and drifts between machines.
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desired, t);

        transform.LookAt(player.position + Vector3.up * lookHeight);
    }
}
