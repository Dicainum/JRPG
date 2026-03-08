using UnityEngine;
using DG.Tweening;

public class IceFairyAnimationHandler : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private BaseBattleAttack _baseBattleAttack;
    [SerializeField] private float _projectileOffset = 1.25f;
    [SerializeField] private float _maxRayDistance;
    
    private Quaternion _originalRotation;

    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    public void ApplyAttack()
    {
        _baseBattleAttack.ApplyDamage();
    }

    public void PlayAttackVfx()
    {
        Vector3 origin = transform.position + Vector3.up * _projectileOffset;
        Vector3 direction = Quaternion.Euler(0, -90, 0) * transform.forward;
        Ray ray = new Ray(origin, direction);
        
        Debug.DrawRay(origin, direction * _maxRayDistance, Color.red, 1f);

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
    
    public void ResetRotation()
    {
        transform.DORotateQuaternion(_originalRotation, 0.5f);
    }
}
