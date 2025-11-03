using UnityEngine;

public class CameraBattleController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform[] cameraPositions;
    [SerializeField] private Transform[] defencePositions;
    [SerializeField] private OrderController orderController;
    [SerializeField] private float timeToMove = 1f;

    private bool isMoving = false;
    private float moveTimer = 0f;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 targetPos;
    private Quaternion targetRot;

    private void OnEnable()
    {
        orderController.OnTurnStarted += ChangeCameraPosition;
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= ChangeCameraPosition;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            moveTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(moveTimer / timeToMove);

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }

    private void ChangeCameraPosition(TurnUnit currentUnit)
    {
        if (currentUnit.stats.isEnemy)
        {
            MoveCamera(defencePositions[0]);
        }
        else
        {
            int index = currentUnit.stats.index;
            MoveCamera(cameraPositions[index]);
        }
    }

    private void MoveCamera(Transform target)
    {
        startPos = cam.transform.position;
        startRot = cam.transform.rotation;
        targetPos = target.position;
        targetRot = target.rotation;

        moveTimer = 0f;
        isMoving = true;
    }
}