using UnityEngine;

public class PoseSample : MonoBehaviour
{
    [SerializeField] private AnimationClip clip;
    [SerializeField] private float timeInSeconds;

    private void Start()
    {
        if (clip != null)
        {
            Vector3 originalPos = transform.position;
            Quaternion originalRot = transform.rotation;

            clip.SampleAnimation(gameObject, timeInSeconds);

            transform.position = originalPos;
            transform.rotation = originalRot;

            if (TryGetComponent(out Animator animator)) animator.enabled = false;
        }
    }
}
