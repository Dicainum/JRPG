using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnUnit
{
    public CharacterStats stats;
    public bool IsAlive => stats != null && stats.isAlive;

    public GameObject gObject;
}

public class OrderController : MonoBehaviour
{
    public static OrderController Order { get; private set; }
    public readonly List<TurnUnit> units = new();
    public readonly Queue<TurnUnit> turnQueue = new();
    private readonly Dictionary<TurnUnit, float> timeToAct = new();

    public TurnUnit currentUnit;
    public BattleAttack battleAttack;
    private bool isProcessingTurn;

    public Action<TurnUnit> OnTurnStarted;
    public Action<TurnUnit> OnTurnEnded;
    public Action<TurnUnit> OnActionPerformed;
    public Action<List<TurnUnit>> OnOrderUpdated;
    [SerializeField] private ShowOrder showOrder;
    [SerializeField] private LoadingUIController _loadingUIController;

    [Header("Settings")] [SerializeField] private int previewLength = 10;
    [SerializeField] private float turnThreshold = 100f;

    [Header("Databases")]
    public EnemiesDatabase enemiesDatabase;

    private void Awake()
    {
        // Assign this instance as the singleton
        if (Order != null && Order != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Order = this;
        }
    }

    private void OnDisable()
    {
        if (battleAttack != null)
        {
            battleAttack.Attacked -= UseAction;
        }
    }

    private void Start()
    {
        CacheAllUnits();
        InitializeTimeToAct();
        RecalculateTurnOrder();
        _loadingUIController.StartAnimation();
    }
    
    public void CacheAllUnits()
    {
        units.Clear();

        var allObjects = new List<GameObject>();
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Character"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        foreach (var obj in allObjects)
        {
            if (obj.TryGetComponent<CharacterStats>(out var stats))
                units.Add(new TurnUnit { stats = stats, gObject = obj });

        }

        OnOrderUpdated?.Invoke(GetTurnOrderPreview());
    }

    private void InitializeTimeToAct()
    {
        timeToAct.Clear();
        const float MIN_SPEED = 0.0001f;

        foreach (var u in units)
        {
            if (!u.IsAlive) continue;
            float sp = Mathf.Max(u.stats.speed, MIN_SPEED);
            timeToAct[u] = turnThreshold / sp;
        }
    }

    private void RecalculateTurnOrder()
    {
        const float MIN_SPEED = 0.0001f;

        var toRemove = new List<TurnUnit>();
        foreach (var kv in timeToAct)
        {
            if (!kv.Key.IsAlive)
                toRemove.Add(kv.Key);
        }

        foreach (var d in toRemove)
            timeToAct.Remove(d);

        foreach (var u in units)
        {
            if (!u.IsAlive) continue;
            if (!timeToAct.ContainsKey(u))
            {
                float sp = Mathf.Max(u.stats.speed, MIN_SPEED);
                // если юнит новый — ставим его в очередь как будто он должен ждать полный интервал. Нужно для механики реса
                timeToAct[u] = turnThreshold / sp;
            }
        }

        var simTime = new Dictionary<TurnUnit, float>(timeToAct);

        turnQueue.Clear();

        for (int i = 0; i < previewLength; i++)
        {
            TurnUnit next = null;
            float bestTime = float.MaxValue;

            foreach (var kv in simTime)
            {
                var u = kv.Key;
                var t = kv.Value;

                if (!u.IsAlive) continue;

                if (t < bestTime)
                {
                    bestTime = t;
                    next = u;
                }
                else if (Mathf.Approximately(t, bestTime))
                {
                    if (next == null || u.stats.speed > next.stats.speed)
                        next = u;
                }
            }

            if (next == null)
                break;

            turnQueue.Enqueue(next);

            float spNext = Mathf.Max(next.stats.speed, MIN_SPEED);
            simTime[next] += turnThreshold / spNext;
        }

        Debug.Log("<color=cyan>===== Новый прогноз очереди =====</color>");
        int idx = 1;
        foreach (var u in turnQueue)
        {
            Debug.Log($"{idx++}. {u.stats.characterName} (Speed: {u.stats.speed})");
        }

        Debug.Log("<color=cyan>==============================</color>");
        currentUnit = turnQueue.Dequeue();
        
        OnOrderUpdated?.Invoke(GetTurnOrderPreview());
    }

