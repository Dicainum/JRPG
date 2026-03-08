using UnityEngine;

public class IceFairyAnimationHandler : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private BaseBattleAttack _baseBattleAttack;
    [SerializeField] private float _projectileOffset = 1.25f;
    [SerializeField] private float _maxRayDistance;
    
    public void ApplyAttack()
    {
        _baseBattleAttack.ApplyDamage();
    }

    public void PlayAttackVfx()
    {
        Vector3 origin = transform.position + Vector3.up * _projectileOffset;
        Ray ray = new Ray(origin, transform.forward);
        
        Debug.DrawRay(origin, transform.forward * _maxRayDistance, Color.red, 1f);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxRayDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject projectile = Instantiate(_projectile, origin, Quaternion.identity);
                BattleProjectileController controller = projectile.GetComponent<BattleProjectileController>();
                if (controller != null)
                {
                    controller.MoveToTarget(hit.transform);
                }
                Debug.Log("Hit");
            }
        }
    }
}
