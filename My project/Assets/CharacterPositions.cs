using UnityEngine;

public class CharacterPositions : MonoBehaviour
{
    public Transform[] charPositions;
    public static CharacterPositions characterPositions { get; private set; }

    private void Awake()
    {
        // Assign this instance as the singleton
        if (characterPositions != null && characterPositions != this)
        {
            Destroy(gameObject);
        }
        else
        {
            characterPositions = this;
        }
    }
}
