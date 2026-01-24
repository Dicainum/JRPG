using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUIExporter : MonoBehaviour
{
    private BasicSkill[] _skills;
    private BattleSkillsCanvasController _battleSkillsCanvasController;
    private Stats _stats;

    [SerializeField] private GameObject _skillButtonPrefab;

    private struct SkillButtonPair
    {
        public BasicSkill Skill;
        public Button Button;
    }

    private List<SkillButtonPair> _skillButtons = new List<SkillButtonPair>();

    private void Start()
    {
        _skills = GetComponents<BasicSkill>();
        _battleSkillsCanvasController = FindFirstObjectByType<BattleSkillsCanvasController>();
        _stats = GetComponent<Stats>();
        
        if (_battleSkillsCanvasController == null || _battleSkillsCanvasController.skillPages == null || _stats == null)
        {
            Debug.LogWarning("SkillsUIExporter: Missing dependencies (BattleSkillsCanvasController or Stats).");
            return;
        }

        if (_stats.index < 0 || _stats.index >= _battleSkillsCanvasController.skillPages.Length)
        {
            Debug.LogError($"SkillsUIExporter: Stats index {_stats.index} is out of range for skillPages.");
            return;
        }

        Transform parentPage = _battleSkillsCanvasController.skillPages[_stats.index].transform;

        foreach (BasicSkill skill in _skills)
        {
            GameObject buttonObj = Instantiate(_skillButtonPrefab, parentPage);
            
            TextMeshProUGUI buttonLabel = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonLabel != null) buttonLabel.text = skill.skillName;

            var skillUIButton = buttonObj.GetComponent<SkillUIButton>();
            if (skillUIButton != null && _battleSkillsCanvasController.descriptions != null && _stats.index < _battleSkillsCanvasController.descriptions.Length)
            {
                skillUIButton.Init(_battleSkillsCanvasController.descriptions[_stats.index], skill.skillDescription);
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => skill.TryCast());
                
                _skillButtons.Add(new SkillButtonPair { Skill = skill, Button = button });
            }
        }
    }

    private void Update()
    {
        foreach (var pair in _skillButtons)
        {
            if (pair.Skill != null && pair.Button != null)
            {
                pair.Button.interactable = !pair.Skill.IsOnCooldown;
            }
        }
    }
}