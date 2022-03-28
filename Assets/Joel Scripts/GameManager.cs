using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Battle Information")]
    public List<Monster> partyMonsters;
    public Monster opMonster1;
    public Monster opMonster2;
    public BattleSystem battleSystem;

    private void Start()
    {
        StartCoroutine(InvokeBattleRoutine());
    }
    IEnumerator InvokeBattleRoutine()
    {
        yield return new WaitForSeconds(2f);
        battleSystem.intializeBattle(partyMonsters, opMonster1, opMonster2);
    }
}
