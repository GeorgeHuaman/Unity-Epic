using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int JumpHash = Animator.StringToHash("JumpTrigger");
    private static readonly int FreeFallHash = Animator.StringToHash("FreeFall");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private RuntimeAnimatorController runtimeAnimator;
    private GameObject avatar;
    private PlayerFusion playerFusion;
    private Animator animator;
    private bool inputEnabled = true;
    private bool isInitialized;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Init()
    {
        playerFusion = GetComponent<PlayerFusion>();
        isInitialized = true;
    }
    public void Setup(GameObject target, RuntimeAnimatorController runtimeAnimatorController)
    {
        if (!isInitialized)
        {
            Init();
        }

        avatar = target;
        animator = avatar.GetComponent<Animator>();
        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.applyRootMotion = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (avatar == null)
            return;
        
        UpdateAnimator();
    }
    private void UpdateAnimator()
    {
        var isGrounded = playerFusion.kcc.FixedData.IsGrounded;
        animator.SetFloat(MoveSpeedHash, playerFusion.kcc.FixedData.RealVelocity.magnitude);
        animator.SetBool(IsGroundedHash, isGrounded);
        if (isGrounded)
        {
            animator.SetBool(FreeFallHash, false);
        }
        else
        {
          animator.SetBool(FreeFallHash, true);
        }
    }

    public void OnJump()
    {
       animator.SetTrigger(JumpHash);
    }
}
