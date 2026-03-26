using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header ("DefaultStats")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    [Header("SpeedBoost")]
    public float speedBoostMultiplier = 1f;
    public static ColorSpace colorSpace = ColorSpace.Gamma;
    [Header("Inventory")]
    public float Crystals = 0;
}
