using UnityEngine;
[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Character Stats", order = 1)]

public class CharacterStatsSO : ScriptableObject
{
    public int health = -1;
    public int speed = -1;
    public int damage = -1;
    public int maxHealth = -1;
    public int baseSpeed = -1 ;
    public int baseDamage = -1;
    public string characterName = "";
    public Sprite characterIcon;
    public bool isEnemy = false;
    public int index = 0;
}
