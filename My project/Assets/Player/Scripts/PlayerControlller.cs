using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlller : MonoBehaviour, ICharacter
{
    [Header("Links")]
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private PlayerAnimator playerAnimator;

    private CharacterController controller;
    private Transform cam;

    private bool isSprinting = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        if (playerAnimator == null)
            playerAnimator = GetComponent<PlayerAnimator>();
    }

    public void SetSpeedBoost(bool active)
    {
        isSprinting = active;
    }

    public void HandleMove(Vector2 input)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        bool isMoving = moveDir.sqrMagnitude > 0.001f;

        float targetAnimSpeed = 0f;

        if (isMoving)
        {
            targetAnimSpeed = isSprinting ? 1f : 0.5f;
        }
        else
        {
            targetAnimSpeed = 0f;
        }

        if (playerAnimator != null)
        {
            playerAnimator.UpdateMovementState(targetAnimSpeed);
        }

        if (isMoving)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

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
        //float boost = isSprinting ? playerSettings.speedBoostMultiplier : 1f;

        //controller.Move(deltaPos * boost);
        controller.Move(deltaPos);
    }

    public void HandleAttack()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.HandleAttack();
    }
}