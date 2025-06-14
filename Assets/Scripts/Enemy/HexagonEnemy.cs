using UnityEngine;

public class HexagonEnemy : Enemy
{
    protected override void Start()
    {
        enemyType = EnemyType.Hexagon;
        maxHealth = 10;
        moveSpeed = 3f;
        attackDamage = 5;
        scoreValue = 5;
        expValue = 2;
        enemyGiftingGold = 1;
        base.Start();
    }
}
