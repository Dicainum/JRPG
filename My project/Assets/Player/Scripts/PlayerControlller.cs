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
    private PlayerAttack playerAttack;

    private bool isSprinting = false;
    private float _verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        if (playerAnimator == null) playerAnimator = GetComponent<PlayerAnimator>();

        playerAttack = GetComponent<PlayerAttack>();
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
                bool canRotate = playerAttack == null || !playerAttack.IsAttacking;

                if (canRotate)
                {
                    Quaternion targetRot = Quaternion.LookRotation(move);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, playerSettings.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void OnRootMotionReceived(Vector3 deltaPos, Quaternion deltaRot)
    {
        Vector3 finalMove = deltaPos;

        bool isHit = Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer);

        float distanceToGround = isHit ? (hit.distance - 0.5f) : Mathf.Infinity;

        if (isHit && distanceToGround <= snapDistance)
        {
            _verticalVelocity = 0f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;

            finalMove.y = deltaPos.y + (_verticalVelocity * Time.deltaTime);
        }

        controller.Move(finalMove);
    }

    public void HandleAttack()
    {
        if (playerAttack != null) playerAttack.HandleAttack();
    }
}