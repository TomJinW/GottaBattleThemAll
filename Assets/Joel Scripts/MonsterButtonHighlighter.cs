using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterButtonHighlighter : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image myImageToHighLight;
    Color ogColor;
    float translucence;
    public void setImage(Image image)
    {
        myImageToHighLight = image;
        ogColor = myImageToHighLight.color;
        translucence = ogColor.a / 2;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        myImageToHighLight.color = new Color(ogColor.r, ogColor.g, ogColor.b, translucence);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myImageToHighLight.color = new Color(ogColor.r, ogColor.g, ogColor.b, ogColor.a);
    }
    private void OnDestroy()
    {
        myImageToHighLight.color = new Color(ogColor.r, ogColor.g, ogColor.b, ogColor.a);
    }
}
