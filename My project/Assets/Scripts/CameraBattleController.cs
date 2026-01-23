using DG.Tweening;
using UnityEngine;

public class CameraBattleController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform[] cameraPositions;
    [SerializeField] private Transform[] defencePositions;
    [SerializeField] private OrderController orderController;
    [SerializeField] private float timeToMove = 1f;
    [SerializeField] private float lookAtSpeed = 1f;

    private bool isMoving = false;
    private float moveTimer = 0f;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 targetPos;
    private Quaternion targetRot;

    private void OnEnable()
    {
        orderController.OnTurnStarted += ChangeToUnitCameraPos;
    }

    private void OnDisable()
    {
        orderController.OnTurnStarted -= ChangeToUnitCameraPos;
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

    public void ChangeToUnitCameraPos(TurnUnit currentUnit)
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
        cam.transform.DOKill();

        startPos = cam.transform.position;
        startRot = cam.transform.rotation;
        targetPos = target.position;
        targetRot = target.rotation;

        moveTimer = 0f;
        isMoving = true;
    }

    public void BattleCameraLookAtTarget(GameObject target)
    {
        if (target == null) return;
        isMoving = false;
        cam.transform.DOKill();

        Vector3 dir = target.transform.position - cam.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        cam.transform
            .DORotateQuaternion(lookRot, lookAtSpeed)
            .SetEase(Ease.InOutSine);
    }

    public void BattleCameraChangeRotation(Transform rotateTarget)
    {
        if (rotateTarget == null) return;

        isMoving = false;
        cam.transform.DOKill();

        cam.transform
            .DORotateQuaternion(rotateTarget.rotation, lookAtSpeed)
            .SetEase(Ease.Linear);    
    }

    public void ResetCamera(Vector3 position, Quaternion rotation)
    {
        isMoving = false;
        cam.transform.DOKill();

        cam.transform.DOMove(position, timeToMove).SetEase(Ease.InOutSine);
        cam.transform.DORotateQuaternion(rotation, timeToMove).SetEase(Ease.InOutSine);
    }
}