using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesDatabase", menuName = "Scriptable Objects/EnemiesDatabase")]
public class EnemiesDatabase : ScriptableObject
{
    [Header("Is dead")]
    public List<string> deadEnemyIds = new List<string>();

    public void ClearDatabase()
    {
        deadEnemyIds.Clear();
    }
}