using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Persistent/Move")]
public class MoveBase : ScriptableObject
{
    public new string name;
    public Type type;
    public Target target;
    public int baseDamage;
    public int critProb;
    public Stats statEffects;
}

