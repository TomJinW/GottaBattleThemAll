using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Battle Information")]
    public List<MonsterUnit> partyMonsters;

    private void Start()
    {
        partyMonsters = Internals.partyMonsters;
    }
}
