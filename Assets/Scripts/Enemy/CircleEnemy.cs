using UnityEngine;

public class CircleEnemy : Enemy
{
    protected override void Start()
    {
        enemyType = EnemyType.Circle;
        maxHealth = 20;
        moveSpeed = 2f;
        attackDamage = 10;
        scoreValue = 10;
        expValue = 5;
        enemyGiftingGold = 2;
        base.Start();
    }
}
