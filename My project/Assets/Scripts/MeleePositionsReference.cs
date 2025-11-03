using UnityEngine;

public class MeleePositionsReference : MonoBehaviour
{
    public static Transform[] MeleePositions;

    private void Awake()
    {
        int childCount = transform.childCount;
        MeleePositions = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            MeleePositions[i] = transform.GetChild(i);
        }
    }
}
