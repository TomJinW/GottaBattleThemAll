using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Persistent Information")]
    public List<MonsterBase> allMonsterInGame;

    [Header("Battle Information")]
    public List<Monster> partyMonsters;
    public Monster opMonster1;
    public Monster opMonster2;
    public BattleSystem battleSystem;

    private void Start()
    {
        foreach(Monster monster in partyMonsters)
            monster.Init();
        opMonster1.Init();
        opMonster2.Init();

        StartCoroutine(InvokeBattleRoutine());
    }
    IEnumerator InvokeBattleRoutine()
    {
        yield return new WaitForSeconds(2f);
        battleSystem.intializeBattle(partyMonsters, opMonster1, opMonster2);
    }
}
