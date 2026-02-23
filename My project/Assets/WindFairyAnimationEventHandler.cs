using UnityEngine;

public class WindFairyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private WindForceSkill _speedBuff;

    public void PlaySpeedBuffVfx()
    {
        _speedBuff.SpawnVFX();
    }
}
