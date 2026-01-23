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

    private void Awake()
    {
        if (skillTargetSystem == null)
        {
            skillTargetSystem = FindFirstObjectByType<SkillTargetSystem>();
        }
    }

    private void OnEnable()
    {
        orderController.OnTurnStarted += ShowCharacterCanvas;
        orderController.OnActionPerformed += PastActionUIUpdate;
        orderController.OnTurnEnded += EndAction;

        if (goBackInput != null && goBackInput.action != null)
        {
            goBackInput.action.Enable();
            goBackInput.action.performed += OnGoBackAction;
        }

        if (skillTargetSystem != null)
        {
            skillTargetSystem.TargetCanceled += OnTargetCanceled;
            skillTargetSystem.TargetStarted += OnTargetStarted;
        }
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= ShowCharacterCanvas;
        orderController.OnActionPerformed -= PastActionUIUpdate;
        orderController.OnTurnEnded -= EndAction;

        if (goBackInput != null && goBackInput.action != null)
        {
            goBackInput.action.performed -= OnGoBackAction;
            goBackInput.action.Disable();
        }

        if (skillTargetSystem != null)
        {
            skillTargetSystem.TargetCanceled -= OnTargetCanceled;
            skillTargetSystem.TargetStarted -= OnTargetStarted;
        }
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
                if (skillTargetSystem != null)
                {
                    skillTargetSystem.CancelTargeting();
                }
                break;

            case BattleUIState.SkillSelection:
                CloseSkillPage();
                break;

            case BattleUIState.ActionSelection:
                Debug.Log("already in main!");
                break;
        }
    }

    private void OnTargetStarted()
    {
        if (activatedCanvas != null)
        {
            activatedCanvas.SetActive(false);
        }
    }

    private void OnTargetCanceled()
    {
        if (cameraBattleController != null && currentActiveUnit != null)
        {
            cameraBattleController.ChangeToUnitCameraPos(currentActiveUnit);
        }

        CloseSkillPage();
    }

    private void CloseSkillPage()
    {
        DisableUnitPages(currentActiveUnit);

        if (currentActiveUnit != null)
        {
            int index = currentActiveUnit.stats.index;

            if (descriptions != null && index >= 0 && index < descriptions.Length)
            {
                if (descriptions[index] != null)
                {
                    descriptions[index].SetActive(false);
                }
            }
            EnableCanvas(index);
            
            if (firstPages != null && index >= 0 && index < firstPages.Length)
            {
                if (firstPages[index] != null)
                {
                    firstPages[index].SetActive(true);
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