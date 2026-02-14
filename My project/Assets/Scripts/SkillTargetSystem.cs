using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTargetSystem : MonoBehaviour
{
    public static SkillTargetSystem SkillTarget{ get; private set; }
    [SerializeField] private OrderController orderController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference confirmInput;
    [SerializeField] private InputActionReference cancelInput;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject aim;

    [SerializeField] private CameraBattleController cameraController;
    
    private bool _isTargetingEnemy = false;
    private bool _isTargetingAlly = false;
    
    private Vector3 _originalCamPos;
    private Quaternion _originalCamRot;

    private List<TurnUnit> _enemyTargets = new();
    private List<TurnUnit> _allyTargets = new();

    private TurnUnit _target;
    private TurnUnit _lastTarget;
    private int _currentTargetIndex = 0;
    private bool _targeting = false;
    private bool _wasLeftHeld = false;
    private bool _wasRightHeld = false;
    public System.Action<TurnUnit> TargetSelected;
    public System.Action TargetCanceled;
    public System.Action TargetStarted;
    private BasicSkill _currentSkill;

    private void OnEnable()
    {
        orderController.OnOrderUpdated += UpdateCharacters;
        moveInput.action.Enable();
        confirmInput.action.Enable();
        confirmInput.action.performed += OnConfirmPressed;
        if (cancelInput != null)
        {
            cancelInput.action.Enable();
            cancelInput.action.performed += OnCancelPressed;
        }
    }

    private void OnDisable()
    {
        orderController.OnOrderUpdated -= UpdateCharacters;
        moveInput.action.Disable();
        confirmInput.action.performed -= OnConfirmPressed;
        confirmInput.action.Disable();
        if (cancelInput != null)
        {
            cancelInput.action.performed -= OnCancelPressed;
            cancelInput.action.Disable();
        }
    }
    
    private void Awake()
    {
        // Assign this instance as the singleton
        if (SkillTarget != null && SkillTarget != this)
        {
            Destroy(gameObject);
        }
        else
        {
            SkillTarget = this;
        }
    }
    private void FixedUpdate()
    {
        if(_isTargetingAlly || _isTargetingEnemy)
        {
            _targeting = true;
        }
        else
        {
            _targeting = false;
        }
        if (!_targeting)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelTargeting();
            return;
        }

        if (_enemyTargets.Count == 0 && _allyTargets.Count == 0)
            return;

        //TODO: Optimize this 
        if (_isTargetingEnemy)
        {
            HandleTargetSelection(_enemyTargets);
        }
        else if (_isTargetingAlly)
        {
            HandleTargetSelection(_allyTargets);
        }
    }

    private void HandleTargetSelection(List<TurnUnit> targets)
    {
        Vector2 move = moveInput.action.ReadValue<Vector2>();

        bool rightHeld = move.x > 0.5f;
        bool leftHeld = move.x < -0.5f;

        if (rightHeld && !_wasRightHeld)
        {
            _currentTargetIndex = (_currentTargetIndex + 1) % targets.Count;
            _target = targets[_currentTargetIndex];
        }
        else if (leftHeld && !_wasLeftHeld)
        {
            _currentTargetIndex--;
            if (_currentTargetIndex < 0)
                _currentTargetIndex = targets.Count - 1;
            _target = targets[_currentTargetIndex];
        }

        _wasRightHeld = rightHeld;
        _wasLeftHeld = leftHeld;

        if (_target != null && _lastTarget != _target)
        {
            //Debug.Log("Moving camera");
            _lastTarget = _target;
            cameraController.BattleCameraLookAtTarget(_target.gObject);
        }

        if (confirmInput.action.WasPressedThisFrame())
        {
        }
    }

    private void UpdateCharacters(List<TurnUnit> turnQueue)
    {
        _enemyTargets.Clear();
        _allyTargets.Clear();

        foreach (var unit in turnQueue)
        {
            if (unit.stats.isEnemy)
                _enemyTargets.Add(unit);
            else
                if (!_allyTargets.Contains(unit))
            {
                _allyTargets.Add(unit);
            }
        }

        _enemyTargets.Sort((a, b) => a.stats.index.CompareTo(b.stats.index));
        _allyTargets.Sort((a, b) => a.stats.index.CompareTo(b.stats.index));
    }
    public void SetCurrentSkill(BasicSkill skill)
    {
        _currentSkill = skill;
    }
    private void OnConfirmPressed(InputAction.CallbackContext ctx)
    {
        if (!_isTargetingEnemy && !_isTargetingAlly) return;

        if (OrderController.Order.currentUnit.gObject != null)
        {
            var animController = OrderController.Order.currentUnit.gObject.GetComponent<CharacterAnimationController>();
            if (animController != null)
            {
                animController.ExecuteAction();
            }
        }
        float delay = 0f;
        Debug.Log(_currentSkill.delayBeforeCamMove);
        if (_currentSkill != null)
        {
            delay = _currentSkill.delayBeforeCamMove;
        }
        StartCoroutine(ConfirmTargetRoutine(delay, _target));
    }

    private IEnumerator ConfirmTargetRoutine(float delay, TurnUnit selectedTarget)
    {
        _isTargetingEnemy = false;
        _isTargetingAlly = false;
        aim.SetActive(false);
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        TargetSelected?.Invoke(selectedTarget);
        StopTargeting();
    }

    private void OnCancelPressed(InputAction.CallbackContext ctx)
    {
        if (_targeting)
        {
            CancelTargeting();
        }
    }

    public void CancelTargeting()
    {
        if (OrderController.Order.currentUnit.gObject != null)
        {
            var animController = OrderController.Order.currentUnit.gObject.GetComponent<CharacterAnimationController>();
            if (animController != null)
            {
                animController.CancelAiming();
            }
        }
        
        TargetCanceled?.Invoke();
        
        if (cameraController != null)
        {
            cameraController.ResetCamera(_originalCamPos, _originalCamRot);
        }

        StopTargeting();
        _currentSkill = null;
    }

    public void StartTargeting()
    {
        TargetStarted?.Invoke();
        SaveCameraTransform();
        _isTargetingEnemy = true;
        _isTargetingAlly = false;

        if (_enemyTargets.Count > 0)
        {
            _currentTargetIndex = 0;
            _target = _enemyTargets[_currentTargetIndex];
            cameraController.BattleCameraLookAtTarget(_target.gObject);
            aim.SetActive(true);
        }
    }

    public void StartTargetingAlly()
    {
        TargetStarted?.Invoke();
        SaveCameraTransform();
        _isTargetingAlly = true;
        _isTargetingEnemy = false;

        if (_allyTargets.Count > 0)
        {
            _currentTargetIndex = 0;
            _target = _allyTargets[_currentTargetIndex];
            cameraController.BattleCameraLookAtTarget(_target.gObject);
            aim.SetActive(true);
        }
    }

    private void SaveCameraTransform()
    {
        if (cam != null)
        {
            _originalCamPos = cam.transform.position;
            _originalCamRot = cam.transform.rotation;
        }
    }

    public void StopTargeting()
    {
        _isTargetingEnemy = false;
        _isTargetingAlly = false;
        aim.SetActive(false);
        _currentSkill = null;
    }
}