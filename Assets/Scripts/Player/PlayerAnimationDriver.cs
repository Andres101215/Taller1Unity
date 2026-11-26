using UnityEngine;

/// <summary>
/// Feeds the Animator Controller with the state of the player.
///
/// Expected Animator parameters (see the plan, T-S5):
///   Speed    (float)  horizontal speed, used by the Locomotion blend tree
///   Grounded (bool)   true while the character stands on something
///   Jump     (trigger) fired on the frame the character leaves the ground
///
/// The Animator lives on the visual model (a child of the player root), not on
/// the root itself: the root is what the CharacterController moves, the model is
/// what gets animated.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationDriver : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player controller that owns the movement state.")]
    public PlayerController playerController;

    [Header("Parameters")]
    public string speedParameter = "Speed";
    public string groundedParameter = "Grounded";
    public string jumpParameter = "Jump";

    [Header("Tuning")]
    [Tooltip("Damping applied to the Speed parameter so the blend is not jittery.")]
    public float speedDamping = 0.15f;

    [Tooltip("Speed above which the character is considered running.")]
    public float runThreshold = 3.5f;

    private Animator animator;
    private PlayerController controller;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        controller = playerController;
    }

    void OnEnable()
    {
        if (controller != null)
        {
            controller.Jumped += HandleJumped;
        }
    }

    void OnDisable()
    {
        if (controller != null)
        {
            controller.Jumped -= HandleJumped;
        }
    }

    void Update()
    {
        if (animator == null || controller == null)
        {
            return;
        }

        CharacterController cc = controller.GetComponent<CharacterController>();
        Vector3 velocity = cc != null ? cc.velocity : Vector3.zero;

        // Horizontal speed only: the vertical component is the jump arc, not movement.
        float horizontalSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;

        animator.SetFloat(speedParameter, horizontalSpeed, speedDamping, Time.deltaTime);
        animator.SetBool(groundedParameter, controller.IsGrounded);
    }

    void HandleJumped()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(jumpParameter);
        AudioDirector.Instance?.PlayJump();
    }
}
