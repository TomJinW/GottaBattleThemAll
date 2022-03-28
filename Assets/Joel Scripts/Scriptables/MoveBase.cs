using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Persistent/Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Type type;
    [SerializeField] private Target target;
    [SerializeField] private int baseDamage;
    [SerializeField] private int critProb;
    [SerializeField] private Stats statEffects;

    public string Name { get => name;}
    public Type Type { get => type;}
    public Target Target { get => target;}
    public int BaseDamage { get => baseDamage;}
    public int CritProb { get => critProb;}
    public Stats StatEffects { get => statEffects;}
}

