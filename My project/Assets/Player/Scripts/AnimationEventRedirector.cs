using UnityEngine;

public class AnimationEventRedirector : MonoBehaviour
{
    private PlayerAttack _rootAttackScript;

    private void Awake()
    {
        _rootAttackScript = GetComponentInParent<PlayerAttack>();
    }

    public void RedirectEnableHitbox()
    {
        _rootAttackScript?.EnableAttackHitbox();
    }

    public void RedirectDisableHitbox()
    {
        _rootAttackScript?.DisableAttackHitbox();
    }

    public void RedirectLockRotation()
    {
        _rootAttackScript?.LockRotation();
    }

    public void RedirectUnlockRotation()
    {
        _rootAttackScript?.UnlockRotation();
    }
}