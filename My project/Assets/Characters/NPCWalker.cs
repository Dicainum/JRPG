using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float lifeDistance = 20f;

    private Vector3 startPos;

    public void ActivateNPC(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        startPos = position;
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= lifeDistance)
        {
            gameObject.SetActive(false);
        }
    }
}
