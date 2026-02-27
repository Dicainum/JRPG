using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator[] animator;

    private int speedHash;

    private void Awake()
    {
        speedHash = Animator.StringToHash("Speed");
    }

    public void UpdateMovementState(float targetSpeed)
    {
        if (animator != null)
        {
            foreach (var anim in animator)
            {
                anim.SetFloat(speedHash, targetSpeed, 0.1f, Time.deltaTime);
            }
        }
    }
    internal void SetTrigger(int triggerHash)
    {
        if (animator != null)
        {
            foreach (var anim in animator)
            {
                if (anim != null)
                {
                    anim.SetTrigger(triggerHash);
                }
            }
        }
    }
}