using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlller : MonoBehaviour, ICharacter
{
    [Header("Links")]
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Physics & Snapping")]
    [SerializeField] private float gravity = -5.0f;
    [SerializeField] private float snapDistance = 1f;
    [SerializeField] private LayerMask groundLayer = 3;

    private CharacterController controller;
    private Transform cam;
    private bool isSprinting = false;
    private float _verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        if (playerAnimator == null) playerAnimator = GetComponent<PlayerAnimator>();
    }

    public void SetSpeedBoost(bool active) => isSprinting = active;

    public void HandleMove(Vector2 input)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        bool isMoving = moveDir.sqrMagnitude > 0.001f;
        float targetAnimSpeed = isMoving ? (isSprinting ? 1f : 0.5f) : 0f;

        if (playerAnimator != null) playerAnimator.UpdateMovementState(targetAnimSpeed);

        if (isMoving)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();

            Vector3 move = camForward * moveDir.z + camRight * moveDir.x;

            if (move != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, playerSettings.rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void OnRootMotionReceived(Vector3 deltaPos, Quaternion deltaRot)
    {
        if (deltaPos.y > 0.001f)
        {
            _verticalVelocity = 0;
            controller.Move(deltaPos);
            return;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, snapDistance + 0.5f, groundLayer))
        {
            _verticalVelocity = 0;

            Vector3 snapMove = deltaPos;
            snapMove.y = -(hit.distance - 0.5f) - 0.05f;

            controller.Move(snapMove);
            return;
        }

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 fallMove = deltaPos;
        fallMove.y += _verticalVelocity * Time.deltaTime;

        controller.Move(fallMove);
    }

    public void HandleAttack()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.HandleAttack();
    }
}