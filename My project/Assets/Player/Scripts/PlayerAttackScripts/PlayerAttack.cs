using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private PlayerAnimator animator;

    private static readonly int AttackTrigger = Animator.StringToHash("OWAttacking");
    private void Awake()
    {
        if (animator == null) animator = GetComponent<PlayerAnimator>();
    }

    public void HandleAttack()
    {
        Debug.Log($"{name} is attacking");

        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
        }

        StartCoroutine(ActivateHitbox());
    }

    private System.Collections.IEnumerator ActivateHitbox()
    {
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        attackHitbox.SetActive(false);
    }
}