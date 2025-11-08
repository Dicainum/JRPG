using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTargetSystem : MonoBehaviour
{
    public static SkillTargetSystem SkillTarget{ get; private set; }
    [SerializeField] private OrderController orderController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference confirmInput;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject aim;

    private bool _isTargeting = false;
    private Transform _originalCamTransform;
    private List<TurnUnit> _enemyTargets = new();
    private List<TurnUnit> _allyTargets = new();

    private TurnUnit _target;
    private int _currentTargetIndex = 0;
    private bool _wasLeftHeld = false;
    private bool _wasRightHeld = false;
    public System.Action<TurnUnit> TargetSelected;

    private void OnEnable()
    {
        orderController.OnOrderUpdated += UpdateCharacters;
        moveInput.action.Enable();
        confirmInput.action.Enable();
        confirmInput.action.performed += OnConfirmPressed;
    }

    private void OnDisable()
    {
        orderController.OnOrderUpdated -= UpdateCharacters;
        moveInput.action.Disable();
        confirmInput.action.performed -= OnConfirmPressed;
        confirmInput.action.Disable();
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
        if (!_isTargeting || _enemyTargets.Count == 0)
            return;

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

        if (_target != null)
            cam.transform.LookAt(_target.gObject.transform.position);

        if (confirmInput.action.WasPressedThisFrame())
        {
            TargetSelected?.Invoke(_target);
            StopTargeting();
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
                _allyTargets.Add(unit);
        }

        _enemyTargets.Sort((a, b) => a.stats.index.CompareTo(b.stats.index));
        _allyTargets.Sort((a, b) => a.stats.index.CompareTo(b.stats.index));
    }
    private void OnConfirmPressed(InputAction.CallbackContext ctx)
    {
        if (!_isTargeting) return;

        TargetSelected?.Invoke(_target);
        StopTargeting();
    }

    public void StartTargeting()
    {
        _originalCamTransform = cam.transform; //TODO: make a camera controller to smooth camera transitions
        _isTargeting = true;

        if (_enemyTargets.Count > 0)
        {
            _currentTargetIndex = 0;
            _target = _enemyTargets[_currentTargetIndex];
            cam.transform.LookAt(_target.gObject.transform.position); //TODO: make a camera controller to smooth camera transitions
            aim.SetActive(true);
        }
    }

    public void StopTargeting()
    {
        _isTargeting = false;
        aim.SetActive(false);
    }

    public void ReturnCameraRotation()
    {
        cam.transform.rotation = _originalCamTransform.rotation; //TODO: make a camera controller to smooth camera transitions
    }
}
