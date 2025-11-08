using UnityEngine;

public class MeleePositionsReference : MonoBehaviour
{
    public Transform[] MeleePositions;
    public static MeleePositionsReference meleePositionsReference { get; private set; }

    private void Awake()
    {
        // Assign this instance as the singleton
        if (meleePositionsReference != null && meleePositionsReference != this)
        {
            Destroy(gameObject);
        }
        else
        {
            meleePositionsReference = this;
        }
        
        int childCount = transform.childCount;
        MeleePositions = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            MeleePositions[i] = transform.GetChild(i);
        }
    }
}
