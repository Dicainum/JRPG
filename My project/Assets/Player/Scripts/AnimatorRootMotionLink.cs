using UnityEngine;

public class AnimatorRootMotionLink : MonoBehaviour
{
    private PlayerControlller _rootController;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rootController = GetComponentInParent<PlayerControlller>();
    }

    private void OnAnimatorMove()
    {
        if (_rootController != null && _animator != null)
        {
            _rootController.OnRootMotionReceived(_animator.deltaPosition, _animator.deltaRotation);
        }
    }
}
