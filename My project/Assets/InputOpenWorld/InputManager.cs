using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Links")]
    public CharacterSwitcher switcher;

    private PlayerInput inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        inputActions = new PlayerInput();

        inputActions.Player.OnMove.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.OnMove.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Attack.performed += ctx => HandleAttack();
        inputActions.Player.SwitchCharacter.performed += ctx => switcher.SwitchCharacter();
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Update()
    {
        if (switcher != null && switcher.ActiveCharacter != null)
            switcher.ActiveCharacter.HandleMove(moveInput);
    }

    private void HandleAttack()
    {
        if (switcher != null && switcher.ActiveCharacter != null)
            switcher.ActiveCharacter.HandleAttack();
    }
}