using GhostSloth.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour, ITurnInfo
{
    public bool IsEnemyTurn { get; private set; }

    [SerializeField]
    private EntityObject player;
    [SerializeField]
    private EntityObject enemy;

    private void Start()
    {
        player.onDeath = Lose;
        enemy.onDeath  = Win;

        var playerInputManager = player.GetComponent<PlayerInputManager>();
        playerInputManager.SetTurnInfo(this);
        playerInputManager.usedAction = EnemyTurn;
    }

    private void Lose()
    {

    }

    private void Win()
    {

    }

    private void EnemyTurn()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        IsEnemyTurn = true;
        yield return new WaitForSeconds(1);
        IsEnemyTurn = false;
    }
}
