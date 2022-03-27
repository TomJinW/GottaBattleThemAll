using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    //[SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Internals.transitionName + "_End");
        GetComponent<Animator>().SetTrigger(Internals.transitionName + "_End");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
