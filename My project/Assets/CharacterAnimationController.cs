using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Components")]
    
    [SerializeField] private Animator[] _animators;
    private static readonly int IsAimingAttackHash = Animator.StringToHash("IsAimingAttack");
    private static readonly int IsAimingSkillHash = Animator.StringToHash("IsAimingSkill");
    private static readonly int IsSelectingSkillHash = Animator.StringToHash("IsSelectingSkill");
    private static readonly int TriggerActionHash = Animator.StringToHash("TriggerAction");
    
    private static readonly int SkillIndexHash = Animator.StringToHash("SkillIndex");
    private static readonly int HitTriggerHash = Animator.StringToHash("TriggerHit");
    private static readonly int LowHealthBoolHash = Animator.StringToHash("IsLowHealth");

    public enum CharacterState
    {
        Idle,
        SelectingSkill, // Loop
        AimingBasic,    // Loop
        Attacking,      // Action
        AimingSkill,    // Loop
        CastingSkill,   // Action
        TakingDamage,
        Dead
    }

    private CharacterState _currentState;

    private void Awake()
    {
        if (_animators == null || _animators.Length == 0)
            _animators = GetComponentsInChildren<Animator>();
            
        foreach (var anim in _animators)
        {
            if (anim == null) continue;
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 1);
        }
    }
    
    public void StartSelectingSkill()
    {
        if (_animators == null || _animators.Length == 0 || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.SelectingSkill;
        foreach (var anim in _animators)
        {
            if (anim != null) anim.SetBool(IsSelectingSkillHash, true);
        }
    }
    
    public void StartAimingBasicAttack()
    {
        if (_animators == null || _animators.Length == 0 || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.AimingBasic;
        foreach (var anim in _animators)
        {
            if (anim != null) anim.SetBool(IsAimingAttackHash, true);
        }
    }
    
    public void StartAimingSkill(int skillIndex)
    {
        if (_animators == null || _animators.Length == 0 || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.AimingSkill;
        foreach (var anim in _animators)
        {
            if (anim != null)
            {
                anim.SetInteger(SkillIndexHash, skillIndex);
                anim.SetBool(IsAimingSkillHash, true);
            }
        }
    }
    
    public void ExecuteAction()
    {
        if (_animators == null || _animators.Length == 0) return;

        if (_currentState == CharacterState.AimingBasic)
        {
            _currentState = CharacterState.Attacking;
            foreach (var anim in _animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger(TriggerActionHash);
                    anim.SetBool(IsAimingAttackHash, false);
                }
            }
        }
        else if (_currentState == CharacterState.AimingSkill)
        {
            _currentState = CharacterState.CastingSkill;
            foreach (var anim in _animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger(TriggerActionHash);
                    anim.SetBool(IsAimingSkillHash, false);
                }
            }
        }
    }
    
    public void CancelAiming()
    {
        if (_animators == null) return;

        foreach (var anim in _animators)
        {
            if (anim != null)
            {
                anim.SetBool(IsAimingAttackHash, false);
                anim.SetBool(IsAimingSkillHash, false);
                anim.SetBool(IsSelectingSkillHash, false);
            }
        }
        
        if (_currentState == CharacterState.AimingBasic || 
            _currentState == CharacterState.AimingSkill ||
            _currentState == CharacterState.SelectingSkill)
        {
            _currentState = CharacterState.Idle;
        }
    }

    public void TakeDamage()
    {
        if (_animators == null) return;

        CancelAiming();

        _currentState = CharacterState.TakingDamage;
        foreach (var anim in _animators)
        {
            if (anim != null) anim.SetTrigger(HitTriggerHash);
        }
    }

    public void SetLowHealth(bool isLow)
    {
        if (_animators == null) return;
        foreach (var anim in _animators)
        {
            if (anim != null) anim.SetBool(LowHealthBoolHash, isLow);
        }
    }
    
    public void OnActionFinished()
    {
        _currentState = CharacterState.Idle;
    }
    
    private bool IsInBlockingState()
    {
        return _currentState == CharacterState.TakingDamage 
            || _currentState == CharacterState.Dead
            || _currentState == CharacterState.Attacking
            || _currentState == CharacterState.CastingSkill;
    }
}
