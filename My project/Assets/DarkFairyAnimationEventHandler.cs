using UnityEngine;
using DG.Tweening;

public class DarkFairyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private ArrowOfDarknessSkill _arrowOfDarknessSkill;
    [SerializeField] private GameObject _bow;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private BaseBattleAttack _baseBattleAttack;
    [SerializeField] private float _projectileOffset = 1.25f;
    [SerializeField] private float _maxRayDistance = 50f;
    [SerializeField] private GameObject _blackHole;
    private Animator anim;
    private Quaternion _originalRotation;

    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 1);
    }
    public void SpawnBow()
    {
        _bow.SetActive(true);
    }

    public void DespawnBow()
    {
        _bow.SetActive(false);
    }
    public void ApplyAttack()
    {
        _baseBattleAttack.ApplyDamage();
    }

    public void ActivateBlackHole()
    {
        _blackHole.gameObject.SetActive(true);
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
    
    public void ResetRotation()
    {
        transform.DORotateQuaternion(_originalRotation, 0.5f);
    }
}
