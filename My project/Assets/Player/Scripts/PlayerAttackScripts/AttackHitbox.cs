using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy attacked");
            other.GetComponent<EnemyAI>()?.OnAttacked();
        }
    }
}
