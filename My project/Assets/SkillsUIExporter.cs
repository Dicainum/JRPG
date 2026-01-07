using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUIExporter : MonoBehaviour
{
    private BasicSkill[] _skills;
    private BattleSkillsCanvasController _battleSkillsCanvasController;
    private Stats _stats;

    [SerializeField] private GameObject _skillButtonPrefab;

    private void Start()
    {
        _skills = GetComponents<BasicSkill>();
        _battleSkillsCanvasController = FindFirstObjectByType<BattleSkillsCanvasController>();
        _stats = GetComponent<Stats>();
        Debug.Log(_skills.Length);

        Transform parentPage = _battleSkillsCanvasController.skillPages[_stats.index].transform;

        foreach (BasicSkill skill in _skills)
        {
            GameObject buttonObj = Instantiate(_skillButtonPrefab, parentPage);
            
            TextMeshProUGUI buttonLabel = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonLabel != null) buttonLabel.text = skill.skillName;

            var skillUIButton = buttonObj.GetComponent<SkillUIButton>();
            if (skillUIButton != null)
            {
                skillUIButton.Init(_battleSkillsCanvasController.descriptions[_stats.index], skill.skillDescription);
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => skill.TryCast());
            }
        }
    }
}