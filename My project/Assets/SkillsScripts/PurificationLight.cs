using System.Linq;
using UnityEngine;

public class PurificationLight : BasicSkill
{
    private void Awake()
    {
        skillName = "Purification Light";
        skillDescription = "Deals medium light damage. If the target has buffs, deals additional pure damage and removes one random buff. Grants one stack of Light.";
        _cooldownTime = 2;
    }

    public override void TryCast()
    {
        if (_inCooldown)
        {
            Debug.Log($"{skillName} is on cooldown.");
            return;
        }
        
        if (_currentUnit == null)
        {
            _currentUnit = GetCurrentUnit();
            if (_currentUnit == null)
            {
                Debug.LogError("Could not find current unit for the skill");
                return;
            }
        }
        
        if (_skillTargetSystem == null)
        {
            _skillTargetSystem = FindFirstObjectByType<SkillTargetSystem>();
            if (_skillTargetSystem == null)
            {
                Debug.LogError("SkillTargetSystem not found in the scene. Cannot cast skill");
                return;
            }
        }

        _skillTargetSystem.SetCurrentSkill(this);
        _skillTargetSystem.TargetSelected += OnTargetSelected;
        _skillTargetSystem.TargetCanceled += OnTargetingCanceled;
        
        _skillTargetSystem.StartTargeting();
    }

    private void OnTargetSelected(TurnUnit target)
    {
        _skillTargetSystem.TargetSelected -= OnTargetSelected;
        _skillTargetSystem.TargetCanceled -= OnTargetingCanceled;

        if (target == null || !target.IsAlive)
        {
            return;
        }
        
        if (_currentUnit == null)
        {
            _currentUnit = GetCurrentUnit();
            if (_currentUnit == null)
            {
                Debug.LogError("Cant find current unit for the skill");
                return;
            }
        }

        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }

        UseAction();
        StartCooldown();

        target.stats.TakeDamage(20);

        var buffs = target.gObject.GetComponents<BasicBuff>().Where(b => !(b is DarkAndLightStacksBuff)).ToList();
        if (buffs.Count > 0)
        {
            target.stats.TakeDamage(10);

            // Remove one random buff
            var buffToRemove = buffs[Random.Range(0, buffs.Count)];
            Destroy(buffToRemove);
            Debug.Log($"Removed {buffToRemove.GetType().Name} from {target.stats.characterName}");
        }

        var stacksBuff = _currentUnit.gObject.GetComponent<DarkAndLightStacksBuff>();
        if (stacksBuff != null)
        {
            stacksBuff.CurrentForm = DarkAndLightStacksBuff.Form.Light;
            stacksBuff.AddStacks(1, _currentUnit);
        }
    }
    
    private void OnTargetingCanceled()
    {
        _skillTargetSystem.TargetSelected -= OnTargetSelected;
        _skillTargetSystem.TargetCanceled -= OnTargetingCanceled;
    }
}
