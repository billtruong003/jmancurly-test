using Example.Dash;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
[DefaultExecutionOrder(100)]
public class SimpleAnimator : NetworkBehaviour
{
    private NetworkMecanimAnimator netAnim;
    private KCC kcc;
    private DashProcessor _dashProcessor;
    [SerializeField] private ParticleController _particleController;

    #region KCC State
    private Vector3 inputDirection;
    private bool isGrounded;
    private bool isMoving;
    private bool isSprint;
    private bool wasGroundedLastFrame = true;
    private bool isDashing;
    #endregion

    #region Animator Float Parameter
    private static readonly float IDLE = 0f;
    private static readonly float WALK = 3f;
    private static readonly float RUN = 6f;
    #endregion

    #region Animator Hash
    private static readonly int SPEED_PARAM_HASH = Animator.StringToHash("Speed");
    private static readonly int IS_GROUNDED_PARAM_HASH = Animator.StringToHash("IsGrounded");
    private static readonly int LANDING_TRIGGER_PARAM_HASH = Animator.StringToHash("Landing");
    private static readonly int DASH_TRIGGER_PARAM_HASH = Animator.StringToHash("Dash");
    #endregion

    protected void Awake()
    {
        if (TryGetComponent(out NetworkMecanimAnimator anim))
        {
            netAnim = anim;
        }
        else
        {
            Debug.LogError("Could not find NetworkMecanimAnimator component on object " + gameObject.name);
        }

        if (TryGetComponent(out KCC kccComponent))
        {
            kcc = kccComponent;
        }
        else
        {
            Debug.LogError("Could not find KCC component on object " + gameObject.name);
        }
        if (_particleController == null)
        {
            Debug.LogError("Could not find ParticleController component on object or children of " + gameObject.name);
        }
        _dashProcessor = kcc.GetComponentInChildren<DashProcessor>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsForward || IsProxy)
            return;

        CheckKCC();
        AnimatorBridge();

        wasGroundedLastFrame = isGrounded;
    }

    private void CheckKCC()
    {
        KCCData fixedData = kcc.FixedData;

        inputDirection = fixedData.InputDirection;
        isMoving = !inputDirection.IsZero();

        isGrounded = fixedData.IsGrounded;
        isSprint = fixedData.Sprint;

        if (_dashProcessor != null)
        {
            isDashing = _dashProcessor != null && _dashProcessor.IsDashing;
            Debug.Log($"CheckKCC - isDashing: {isDashing}");
        }
        else
        {
            _dashProcessor = kcc.GetComponentInChildren<DashProcessor>();
        }
    }
    private void AnimatorBridge()
    {
        Animator animator = netAnim.Animator;

        animator.SetBool(IS_GROUNDED_PARAM_HASH, isGrounded);

        if (!isGrounded && wasGroundedLastFrame) // Detect Jump
        {
            _particleController.DeactivateAllTrails(); // Tắt trails khi nhảy
            _particleController.ActivateParticleByName(ParticleController.CharacterVFX.Jump); // Kích hoạt Jump particle
        }
        if (isGrounded && !wasGroundedLastFrame) // Detect Landing
        {
            animator.SetTrigger(LANDING_TRIGGER_PARAM_HASH);
            _particleController.ActivateParticleByName(ParticleController.CharacterVFX.Land); // Kích hoạt Land particle
        }

        // Dash
        if (isDashing)
        {
            Debug.Log("AnimatorBridge - Setting Dash Trigger!");
            animator.SetTrigger(DASH_TRIGGER_PARAM_HASH);
            _particleController.ActivateParticleByName(ParticleController.CharacterVFX.Dash); // Bật Dash trail
            _particleController.DeactivateTrailByName(ParticleController.CharacterVFX.Move); // Optional: Tắt Move trail khi dash
        }
        else
        {
            _particleController.DeactivateTrailByName(ParticleController.CharacterVFX.Dash); // Tắt Dash trail khi không dash
        }

        // Move
        if (isMoving)
        {
            animator.SetFloat(SPEED_PARAM_HASH, isSprint ? RUN : WALK);
            _particleController.ActivateParticleByName(ParticleController.CharacterVFX.Move); // Bật Move trail khi di chuyển
        }
        else
        {
            animator.SetFloat(SPEED_PARAM_HASH, IDLE);
            _particleController.DeactivateTrailByName(ParticleController.CharacterVFX.Move); // Tắt Move trail khi không di chuyển
        }
    }
}