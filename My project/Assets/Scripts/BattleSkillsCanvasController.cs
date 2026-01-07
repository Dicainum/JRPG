using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleSkillsCanvasController : MonoBehaviour
{
    [SerializeField] private OrderController orderController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject[] battleSkills;
    [SerializeField] private GameObject activatedCanvas;
    [SerializeField] private GameObject[] firstPages;
    public GameObject[] skillPages;
    public GameObject[] descriptions;

    [SerializeField] private InputActionReference goBackInput;
    private TurnUnit currentActiveUnit;

    [SerializeField] private SkillTargetSystem skillTargetSystem;
    [SerializeField] private CameraBattleController cameraBattleController;

    public enum BattleUIState
    {
        None,
        ActionSelection,
        SkillSelection,
        TargetSelection
    }

    [SerializeField] private BattleUIState currentState = BattleUIState.None;

    private void OnEnable()
    {
        orderController.OnTurnStarted += ShowCharacterCanvas;
        orderController.OnActionPerformed += PastActionUIUpdate;
        orderController.OnTurnEnded += EndAction;

        goBackInput.action.Enable();
        goBackInput.action.performed += OnGoBackAction;
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= ShowCharacterCanvas;
        orderController.OnActionPerformed -= PastActionUIUpdate;
        orderController.OnTurnEnded -= EndAction;

        goBackInput.action.performed -= OnGoBackAction;
        goBackInput.action.Disable();
    }

    private void ShowCharacterCanvas(TurnUnit currentUnit)
    {
        currentActiveUnit = currentUnit;

        if (!currentUnit.stats.isEnemy)
        {
            DisableAllCanvas();
            EnableCanvas(currentUnit.stats.index);

            currentState = BattleUIState.ActionSelection;
        }
        else
        {
            DisableAllCanvas();
            currentState = BattleUIState.None;
        }
    }

    public void OnGoBackAction(InputAction.CallbackContext ctx)
    {
        switch (currentState)
        {
            case BattleUIState.TargetSelection:
                CancelTargeting();
                break;

            case BattleUIState.SkillSelection:
                CloseSkillPage();
                break;

            case BattleUIState.ActionSelection:
                Debug.Log("already in main!");
                break;
        }
    }

    private void CancelTargeting()
    {
        skillTargetSystem.StopTargeting();

        if (cameraBattleController != null && currentActiveUnit != null)
        {
            cameraBattleController.ChangeToUnitCameraPos(currentActiveUnit);
        }

        currentState = BattleUIState.SkillSelection;
        Debug.Log("back to skill pages");
    }

    private void CloseSkillPage()
    {
        DisableUnitPages(currentActiveUnit);

        if (currentActiveUnit != null && descriptions != null)
        {
            int index = currentActiveUnit.stats.index;

            if (index >= 0 && index < descriptions.Length)
            {
                if (descriptions[index] != null)
                {
                    descriptions[index].SetActive(false);
                }
            }
        }

        currentState = BattleUIState.ActionSelection;
        Debug.Log("back to actions");
    }

    public void EnterSkillSelection()
    {
        currentState = BattleUIState.SkillSelection;
    }

    public void EnterTargeting()
    {
        currentState = BattleUIState.TargetSelection;
    }

    public void EndAction(TurnUnit unit)
    {
        DisableUnitPages(unit);

        if (unit == currentActiveUnit)
        {
            currentActiveUnit = null;
            currentState = BattleUIState.None;
        }

        DisableAllCanvas();
    }

    private void DisableUnitPages(TurnUnit unit)
    {
        if (unit == null || unit.stats == null) return;

        int index = unit.stats.index;

        if (index >= 0 && index < skillPages.Length)
        {
            if (skillPages[index] != null)
                skillPages[index].SetActive(false);
        }
    }

    private void EnableCanvas(int index)
    {
        if (index >= 0 && index < battleSkills.Length)
        {
            battleSkills[index].SetActive(true);
            activatedCanvas = battleSkills[index];
        }
    }

    private void DisableAllCanvas()
    {
        foreach (var bs in battleSkills)
        {
            if (bs != null) bs.SetActive(false);
        }
    }

    private void PastActionUIUpdate(TurnUnit unit)
    {
        if (unit.stats.actions > 0)
        {
            EnableCanvas(unit.stats.index);
            currentState = BattleUIState.ActionSelection;
        }
    }
}