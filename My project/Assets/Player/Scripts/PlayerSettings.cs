using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header ("PlayerController")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
}