    public void StartNextTurn()
    {
        if (turnQueue.Count == 0)
        {
            RecalculateTurnOrder();
        }

        if (turnQueue.Count == 0)
        {
            Debug.LogWarning("Нет активных юнитов для хода!");
            return;
        }

        currentUnit = turnQueue.Dequeue();

        if (!currentUnit.IsAlive)
        {
            StartNextTurn();
            return;
        }

        isProcessingTurn = true;

        Debug.Log($"<color=yellow>Сейчас ходит:</color> {currentUnit.stats.characterName}");
        OnTurnStarted?.Invoke(currentUnit);
        InitializeAllUnitDependencies(currentUnit);

        if (turnQueue.Count > 0)
        {
            string nextNames = "";
            foreach (var u in turnQueue)
                nextNames += $"{u.stats.characterName} > ";
            nextNames = nextNames.TrimEnd(' ', '>');
            Debug.Log($"Следующие: {nextNames}");
        }
        else
        {
            Debug.Log("Очередь пуста - пересчет после завершения хода");
        }
    }

    public void NextTurn()
    {
        if (currentUnit == null)
            return;

        OnTurnEnded?.Invoke(currentUnit);
        isProcessingTurn = false;

        Debug.Log($"Ход завершен: {currentUnit.stats.characterName}");


        const float MIN_SPEED = 0.0001f;
        if (currentUnit.IsAlive)
        {
            if (!timeToAct.ContainsKey(currentUnit))
            {
                timeToAct[currentUnit] = turnThreshold / Mathf.Max(currentUnit.stats.speed, MIN_SPEED);
            }
            else
            {
                float sp = Mathf.Max(currentUnit.stats.speed, MIN_SPEED);
                timeToAct[currentUnit] += turnThreshold / sp;
            }
        }
        else
        {
            // если умер - удаляем
            if (timeToAct.ContainsKey(currentUnit))
                timeToAct.Remove(currentUnit);
        }

        RecalculateTurnOrder();

        StartNextTurn();
    }

    public List<TurnUnit> GetTurnOrderPreview()
    {
        return new List<TurnUnit>(turnQueue);
    }

    /// Принудительно обновляет очередь например, при изменении speed у кого-то
    public void ForceRecalculateAndRefresh()
    {
        const float MIN_SPEED = 0.0001f;
        var toAdd = new List<TurnUnit>();
        foreach (var u in units)
        {
            if (!u.IsAlive) continue;
            if (!timeToAct.ContainsKey(u))
                toAdd.Add(u);
        }

        foreach (var u in toAdd)
            timeToAct[u] = turnThreshold / Mathf.Max(u.stats.speed, MIN_SPEED);

        RecalculateTurnOrder();
    }

    public void UseAction(TurnUnit unit)
    {
        Debug.Log(unit);
        unit.stats.actions -= 1;
        OnActionPerformed?.Invoke(unit);
        if (unit.stats.actions >= 1) return;
        Debug.Log(unit.stats.characterName + " ended turn by attack");
        unit.stats.actions = unit.stats.baseActions;
        NextTurn();
    }

    private void InitializeAllUnitDependencies(TurnUnit unit)
    {
        battleAttack = unit.gObject.GetComponent<BattleAttack>();

        if (battleAttack == null) return;
        battleAttack.Attacked -= UseAction;
        battleAttack.Attacked += UseAction;
        Debug.Log($"Initialized BattleAttack for {unit.stats.characterName}");
    }
    public void CheckWinCondition()
    {
        int aliveEnemies = 0;

        foreach (var unit in units)
        {
            if (unit.gObject.CompareTag("Enemy") && unit.IsAlive)
            {
                aliveEnemies++;
            }
        }

        if (aliveEnemies <= 0)
        {
            Debug.Log("<color=green>Все враги повержены! Возвращаемся в данж...</color>");
            StartCoroutine(EndBattleRoutine());
        }
    }

    private System.Collections.IEnumerator EndBattleRoutine()
    {
        yield return new WaitForSeconds(2.5f);

        GameDataManager.isReturningFromBattle = true;

        if (enemiesDatabase != null && !string.IsNullOrEmpty(GameDataManager.currentEnemyId))
        {
            if (!enemiesDatabase.defeatedEnemies.Contains(GameDataManager.currentEnemyId))
            {
                enemiesDatabase.defeatedEnemies.Add(GameDataManager.currentEnemyId);
                Debug.Log($"<color=green>Враг {GameDataManager.currentEnemyId} записан в базу как мертвый!</color>");
            }
        }

        DG.Tweening.DOTween.KillAll();

        UnityEngine.SceneManagement.SceneManager.LoadScene(GameDataManager.dungeonSceneName);
    }
}
