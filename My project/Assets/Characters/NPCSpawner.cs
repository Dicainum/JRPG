using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject [] npcPrefab;
    [SerializeField] private int poolSize = 5;

    [SerializeField] private Transform [] spawnPoint;
    [SerializeField] private float spawnInterval = 7f;

    private List<NPCWalker> pool;
    private float timer;

    void Start()
    {
        pool = new List<NPCWalker>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(npcPrefab[Random.Range(0, npcPrefab.Length)]);
            NPCWalker walker = obj.GetComponent<NPCWalker>();
            obj.SetActive(false);
            pool.Add(walker);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnNPC();
            timer = 0;
        }
    }

    void SpawnNPC()
    {
        foreach (NPCWalker walker in pool)
        {
            if (!walker.gameObject.activeInHierarchy)
            {
                Transform randomSpawn = spawnPoint[Random.Range(0, spawnPoint.Length)];
                walker.ActivateNPC(randomSpawn.position, randomSpawn.rotation);
                return;
            }
        }
    }
}
