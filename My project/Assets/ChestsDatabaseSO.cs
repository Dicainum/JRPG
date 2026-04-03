using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestsDatabaseSO", menuName = "Scriptable Objects/ChestsDatabaseSO")]
public class ChestsDatabase : ScriptableObject
{
    [Tooltip("Chests' ID")]
    public List<string> openedChests = new List<string>();
    public void ResetData()
    {
        openedChests.Clear();
    }
}
