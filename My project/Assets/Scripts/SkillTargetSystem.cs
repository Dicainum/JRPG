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
    private Transform _originalCamTransform;
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
            //Debug.Log("targeting enemy");
            Vector2 move = moveInput.action.ReadValue<Vector2>();

            bool rightHeld = move.x > 0.5f;
            bool leftHeld = move.x < -0.5f;

            if (rightHeld && !_wasRightHeld)
            {
                _currentTargetIndex = (_currentTargetIndex + 1) % _enemyTargets.Count;
                _target = _enemyTargets[_currentTargetIndex];
            }
            else if (leftHeld && !_wasLeftHeld)
            {
                _currentTargetIndex--;
                if (_currentTargetIndex < 0)
                    _currentTargetIndex = _enemyTargets.Count - 1;
                _target = _enemyTargets[_currentTargetIndex];
            }

            _wasRightHeld = rightHeld;
            _wasLeftHeld = leftHeld;

            if (_target != null && _lastTarget != _target)
            {
                Debug.Log("Moving camera");
                _lastTarget = _target;
                cameraController.BattleCameraLookAtTarget(_target.gObject);
            }

            if (confirmInput.action.WasPressedThisFrame())
            {
                TargetSelected?.Invoke(_target);
                StopTargeting();
            }
        }
        if (_isTargetingAlly)
        {
            //Debug.Log("targ al");
            //Debug.Log(_allyTargets);
            Vector2 move = moveInput.action.ReadValue<Vector2>();

            bool rightHeld = move.x > 0.5f;
            bool leftHeld = move.x < -0.5f;

            if (rightHeld && !_wasRightHeld)
            {
                _currentTargetIndex = (_currentTargetIndex + 1) % _allyTargets.Count;
                _target = _allyTargets[_currentTargetIndex];
            }
            else if (leftHeld && !_wasLeftHeld)
            {
                _currentTargetIndex--;
                if (_currentTargetIndex < 0)
                    _currentTargetIndex = _allyTargets.Count - 1;
                _target = _allyTargets[_currentTargetIndex];
            }

            _wasRightHeld = rightHeld;
            _wasLeftHeld = leftHeld;

            if (_target != null && _lastTarget != _target)
            {
                Debug.Log("Moving camera");
                _lastTarget = _target;
                cameraController.BattleCameraLookAtTarget(_target.gObject);
            }

            if (confirmInput.action.WasPressedThisFrame())
            {
                TargetSelected?.Invoke(_target);
                StopTargeting();
            }
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
    private void OnConfirmPressed(InputAction.CallbackContext ctx)
    {
        if (!_isTargetingEnemy && !_isTargetingAlly) return;

        TargetSelected?.Invoke(_target);
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
        TargetCanceled?.Invoke();
        StopTargeting();
    }

    public void StartTargeting()
    {
        _originalCamTransform = cam.transform; 
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
        _originalCamTransform = cam.transform;
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

    public void StopTargeting()
    {
        _isTargetingEnemy = false;
        _isTargetingAlly = false;
        aim.SetActive(false);
    }

    public void ReturnCameraRotation()
    {
        cameraController.BattleCameraChangeRotation(_originalCamTransform);
    }
}