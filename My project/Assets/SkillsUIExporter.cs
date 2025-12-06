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

            TextMeshProUGUI tmp = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            var skillUIButton = buttonObj.GetComponent<SkillUIButton>();
            skillUIButton.skillDescription = skill.skillDescription;
            skillUIButton.skillDescriptionGO = _battleSkillsCanvasController.descriptions[_stats.index];

            if (tmp != null)
                tmp.text = skill.skillName;

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => skill.TryCast());
            }
            else
            {
                Debug.LogWarning($"Button component not found on {buttonObj.name}");
            }
        }
    }
}