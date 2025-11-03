using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health = 100;
    public int speed = 100;
    public int damage = 10;
    public int maxHealth;
    public int baseSpeed;
    public int baseDamage;
    public bool isAlive = true;
    public bool isStunned = false;
    public string characterName = "";
    public Sprite characterIcon;
    public bool isEnemy = false;
    public int index = 0;

    protected virtual void OnAwake()
    {
        RecalculateStats();
        UpdateUI();
    }

    protected virtual void RecalculateStats()
    {
        maxHealth = health;
        baseSpeed = speed;
        baseDamage = damage;
    }

    private void Awake()
    {
        OnAwake();
    }

    public void TakeDamage (int damageTaken)
    {
        health -= damageTaken;
        if (health <= 0)
        {
            Death();
        }
        UpdateUI();
    }

    public void ChangeSpeed (int speedChange)
    {
        speed += speedChange;
        if (speed < 0)
        {
            speed = 0;
        }
    }

    public void Heal (int heal)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        UpdateUI();
    }

    private void Death()
    {
        Debug.Log(gameObject.name + "is dead");
        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        //update UI when we`ll have one
    }
}
