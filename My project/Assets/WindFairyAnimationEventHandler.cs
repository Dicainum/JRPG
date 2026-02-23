using UnityEngine;

public class WindFairyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private WindForceSkill _speedBuff;
    [SerializeField] private HailOfStonesSkill _hailOfStones;

    public void PlaySpeedBuffVfx()
    {
        _speedBuff.SpawnVFX();
    }
    
    public void PlayStoneVfx()
    {
        _hailOfStones.SpawnVFX();
    }
}
