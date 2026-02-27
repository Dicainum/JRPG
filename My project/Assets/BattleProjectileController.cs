using UnityEngine;
using DG.Tweening;

public class BattleProjectileController : MonoBehaviour
{
    [SerializeField] private float time = 0.5f;
    [SerializeField] private float delayBeforeDestroyProjectile = 0.2f;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float delayBeforeDestroyGO = 2f;

    public void MoveToTarget(Transform target)
    {
        transform.DOMove(target.position, time)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOKill();
                Destroy(_projectile, delayBeforeDestroyProjectile);
                Destroy(gameObject, delayBeforeDestroyGO);
            });
    }
}
