using UnityEngine;

public class CharacterStats : Stats
{
    [SerializeField] private CharacterStatsSO _stats;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetStatsFromSO();
    }
    
    private void GetStatsFromSO()
    {
        if (_stats != null)
        {
            if (_stats.health > 0)
            {
                health = _stats.health;
            }

            if (_stats.speed >= 0)
            {
                speed = _stats.speed;
            }

            if (_stats.damage > 0)
            {
                damage = _stats.damage;
            }

            if (_stats.characterName != "")
            {
                characterName = _stats.characterName;
            }
        
            RecalculateStats();

            if (_stats.maxHealth > 0)
            {
                maxHealth = _stats.maxHealth;
            }

            if (_stats.baseSpeed > 0)
            {
                baseSpeed = _stats.baseSpeed;
            }

            if (_stats.baseDamage > 0)
            {
                baseDamage = _stats.baseDamage;
            }
            
            if (_stats.characterIcon != null)
            {
                characterIcon = _stats.characterIcon;
            }
            
            UpdateUI();
        }
    }
}
