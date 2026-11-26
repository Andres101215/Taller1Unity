using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player controller for the humanoid character.
///
/// Migrated from the advance-1 version (Rigidbody + AddForce on a sphere):
///  - Movement is now velocity based on a CharacterController, so it can stop
///    instantly instead of coasting (the old Rigidbody had zero linear damping).
///  - Jump is gated by an explicit ground check.
///  - Running and camera-relative strafing are supported.
///  - Wall impacts are detected with OnControllerColliderHit. CharacterController
///    does NOT raise OnCollisionEnter, so the old callback silently stopped working.
///
/// The Rigidbody and SphereCollider of the old sphere prefab must be removed:
/// a Rigidbody and a CharacterController on the same GameObject fight each other.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Walking speed in meters per second.")]
    public float walkSpeed = 3f;

    [Tooltip("Running speed in meters per second (Shift).")]
    public float runSpeed = 6.5f;

    [Tooltip("How fast the character reaches (and leaves) the target speed.")]
    public float acceleration = 30f;

    [Tooltip("Downward acceleration. CharacterController does not apply gravity by itself.")]
    public float gravity = -20f;

    [Tooltip("Maximum jump height in meters. The launch speed is derived from it.")]
    public float jumpHeight = 1.2f;

    [Header("References")]
    [Tooltip("Camera used to compute the movement direction. Falls back to Camera.main.")]
    public Transform cameraTransform;

    [Tooltip("The visual model (child of this GameObject) that is rotated toward the movement direction.")]
    public Transform modelRoot;

    [Header("Rotation")]
    [Tooltip("How fast the model turns toward the movement direction.")]
    public float rotationSpeed = 14f;

    /// <summary>Raised on the frame the character leaves the ground.</summary>
    public event System.Action Jumped;

    /// <summary>Raised on every wall impact. Carries the new total impact count.</summary>
    public event System.Action<int> Impacted;

    public int ImpactCount { get; private set; }

    public bool IsRunning { get; private set; }
    public bool IsGrounded => groundChecker != null && groundChecker.IsGrounded;

    private CharacterController controller;
    private GroundChecker groundChecker;
    private Transform cachedCamera;
    private float verticalSpeed;
    private Vector3 facingDirection = Vector3.forward;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundChecker = GetComponent<GroundChecker>();

        if (cameraTransform == null && Camera.main != null)
        {
            cachedCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        bool canMove = GameManager.Instance == null || GameManager.Instance.IsPlaying;

        Vector2 input = canMove ? ReadMoveInput() : Vector2.zero;
        IsRunning = canMove && ReadRunInput();

        float speed = IsRunning ? runSpeed : walkSpeed;
        Vector3 desiredDirection = DirectionRelativeToCamera(input);

        Vector3 current = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        Vector3 target = desiredDirection * speed;
        Vector3 horizontal = Vector3.MoveTowards(current, target, acceleration * Time.deltaTime);

        ApplyGravityAndJump();

        Vector3 motion = horizontal + Vector3.up * verticalSpeed;
        controller.Move(motion * Time.deltaTime);

        RotateModelToward(horizontal);
    }

    void ApplyGravityAndJump()
    {
        bool grounded = IsGrounded;

        if (grounded && verticalSpeed <= 0f)
        {
            // Small downward bias keeps the character glued to slopes and steps.
            verticalSpeed = -2f;

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                verticalSpeed = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
                Jumped?.Invoke();
            }
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }
    }

    void RotateModelToward(Vector3 horizontal)
    {
        if (modelRoot == null || horizontal.sqrMagnitude < 0.01f)
        {
            return;
        }

        facingDirection = Vector3.Slerp(facingDirection, horizontal.normalized, Mathf.Clamp01(rotationSpeed * Time.deltaTime));
        modelRoot.rotation = Quaternion.LookRotation(facingDirection, Vector3.up);
    }

    Vector2 ReadMoveInput()
    {
        if (Keyboard.current == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;

        return new Vector2(horizontal, vertical);
    }

    bool ReadRunInput()
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        return Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
    }

    Vector3 DirectionRelativeToCamera(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
        {
            return Vector3.zero;
        }

        Transform cam = cameraTransform != null ? cameraTransform : cachedCamera;
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
            cachedCamera = cam;
        }

        if (cam == null)
        {
            return new Vector3(input.x, 0f, input.y).normalized;
        }

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized;
    }

    /// <summary>
    /// CharacterController collision callback. This replaces the OnCollisionEnter of
    /// the old Rigidbody based controller, which is never called for a CharacterController.
    /// </summary>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null || !HasTagInHierarchy(hit.collider.transform, "Muro"))
        {
            return;
        }

        ImpactCount++;
        Impacted?.Invoke(ImpactCount);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterImpact();
        }
    }

    /// <summary>
    /// Checks the collider and its ancestors for a tag. Needed because the imported
    /// model prefabs carry the "Muro" tag on the instance root while their
    /// colliders live on child objects.
    /// </summary>
    static bool HasTagInHierarchy(Transform target, string tag)
    {
        return TagUtil.HasTagInHierarchy(target, tag);
    }
}
