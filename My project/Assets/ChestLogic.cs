using UnityEngine;

public class ChestLogic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int crystalsAmount = 10;
    public string chestId;
    private bool isOpened = false;

    [Header("Data")]
    public ChestsDatabase database;

    [Header("Canvas")]
    public GameObject hintObject;

    [Header("Visuals & Animation")]
    public Transform chestTop;
    public GameObject openVFX;
    public Vector3 openRotation = new Vector3(-15f, 0, 0);

    private void Start()
    {
        HideHint();
        if (database != null && database.openedChests.Contains(chestId))
        {
            isOpened = true;
            ApplyOpenState();
        }
    }
    public int ChestOpen()
    {
        if (isOpened) return 0;

        isOpened = true;
        HideHint();

        ApplyOpenState();

        if (openVFX != null)
        {
            Instantiate(openVFX, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (database != null && !string.IsNullOrEmpty(chestId))
        {
            if (!database.openedChests.Contains(chestId))
            {
                database.openedChests.Add(chestId);
            }
        }

        Debug.Log("Chest was opened");
        return crystalsAmount;
    }
    public void ShowHint()
    {
        if (hintObject != null && !isOpened)
        {
            hintObject.SetActive(true);
        }
    }

    public void HideHint()
    {
        if (hintObject != null)
        {
            hintObject.SetActive(false);
        }
    }
    private void ApplyOpenState()
    {
        if (chestTop != null)
        {
            chestTop.localEulerAngles = openRotation;
        }
    }
}
