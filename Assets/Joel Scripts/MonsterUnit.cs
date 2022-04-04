using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterUnit
{
    //fixed state
    [SerializeField] private MonsterBase baseState;
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 10;

    //variable state
    [SerializeField] private int level;
    [SerializeField] private Stats leveledStats;
    [SerializeField] private Stats effectiveStats;
    [SerializeField] private Stats maxStats;

    //battle variable state
    [SerializeField] private int activeHP;
    [SerializeField] private bool isFainted;

    [SerializeField] private int[] currItemQuant;

    //public properties
    public MonsterBase BaseState { get => baseState; set => baseState = value; }
    public int Level { get => level; set => level = value; }
    
    //private properties
    public bool IsFainted { get => isFainted;}
    public Stats EffectiveStats { get => effectiveStats;}
    
    public void Init()
    {
        level = Mathf.Clamp(Level, MIN_LEVEL, MAX_LEVEL);
        
        leveledStats = baseState.BaseStats;
        for (int i = 2; i <= Level; ++i)
            leveledStats += BaseState.StatsChangePerLevel;

        effectiveStats = leveledStats;

        maxStats = baseState.BaseStats;
        for (int i = 2; i <= MAX_LEVEL; ++i)
            maxStats += BaseState.StatsChangePerLevel;

        activeHP = effectiveStats.hp;
        isFainted = false;

        for (int i = 0; i < baseState.Items.Length; i++)
        {
            currItemQuant[i] = baseState.Items[i].Quantity;
        }
    }
    
    #region getters
    public float getNormalizedHP()
    {
        return (float)activeHP / effectiveStats.hp;
    }
    #endregion

    #region modifiers
    public void levelUp()
    {
        if (level >= MAX_LEVEL)
            return;
        
        level++;
        leveledStats += BaseState.StatsChangePerLevel;
    }
    public void takeDamage(int damage)
    {
        activeHP -= damage;
        activeHP = Mathf.Clamp(activeHP, 0,effectiveStats.hp);
        isFainted = activeHP == 0 ? true : false;
    }
    public void takeTemporaryStatEffects(Stats effects)
    {
        effectiveStats += effects;
        effectiveStats.attack = Mathf.Clamp(effectiveStats.attack,baseState.BaseStats.attack, maxStats.attack);
        effectiveStats.defense = Mathf.Clamp(effectiveStats.defense, baseState.BaseStats.defense, maxStats.defense);
        effectiveStats.speed = Mathf.Clamp(effectiveStats.speed, baseState.BaseStats.speed, maxStats.speed);
        effectiveStats.hp = Mathf.Clamp(effectiveStats.hp, baseState.BaseStats.hp, maxStats.hp);
    }
    public void ResetMonster()
    {
        Init();
    }
    #endregion
}
