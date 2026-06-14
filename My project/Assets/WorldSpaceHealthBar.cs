using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private Stats _targetStats;

    public void Initialize(Stats stats)
    {
        _targetStats = stats;
        healthSlider.maxValue = stats.maxHealth;
        healthSlider.value = stats.health;

        _targetStats.OnDamageTaken += UpdateHealth;
    }

    private void UpdateHealth(int damage)
    {
        healthSlider.value = _targetStats.health;
    }

    private void OnDestroy()
    {
        if (_targetStats != null)
            _targetStats.OnDamageTaken -= UpdateHealth;
    }
}