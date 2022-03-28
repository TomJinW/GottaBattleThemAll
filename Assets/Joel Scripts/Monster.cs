using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Monster
{
    //fixed state
    [SerializeField] private MonsterBase baseState;
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 10;

    //variable state
    [SerializeField] private int level;
    [SerializeField] private Stats leveledStats;
    [SerializeField] private int activeHP;
    [SerializeField] private bool isFainted;

    public void Init()
    {
        level = Mathf.Clamp(level, MIN_LEVEL, MAX_LEVEL);
        leveledStats = baseState.baseStats;

        for(int i = 2;i<=level;++i)
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
