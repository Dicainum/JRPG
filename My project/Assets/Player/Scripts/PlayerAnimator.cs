using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator[] animator;

    private int isMovingHash;

    private void Awake()
    {
        //if (animator == null)
          //  animator = GetComponent<Animator>();

        isMovingHash = Animator.StringToHash("IsMoving");
    }

    public void UpdateMovementState(bool isMoving)
    {
        if (animator != null)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(isMovingHash, isMoving);
            }
        }
    }
}