using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject _attackHitbox;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private float _attackCooldown = 1f;

    private static readonly int AttackTriggerHash = Animator.StringToHash("OWAttacking");

    private float _nextAttackTime = 0f;

    private void Awake()
    {
        if (_playerAnimator == null) _playerAnimator = GetComponent<PlayerAnimator>();

        if (_attackHitbox != null) _attackHitbox.SetActive(false);
    }

    public void HandleAttack()
    {
        if (Time.time < _nextAttackTime) return;

        _nextAttackTime = Time.time + _attackCooldown;

        _playerAnimator?.SetTrigger(AttackTriggerHash);
    }
    public void EnableAttackHitbox()
    {
        if (_attackHitbox != null) _attackHitbox.SetActive(true);

        _playerAnimator?.PlayActiveVFX();
    }

    public void DisableAttackHitbox()
    {
        if (_attackHitbox != null) _attackHitbox.SetActive(false);
    }
}