using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Components")]
    
    [SerializeField] private Animator[] _animators;
    [Header("Settings")]
    [SerializeField] private float _dampTime = 0.1f;
    
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
    private float _stateSafetyTimer;
    [SerializeField] private float _minAttackAnimTime = 0.3f;

    private void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
        if (_animators == null || _animators.Length == 0)
        {
            Debug.LogError("No animators found in children");
        }
        foreach (var anim in _animators)
        {
            if (anim == null) continue;
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 1);
        }
    }

    private void Update()
    {
        if (_currentState == CharacterState.Attacking || _currentState == CharacterState.CastingSkill || _currentState == CharacterState.TakingDamage)
        {
            _stateSafetyTimer += Time.deltaTime;
            if (_stateSafetyTimer > _minAttackAnimTime)
            {
                Debug.LogWarning("Animation State stuck! Force resetting to  idle");
                OnActionFinished();
            }
        }
        else
        {
            _stateSafetyTimer = 0f;
        }
    }
    
    public void StartSelectingSkill()
    {
        if (IsInBlockingState()) return;
        CancelAiming();

        _currentState = CharacterState.SelectingSkill;
        foreach (var anim in _animators) anim.SetBool(IsSelectingSkillHash, true);
    }
    
    public void StartAimingBasicAttack()
    {
        if (IsInBlockingState()) return;
        CancelAiming();

        _currentState = CharacterState.AimingBasic;
        foreach (var anim in _animators) anim.SetBool(IsAimingAttackHash, true);
    }
    
    public void StartAimingSkill(int skillIndex)
    {
        if (IsInBlockingState()) return;
        CancelAiming();

        _currentState = CharacterState.AimingSkill;
        foreach (var anim in _animators)
        {
            anim.SetInteger(SkillIndexHash, skillIndex);
            anim.SetBool(IsAimingSkillHash, true);
        }
    }
    
    public void ExecuteAction()
    {
        if (_currentState == CharacterState.AimingBasic)
        {
            _currentState = CharacterState.Attacking;
            foreach (var anim in _animators) anim.SetTrigger(TriggerActionHash);
        }
        else if (_currentState == CharacterState.AimingSkill)
        {
            _currentState = CharacterState.CastingSkill;
            foreach (var anim in _animators) anim.SetTrigger(TriggerActionHash);
        }
        _stateSafetyTimer = 0f;
    }
    
    public void CancelAiming()
    {
        foreach (var anim in _animators)
        {
            anim.SetBool(IsAimingAttackHash, false);
            anim.SetBool(IsAimingSkillHash, false);
            anim.SetBool(IsSelectingSkillHash, false);
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

        CancelAiming();

        _currentState = CharacterState.TakingDamage;
        foreach (var anim in _animators) anim.SetTrigger(HitTriggerHash);
    }

    public void SetLowHealth(bool isLow)
    {
        foreach (var anim in _animators) anim.SetBool(LowHealthBoolHash, isLow);
    }
    
    public void OnActionFinished()
    {
        _currentState = CharacterState.Idle;
        _stateSafetyTimer = 0f;

        foreach (var anim in _animators)
        {
            anim.SetBool(IsAimingAttackHash, false);
            anim.SetBool(IsAimingSkillHash, false);
            anim.SetBool(IsSelectingSkillHash, false);
        }
    }

    public bool IsInBlockingState()
    {
        return _currentState == CharacterState.TakingDamage 
            || _currentState == CharacterState.Dead
            || _currentState == CharacterState.Attacking
            || _currentState == CharacterState.CastingSkill;
    }
}
