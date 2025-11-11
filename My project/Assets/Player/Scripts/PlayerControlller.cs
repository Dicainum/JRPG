using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerControlller : MonoBehaviour, ICharacter
{
    [Header("Links")]
    [SerializeField] private PlayerSettings playerSettings;
    private CharacterController controller;
    private Transform cam;

    private bool isSpeedBoosted = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    public void HandleMove(Vector2 input)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camForward * moveDir.z + camRight * moveDir.x;
            float currentSpeed = playerSettings.moveSpeed * (isSpeedBoosted ? playerSettings.speedBoostMultiplier : 1f);
            controller.Move(move * currentSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, playerSettings.rotationSpeed * Time.deltaTime);
        }
    }

    public void HandleAttack()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.HandleAttack();
    }

    public void SetSpeedBoost(bool active)
    {
        isSpeedBoosted = active;
    }
}