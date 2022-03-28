using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitControl : MonoBehaviour
{
    [Header("Pokemon 1 Attributes")]
    [SerializeField] Image pokemonOneSprite;
    [SerializeField] Image pokemonOneHealth;
    [SerializeField] Text pokemonOneName;

    [Header("Pokemon 2 Attributes")]
    [SerializeField] Image pokemonTwoSprite;
    [SerializeField] Image pokemonTwoHealth;
    [SerializeField] Text pokemonTwoName;

    #region setters
    public void setFirstSprite(Sprite newSprite)
    {
        pokemonOneSprite.sprite = newSprite;
    }
    public void setSecondSprite(Sprite newSprite)
    {
        pokemonTwoSprite.sprite = newSprite;
    }
    public void setSprites(Sprite firstSprite, Sprite secondSprite)
    {
        setFirstSprite(firstSprite);
        setSecondSprite(secondSprite);
    }
    public void setFirstHP(float normalizedHP)
    {
        pokemonOneHealth.transform.localScale = new Vector2(normalizedHP, 1);
    }
    public void setSecondHP(float normalizedHP)
    {
        pokemonTwoHealth.transform.localScale = new Vector2(normalizedHP, 1);
    }
    public void setHPs(float normalHPOne, float normalHPTwo)
    {
        setFirstHP(normalHPOne);
        setSecondHP(normalHPTwo);
    }
    public void setFirstName(string name)
    {
        pokemonOneName.text = name;
    }
    public void setSecondName(string name)
    {
        pokemonTwoName.text = name;
    }
    public void setNames(string firstName, string secondName)
    {
        setFirstName(firstName);
        setSecondName(secondName);
    }
    #endregion
}
