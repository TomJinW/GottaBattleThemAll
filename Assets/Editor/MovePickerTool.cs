using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MonsterBehavior))]
public class MovePickerTool : Editor
{
    MoveManager myMoveManagerScript;
    int move1Index = 0;
    int move2Index = 0;
    int move3Index = 0;
    int move4Index = 0;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("My Moves",MessageType.None);

        //get initial values
        MonsterBehavior monsterBehaviorScript = (MonsterBehavior)target;
        myMoveManagerScript = FindObjectOfType<MoveManager>();
        Move[] allMoves = myMoveManagerScript.allMovesInGame.ToArray();
        string[] moveNames = myMoveManagerScript.moveNames.ToArray();

        //set moveIndices
        move1Index = Math.Max(Array.IndexOf(allMoves,monsterBehaviorScript.moveSet[0]),0);
        move2Index = Math.Max(Array.IndexOf(allMoves, monsterBehaviorScript.moveSet[1]), 0);
        move3Index = Math.Max(Array.IndexOf(allMoves, monsterBehaviorScript.moveSet[2]), 0);
        move4Index = Math.Max(Array.IndexOf(allMoves, monsterBehaviorScript.moveSet[3]), 0);

        //allow choices
        move1Index = EditorGUILayout.Popup("Move 1",move1Index, moveNames);
        move2Index = EditorGUILayout.Popup("Move 2", move2Index, moveNames);
        move3Index = EditorGUILayout.Popup("Move 3", move3Index, moveNames);
        move4Index = EditorGUILayout.Popup("Move 4", move4Index, moveNames);

        //Update choice
        monsterBehaviorScript.moveSet[0] = allMoves[move1Index];
        monsterBehaviorScript.moveSet[1] = allMoves[move2Index];
        monsterBehaviorScript.moveSet[2] = allMoves[move3Index];
        monsterBehaviorScript.moveSet[3] = allMoves[move4Index];
        
        //currently sets the scene to dirty even if a monster is simply selected.
        EditorUtility.SetDirty(target);
    }
}

