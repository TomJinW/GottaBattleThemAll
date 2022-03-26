using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpriteControl : MonoBehaviour
{
    Image selfImage;
    private void Start()
    {
        selfImage = GetComponent<Image>();
    }
    public void SetSprite(Image newSprite)
    {
        selfImage.sprite = newSprite.sprite;
    }
}
