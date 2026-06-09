using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private string _battleSceneName = "BattleScene";
    [SerializeField] private float _delayBeforeLoad = 0.5f;

    private bool _isTransitioning = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy attacked");
            other.GetComponent<EnemyAI>()?.OnAttacked();
            StartCoroutine(LoadSceneWithDelay());
        }
    }
    private IEnumerator LoadSceneWithDelay()
    {
        _isTransitioning = true;

        yield return new WaitForSeconds(_delayBeforeLoad);

        SceneManager.LoadScene(_battleSceneName);
    }
}
