using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//Переписать этот скрипт, а то это жесть
public class EnemyBattleAttack : BattleAttack
{
    private CharacterAnimationController _animationController;
    private TurnUnit _myTurnUnit;

    protected override void OnAwake()
    {
        base.OnAwake();
        _animationController = GetComponentInChildren<CharacterAnimationController>();
    }

    private void Start()
    {
        if (OrderController.Order != null)
        {
            OrderController.Order.OnTurnStarted += HandleTurnStarted;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (OrderController.Order != null)
        {
            OrderController.Order.OnTurnStarted -= HandleTurnStarted;
        }
    }

    private void HandleTurnStarted(TurnUnit unit)
    {
        if (unit.gObject == this.gameObject)
        {
            _myTurnUnit = unit;
            attackApplied = false;

            Debug.Log($"<color=orange>[Enemy AI]</color> 1. Мой ход наступил! Запускаю StartAttacking()...");
            StartAttacking();
        }
    }

    public override void StartAttacking()
    {
        base.StartAttacking();
        StartCoroutine(EnemyAIAttackRoutine());
    }

    private IEnumerator EnemyAIAttackRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        if (_animationController != null)
        {
            while (_animationController.IsInBlockingState())
            {
                if (_myTurnUnit != null && !_myTurnUnit.IsAlive)
                {
                    EndBaseAttack();
                    yield break;
                }

                yield return null;
            }
        }
        if (_animationController != null)
        {
            _animationController.StartAimingBasicAttack();
        }

        TurnUnit target = ChoosePlayerTarget();

        if (target != null)
        {
            ApplyAttack(target);
        }
        else
        {
            EndBaseAttack();
        }
    }

    private TurnUnit ChoosePlayerTarget()
    {
        List<TurnUnit> possibleTargets = new List<TurnUnit>();

        foreach (var unit in OrderController.Order.units)
        {
            if (unit.IsAlive && unit.gObject.CompareTag("Character"))
            {
                possibleTargets.Add(unit);
            }
        }

        if (possibleTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleTargets.Count);
            return possibleTargets[randomIndex];
        }

        return null;
    }

    //protected override void ApplyAttack(TurnUnit target)
    //{
    //    if (!attackApplied)
    //    {
    //        baseAttackTarget = target;
    //        Debug.Log($"<color=orange>[Enemy AI]</color> 5. Флаг атаки в норме. Вызываю ApplyDamage()...");

    //        ApplyDamage();
    //    }
    //    else
    //    {
    //        Debug.Log($"<color=red>[Enemy AI]</color> ОШИБКА: attackApplied почему-то равен TRUE!");
    //        EndBaseAttack();
    //    }
    //}

    protected override void ApplyAttack(TurnUnit target)
    {
        if (!attackApplied)
        {
            baseAttackTarget = target;

            if (_animationController != null)
            {
                _animationController.ExecuteAction();
            }
        }
    }

    public void ApplyDamage()
    {
        CalculateDamage();

        if (baseAttackTarget != null)
        {
            baseAttackTarget.stats.TakeDamage(_damage);
            Debug.Log($"<color=red><b>>>> ВРАГ {gameObject.name} УДАРИЛ {baseAttackTarget.stats.characterName} НА {_damage} УРОНА! <<<</b></color>");
        }
        else
        {
            Debug.Log($"<color=red>[Enemy AI]</color> ОШИБКА: baseAttackTarget потерялся перед нанесением урона!");
        }

        base.ApplyAttack(baseAttackTarget);
        EndBaseAttack();
    }

    public void EndBaseAttack()
    {
        baseAttackTarget = null;
        attackApplied = true;
        Debug.Log($"<color=orange>[Enemy AI]</color> 6. Атака завершена. Передаю ход дальше (UseAction)...");
        UseAction();
    }

    public void MoveBackToPosition()
    {
        transform.DOMove(_currentUnitPosition.position, timeToMoveBack).SetEase(Ease.Linear);
    }
}