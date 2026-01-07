using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string skillDescription;
    public GameObject skillDescriptionGO;
    private TextMeshProUGUI _descriptionText;

    private BattleSkillsCanvasController _battleController;

    public void Init(GameObject descriptionPanel, string description)
    {
        skillDescriptionGO = descriptionPanel;
        skillDescription = description;

        if (skillDescriptionGO != null)
        {
            _descriptionText = skillDescriptionGO.GetComponentInChildren<TextMeshProUGUI>(true);
            skillDescriptionGO.SetActive(false);
        }
    }

    private void Start()
    {
        _battleController = GetComponentInParent<BattleSkillsCanvasController>();

        Button btn = GetComponent<Button>();
        if (btn != null && _battleController != null)
        {
            btn.onClick.AddListener(OnSkillClicked);
        }
        else
        {
            if (_battleController == null) Debug.LogError("SkillButton не нашла BattleSkillsCanvasController в родителях!");
        }
    }

    private void OnSkillClicked()
    {
        _battleController.EnterTargeting();
        Debug.Log("Button clicked: Enter Targeting");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillDescriptionGO != null && _descriptionText != null)
        {
            _descriptionText.text = skillDescription;
            skillDescriptionGO.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionGO != null)
            skillDescriptionGO.SetActive(false);
    }
}