using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPControl : MonoBehaviour
{
    [SerializeField] GameObject health; 
    public void SetHP(float normalizedHP)
    {
        health.transform.localScale = new Vector2(normalizedHP, 1);
    }
}
