using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerControlller : MonoBehaviour, ICharacter
{
    [Header("Links")]
    [SerializeField] private PlayerSettings playerSettings;

    [SerializeField] private PlayerAnimator playerAnimator;

    private CharacterController controller;
    private Transform cam;
    private bool isSpeedBoosted = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        if (playerAnimator == null)
            playerAnimator = GetComponent<PlayerAnimator>();
    }

    public void HandleMove(Vector2 input)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);

        bool isMoving = moveDir.sqrMagnitude > 0.001f;

        if (playerAnimator != null)
        {
            playerAnimator.UpdateMovementState(isMoving);
        }

        if (isMoving)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camForward * moveDir.z + camRight * moveDir.x;
            float currentSpeed = 0 * (isSpeedBoosted ? playerSettings.speedBoostMultiplier : 1f); //playerSettings.Move speed instead of a 0 if want to restore phys. moving

            controller.Move(move * currentSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, playerSettings.rotationSpeed * Time.deltaTime);
        }
    }

    public void HandleAttack()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.HandleAttack();
    }

    public void SetSpeedBoost(bool active)
    {
        isSpeedBoosted = active;
    }
    public void OnRootMotionReceived(Vector3 deltaPos, Quaternion deltaRot)
    {
        controller.Move(deltaPos);
    }
}