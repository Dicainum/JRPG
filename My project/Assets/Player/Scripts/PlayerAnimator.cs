using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator[] _animators;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem[] _attackVFXs;

    private int _speedHash;

    private void Awake()
    {
        _speedHash = Animator.StringToHash("Speed");
    }

    public void UpdateMovementState(float targetSpeed)
    {
        if (_animators == null) return;

        foreach (var anim in _animators)
        {
            if (anim != null && anim.gameObject.activeSelf)
            {
                anim.SetFloat(_speedHash, targetSpeed, 0.1f, Time.deltaTime);
            }
        }
    }
    public void SetTrigger(int triggerHash)
    {
        if (_animators == null) return;

        for (int i = 0; i < _animators.Length; i++)
        {
            var anim = _animators[i];
            if (anim != null && anim.gameObject.activeSelf)
            {
                anim.SetTrigger(triggerHash);
            }
        }
    }

    public void PlayActiveVFX()
    {
        if (_animators == null || _attackVFXs == null) return;

        for (int i = 0; i < _animators.Length; i++)
        {
            var anim = _animators[i];
            if (anim != null && anim.gameObject.activeSelf)
            {
                if (i < _attackVFXs.Length && _attackVFXs[i] != null)
                {
                    _attackVFXs[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _attackVFXs[i].Play(true);
                }
            }
        }
    }
}