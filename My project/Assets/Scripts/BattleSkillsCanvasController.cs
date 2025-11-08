using UnityEngine;

public class BattleSkillsCanvasController : MonoBehaviour
{
    [SerializeField] private OrderController orderController;
    [SerializeField] private GameObject[] battleSkills;
    [SerializeField] private GameObject activatedCanvas;
    [SerializeField] private GameObject[] firstPages;

    private void OnEnable()
    {
        orderController.OnTurnStarted += ShowCharacterCanvas;
        orderController.OnActionPerformed += PastActionUIUpdate;
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= ShowCharacterCanvas;
        orderController.OnActionPerformed -= PastActionUIUpdate;
    }
    
    private void ShowCharacterCanvas(TurnUnit currentUnit)
    {
        if (!currentUnit.stats.isEnemy)
        {
            DisableAllCanvas();
            EnableCanvas(currentUnit.stats.index);
        }
        else
        {
            DisableAllCanvas();
        }
    }

    private void EnableCanvas(int index)
    {
        battleSkills[index].SetActive(true);
        activatedCanvas = battleSkills[index];
    }

    private void DisableCanvas()
    {
        activatedCanvas.SetActive(false);
        activatedCanvas = null;
    }

    private void DisableAllCanvas()
    {
        foreach (var bs in battleSkills)
        {
            bs.SetActive(false);
        }
    }

    private void PastActionUIUpdate(TurnUnit unit)
    {
        if (unit.stats.actions > 0)
        {
            EnableCanvas(unit.stats.index);
        }
    }
    
}
