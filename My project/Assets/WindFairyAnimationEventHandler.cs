using UnityEngine;
using DG.Tweening;

public class WindFairyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private WindForceSkill _speedBuff;
    [SerializeField] private HailOfStonesSkill _hailOfStones;
    [SerializeField] private BaseBattleAttack _baseBattleAttack;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _projectileOffset = 1.25f;
    [SerializeField] private float _maxRayDistance = 50f;
    private Quaternion _originalRotation;

    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    public void PlaySpeedBuffVfx()
    {
        _speedBuff.SpawnVFX();
    }
    
    public void PlayStoneVfx()
    {
        _hailOfStones.SpawnVFX();
    }
    
    public void ApplyAttack()
    {
        _baseBattleAttack.ApplyDamage();
    }

    public void PlayAttackVfx()
    {
        Vector3 origin = transform.position + Vector3.up * _projectileOffset;
        Ray ray = new Ray(origin, transform.forward);
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
            }
        }
    }
    
    public void ResetRotation()
    {
        transform.DORotateQuaternion(_originalRotation, 0.5f);
    }
}
