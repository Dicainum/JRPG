using UnityEngine;
using UnityEngine.AI;

public class DungeonPlayerSetup : MonoBehaviour
{
    private void Start()
    {
        if (GameDataManager.isReturningFromBattle)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(GameDataManager.playerDungeonPosition);
            }
            else
            {
                CharacterController cc = GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                transform.position = GameDataManager.playerDungeonPosition;

                if (cc != null) cc.enabled = true;
            }

            GameDataManager.isReturningFromBattle = false;

            Debug.Log("»грок успешно вернулс€ из бо€ на свои координаты: " + GameDataManager.playerDungeonPosition);
        }
    }
}