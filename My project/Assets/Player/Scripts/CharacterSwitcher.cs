using UnityEngine;
using System.Linq;
using Unity.Cinemachine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Player root")]
    public GameObject playerRoot;

    [Header("List of chars")]
    public GameObject[] characterMeshes;

    [Header("Character stats (SO)")]
    public CharacterStatsSO[] characterStats;

    [Header("Camera")]
    public CinemachineCamera freeLookCamera;

    public ICharacter ActiveCharacter { get; private set; }

    private int currentIndex = 0;

    void Start()
    {
        ActiveCharacter = playerRoot.GetComponent<ICharacter>();

        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = playerRoot.transform;
            freeLookCamera.LookAt = playerRoot.transform;
        }

        SetActiveMesh(currentIndex);
        UpdateCharacterIndices();
    }

    public void SwitchCharacter()
    {
        currentIndex = (currentIndex + 1) % characterMeshes.Length;

        SetActiveMesh(currentIndex);
        UpdateCharacterIndices();

        Debug.Log($"Switched to: {characterMeshes[currentIndex].name}");
    }

    private void SetActiveMesh(int index)
    {
        for (int i = 0; i < characterMeshes.Length; i++)
            characterMeshes[i].SetActive(i == index);
    }

    private void UpdateCharacterIndices()
    {
        if (characterStats == null || characterStats.Length != characterMeshes.Length)
        {
            return;
        }

        int length = characterStats.Length;
        for (int i = 0; i < length; i++)
        {
            characterStats[(currentIndex + i) % length].index = i;
        }

        Debug.Log("New: " + string.Join(", ", characterStats.Select(s => s.index)));
    }
}
