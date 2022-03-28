using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster
{
    //fixed state
    private MonsterBase baseState;
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 10;

    //variable state
    private int level;
    private Stats leveledStats;
    private int activeHP;
    private bool isFainted;

    public Monster(MonsterBase baseState) : this(baseState,1) { }
    public Monster(MonsterBase baseState, int level)
    {
        this.baseState = baseState;
        
        level = Mathf.Clamp(level, MIN_LEVEL, MAX_LEVEL);
        leveledStats = baseState.baseStats;
        while(this.level!=level)
            levelUp();

        activeHP = leveledStats.hp;
        isFainted = false;
    }
    
    #region getters
    public string getName()
    {
        return baseState.name;
    }
    public float getNormalizedHP()
    {
        return activeHP / leveledStats.hp;
    }
    public Sprite getSprite()
    {
        return baseState.sprite;
    }
    public Stats getLeveledStats()
    {
        return leveledStats;
    }
    public bool isDefeated()
    {
        return isFainted;
    }
    #endregion

    #region modifiers
    public void levelUp()
    {
        if (level >= MAX_LEVEL)
            return;
        
        level++;
        leveledStats += baseState.statsChangePerLevel;
    }
    public void takeDamage(int damage)
    {
        activeHP -= damage;
        activeHP = Mathf.Max(damage, 0);
        isFainted = activeHP == 0 ? true : false;
    }
    public void revive()
    {
        activeHP = leveledStats.hp;
        isFainted = false;
    }
    #endregion
}
