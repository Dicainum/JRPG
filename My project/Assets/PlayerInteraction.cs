using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerSettings settings;
    private ChestLogic currentChest;

    public void InteractWithChest()
    {
        if (currentChest != null)
        {
            int amount = currentChest.ChestOpen();
            settings.Crystals += amount;
            Debug.Log($"Получено {amount} кристаллов. У игрока: {settings.Crystals}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ChestLogic chest))
        {
            currentChest = chest;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ChestLogic chest))
        {
            if (currentChest == chest) currentChest = null;
        }
    }
}