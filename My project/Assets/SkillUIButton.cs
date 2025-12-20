using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string skillDescription;
    public GameObject skillDescriptionGO;
    private TextMeshProUGUI _descriptionText;

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