using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string skillDescription;
    public GameObject skillDescriptionGO;

    private TextMeshProUGUI _descriptionText;

    private void OnEnable()
    {
        if (skillDescriptionGO != null)
        {
            skillDescriptionGO.SetActive(false);

            _descriptionText = skillDescriptionGO.GetComponentInChildren<TextMeshProUGUI>();

            if (_descriptionText != null)
                _descriptionText.text = skillDescription;
        }
        else
        {
            Debug.LogWarning($"{name}: skillDescriptionGO is not assigned!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillDescriptionGO != null)
            skillDescriptionGO.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionGO != null)
            skillDescriptionGO.SetActive(false);
    }
}