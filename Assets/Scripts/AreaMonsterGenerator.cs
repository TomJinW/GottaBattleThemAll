using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMonsterGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject monsterPrefab;

    [SerializeField]
    GameObject[] MonsterSpawnPoints;
    private int MonsterSpawnPointsCount;
    public Monster [] Monsters;
    // Start is called before the first frame update

    [SerializeField]
    float timePeriodForNotTriggeringAnotherBattle;

    private float timeLeft = 0;
    private bool periodStarted = false;
    private int tmpMonsterIndex = 0;
    void GenerateNewMonster(int i) {
        GameObject tmpObj = Instantiate(monsterPrefab) as GameObject;
        tmpObj.transform.position = MonsterSpawnPoints[i].transform.position;
        Monster newMonster = tmpObj.GetComponent<Monster>();
        newMonster.listIndex = i;
        Monsters[i] = newMonster;
    }

    void Start()
    {
        MonsterSpawnPointsCount = MonsterSpawnPoints.Length;
        Monsters = new Monster[MonsterSpawnPointsCount];
        for (int i = 0; i < MonsterSpawnPointsCount; i++) {
            GenerateNewMonster(i);
        }
        if (Internals.lastBattleMonsterIndex != -1)
        {
            int index = Internals.lastBattleMonsterIndex;
            Internals.allowMapMovement = true;
            Internals.lastBattleMonsterIndex = -1;
            BattleEnds(index);
            
        }
    }

    public void BattleEnds(int index) {
        Internals.allowBattle = false;
        Internals.battleStarted = false;
        Destroy(Monsters[index].gameObject);
        timeLeft = timePeriodForNotTriggeringAnotherBattle;
        tmpMonsterIndex = index;
        periodStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (periodStarted) {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                GenerateNewMonster(tmpMonsterIndex);
                periodStarted = false;
                Internals.allowBattle = true;
            }
        }
    }
}
