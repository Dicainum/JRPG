using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTargetSystem : MonoBehaviour
{
    [SerializeField] private OrderController orderController;
    [SerializeField] private InputActionReference moveInput;
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

    private void OnEnable()
    {
        orderController.OnTurnStarted += StartTargeting;
        orderController.OnOrderUpdated += UpdateCharacters;
        moveInput.action.Enable();
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= StartTargeting;
        orderController.OnOrderUpdated -= UpdateCharacters;
        moveInput.action.Disable();
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

    public void StartTargeting(TurnUnit currentUnit)
    {
        _originalCamTransform = cam.transform;
        _isTargeting = true;

        if (_enemyTargets.Count > 0)
        {
            _currentTargetIndex = 0;
            _target = _enemyTargets[_currentTargetIndex];
            cam.transform.LookAt(_target.gObject.transform.position);
            aim.SetActive(true);
        }
    }

    public void StopTargeting()
    {
        cam.transform.rotation = _originalCamTransform.rotation;
        _isTargeting = false;
        aim.SetActive(false);
    }
}
