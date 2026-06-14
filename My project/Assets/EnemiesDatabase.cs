using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesDatabase", menuName = "Data/Enemies Database")]
public class EnemiesDatabase : ScriptableObject
{
    public List<string> defeatedEnemies = new List<string>();
}