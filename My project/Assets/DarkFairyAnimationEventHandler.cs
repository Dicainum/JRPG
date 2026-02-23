using UnityEngine;

public class DarkFairyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private ArrowOfDarknessSkill _arrowOfDarknessSkill;
    [SerializeField] private GameObject _bow;

    public void SpawnBow()
    {
        _bow.SetActive(true);
    }

    public void DespawnBow()
    {
        _bow.SetActive(false);
    }
}
