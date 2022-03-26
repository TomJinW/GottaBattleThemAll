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
        AddNewMove("Flamethrower", Type.Volcano, Target.singOp, 6, new Stats(0, 0, 0, 0), 10);
        AddNewMove("Hydro Pump", Type.Swamp, Target.singOp, 6, new Stats(0, 0, 0, 0), 10);
        AddNewMove("Harden", Type.None, Target.self, 0, new Stats(0, 1, 0, 0), 0);
        AddNewMove("Water Pulse", Type.Swamp, Target.singOp, 6, new Stats(0, 0, 0, 0), 10);
    }
    private void AddNewMove(string moveName, Type moveType,Target moveTarget, int baseDamage, Stats myStats, int critProb)
    {
        allMovesInGame.Add(new Move(moveName, moveType, moveTarget, baseDamage, myStats, critProb));
        moveNames.Add(moveName);
    }
}
