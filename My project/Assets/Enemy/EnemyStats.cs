using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Chase")]
    public float chasingSpeed = 10f;
    [Header("FOV")]
    public float eyeHeight = 0.5f;
    public float forwardOffset = 0.5f;
    public float fovAngle = 45f;
    [Header("AttackOptions")]
    public float visionMemoryTime = 1.5f;
    public float reactionDelay = 0.4f;
    [Header("Patrol Options")]
    public float patrolingSpeed = 2f;
    public float patrolPauseTime = 1.5f;
}