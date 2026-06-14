using System.Collections.Generic;
using UnityEngine;

public static class GameDataManager
{
    public static bool isReturningFromBattle = false;
    public static Vector3 playerDungeonPosition;
    public static string dungeonSceneName = "DungeonScene";

    public static string currentFightingEnemyId = "";

    public static List<string> deadEnemyIds = new List<string>();
}