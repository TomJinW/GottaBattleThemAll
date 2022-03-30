using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitControl : MonoBehaviour
{
    [Header("Pokemon 1 Attributes")]
    [SerializeField] Image pokemonOneSprite;
    [SerializeField] Image pokemonOneHealth;
    [SerializeField] Image pokemonOneHealthParent;
    [SerializeField] Text pokemonOneName;

    [Header("Pokemon 2 Attributes")]
    [SerializeField] Image pokemonTwoSprite;
    [SerializeField] Image pokemonTwoHealth;
    [SerializeField] Image pokemonTwoHealthParent;
    [SerializeField] Text pokemonTwoName;

    public Image PokemonOneSprite { get => pokemonOneSprite; set => pokemonOneSprite = value; }
    public Image PokemonTwoSprite { get => pokemonTwoSprite; set => pokemonTwoSprite = value; }

    #region setters
    public void setAllFirst(Sprite sprite, float normalHP, string name)
    {
        resetFirstEmpty();
        setFirstSprite(sprite);
        setFirstHP(normalHP);
        setFirstName(name);
    }
    public void setAllSecond(Sprite sprite, float normalHP, string name)
    {
        resetSecondEmpty();
        setSecondSprite(sprite);
        setSecondHP(normalHP);
        setSecondName(name);
    }
    public void setFirstEmpty()
    {
        pokemonOneSprite.color = Color.clear;
        pokemonOneHealth.color = Color.clear;
        pokemonOneHealthParent.color = Color.clear;
        pokemonOneName.text = "";
    }
    public void setSecondEmpty()
    {
        pokemonTwoSprite.color = Color.clear;
        pokemonTwoHealth.color = Color.clear;
        pokemonTwoHealthParent.color = Color.clear;
        pokemonTwoName.text = "";
    }

    private void setFirstSprite(Sprite newSprite)
    {
        pokemonOneSprite.sprite = newSprite;
    }
    private void setSecondSprite(Sprite newSprite)
    {
        pokemonTwoSprite.sprite = newSprite;
    }
    private void setFirstHP(float normalizedHP)
    {
        pokemonOneHealth.transform.localScale = new Vector2(normalizedHP, 1);
    }
    private void setSecondHP(float normalizedHP)
    {
        pokemonTwoHealth.transform.localScale = new Vector2(normalizedHP, 1);
    }
    private void setFirstName(string name)
    {
        pokemonOneName.text = name;
    }
    private void setSecondName(string name)
    {
        pokemonTwoName.text = name;
    }
    private void resetFirstEmpty()
    {
        pokemonOneSprite.color = Color.white;
        pokemonOneHealth.color = Color.green;
        pokemonOneHealthParent.color = Color.white;
    }
    private void resetSecondEmpty()
    {
        pokemonTwoSprite.color = Color.white;
        pokemonTwoHealth.color = Color.green;
        pokemonTwoHealthParent.color = Color.white;
    }
    #endregion
}
