using UnityEngine;

public class PoseSample : MonoBehaviour
{
    public AnimationClip clip;
    public float timeInSeconds;

    void Start()
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
