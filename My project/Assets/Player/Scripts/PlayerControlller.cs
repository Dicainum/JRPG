using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlller : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private PlayerSettings playerSettings;

    CharacterController controller;
    Transform cam;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(input.x, 0, input.y);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camForward * moveDir.z + camRight * moveDir.x;
            controller.Move(move * playerSettings.moveSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, playerSettings.rotationSpeed * Time.deltaTime);
        }
    }
}
