using UnityEngine;
using Unity.Cinemachine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Player root")]
    public GameObject playerRoot;

    [Header("List of chars")]
    public GameObject[] characterMeshes;

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
    }

    public void SwitchCharacter()
    {
        currentIndex = (currentIndex + 1) % characterMeshes.Length;

        SetActiveMesh(currentIndex);

        Debug.Log($"Switched to: {characterMeshes[currentIndex].name}");
    }

    private void SetActiveMesh(int index)
    {
        for (int i = 0; i < characterMeshes.Length; i++)
            characterMeshes[i].SetActive(i == index);
    }
}
