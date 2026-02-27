using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Animator animator;

    private static readonly int AttackTrigger = Animator.StringToHash("OWAttack);

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
        yield return new WaitForSeconds(0.2f);
        attackHitbox.SetActive(false);
    }
}