using UnityEngine;
using UnityEngine.InputSystem;

public class ChestLogic : MonoBehaviour
{
    [SerializeField] private int crystalsAmount = 10;
    private bool isOpened = false;

    public int ChestOpen()
    {
        if (isOpened) return 0;

        isOpened = true;
        Debug.Log("Сундук открыт");


        return crystalsAmount;
    }
}
