using UnityEngine;

public interface ICharacter
{
    void HandleMove(UnityEngine.Vector2 input);
    void HandleAttack();
}
