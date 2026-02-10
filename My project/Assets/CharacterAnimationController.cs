using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    
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
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 1);
    }
    
    public void StartSelectingSkill()
    {
        if (_animator == null || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.SelectingSkill;
        _animator.SetBool(IsSelectingSkillHash, true);
    }
    
    public void StartAimingBasicAttack()
    {
        if (_animator == null || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.AimingBasic;
        _animator.SetBool(IsAimingAttackHash, true);
    }
    
    public void StartAimingSkill(int skillIndex)
    {
        if (_animator == null || IsInBlockingState()) return;

        CancelAiming();

        _currentState = CharacterState.AimingSkill;
        _animator.SetInteger(SkillIndexHash, skillIndex);
        _animator.SetBool(IsAimingSkillHash, true);
    }
    
    public void ExecuteAction()
    {
        if (_animator == null) return;

        if (_currentState == CharacterState.AimingBasic)
        {
            _currentState = CharacterState.Attacking;
            _animator.SetTrigger(TriggerActionHash);
            _animator.SetBool(IsAimingAttackHash, false);
        }
        else if (_currentState == CharacterState.AimingSkill)
        {
            _currentState = CharacterState.CastingSkill;
            _animator.SetTrigger(TriggerActionHash);
            _animator.SetBool(IsAimingSkillHash, false);
        }
    }
    
    public void CancelAiming()
    {
        if (_animator == null) return;

        _animator.SetBool(IsAimingAttackHash, false);
        _animator.SetBool(IsAimingSkillHash, false);
        _animator.SetBool(IsSelectingSkillHash, false);
        
        if (_currentState == CharacterState.AimingBasic || 
            _currentState == CharacterState.AimingSkill ||
            _currentState == CharacterState.SelectingSkill)
        {
            _currentState = CharacterState.Idle;
        }
    }

    public void TakeDamage()
    {
        if (_animator == null) return;

        CancelAiming();

        _currentState = CharacterState.TakingDamage;
        _animator.SetTrigger(HitTriggerHash);
    }

    public void SetLowHealth(bool isLow)
    {
        if (_animator == null) return;
        _animator.SetBool(LowHealthBoolHash, isLow);
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
