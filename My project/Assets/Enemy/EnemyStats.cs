using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("EnemyAI")]
    public float patrolingSpeed = 2f;
    public float chasingSpeed = 10f;
    public float eyeHeight = 0.5f;
    public float forwardOffset = 0.5f;
    public float fovAngle = 45f;
}