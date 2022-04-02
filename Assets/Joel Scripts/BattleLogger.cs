using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleLogger
{
    public static readonly string LOGS_FOLDER = Application.dataPath + "/SavedLogs/";
    List<string> turnLog = new List<string>();
    
    public static void Init()
    {
        if(!Directory.Exists(LOGS_FOLDER))
        {
            Directory.CreateDirectory(LOGS_FOLDER);
        }
    }
    public void AddTurn(TurnData turn)
    {
        //serializing turn 
        turnLog.Add(JsonUtility.ToJson(turn, true));
    }
    public void SaveBattleLog()
    {
        int battleNumber = 1;
        while (File.Exists(LOGS_FOLDER + "battle_" + battleNumber + ".txt"))
            battleNumber++;

        File.WriteAllLines(LOGS_FOLDER + "battle_" + battleNumber + ".txt", turnLog);
    }
}

[System.Serializable]
public class TurnData
{
    public string[] monsterData = new string[4];
    public List<string> actionData = new List<string>();

    public void AddMonsters(MonsterUnit[] monsters)
    {
        for(int i = 0;i<monsterData.Length;i++)
        {
            //serializing monster
            if (monsters[i] != null)
                monsterData[i] = monsters[i].BaseState.name + " " + monsters[i].getNormalizedHP() * 100;
            else
                monsterData[i] = "Fainted";
        }
    }
    public void AddAction(string action)
    {
        actionData.Add(action);
    }
}