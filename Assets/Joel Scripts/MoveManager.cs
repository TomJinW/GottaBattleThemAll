using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [HideInInspector] public List<string> moveNames;
    public List<Move> allMovesInGame;
    
    private void OnValidate()
    {
        allMovesInGame.Clear();
        moveNames.Clear();
        AddNewMove("Flamethrower", Type.Volcano, 6, new Stats(0, 0, 0, 0), 10);
        AddNewMove("Hydro Pump", Type.Swamp, 6, new Stats(0, 0, 0, 0), 10);
    }
    private void AddNewMove(string moveName, Type moveType,int baseDamage, Stats myStats, int critProb)
    {
        allMovesInGame.Add(new Move(moveName, moveType, baseDamage, myStats, critProb));
        moveNames.Add(moveName);
    }
}
