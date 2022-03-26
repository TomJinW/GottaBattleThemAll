using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterWanderingDirection {
    Up,
    Down,
    Left,
    Right
}

public class Monster : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb;

    public int dexID = 0;
    public int listIndex = 0;


    public float idleTimeInSecondsMin;
    public float idleTimeInSecondsMax;

    public float movementSpeed;
    public float step;

    private bool startWandering = false;
    private Vector2 newPosition;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartWandering());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (startWandering) {
            rb.position = Vector2.MoveTowards(rb.position, newPosition,
                movementSpeed * Time.deltaTime);
        }
    }



    IEnumerator StartWandering() {

        while (true) {
            yield return new WaitForSeconds(Random.Range(idleTimeInSecondsMin,idleTimeInSecondsMax+1));

            MonsterWanderingDirection direction = (MonsterWanderingDirection)Random.Range(0, 4);
            Vector2 baseDirectionVector = new Vector2(0, 0);
            switch (direction) {
                case MonsterWanderingDirection.Up:
                    baseDirectionVector = new Vector2(0, step);
                    break;
                case MonsterWanderingDirection.Down:
                    baseDirectionVector = new Vector2(0, -step);
                    break;
                case MonsterWanderingDirection.Left:
                    baseDirectionVector = new Vector2(-step, 0);
                    break;
                case MonsterWanderingDirection.Right:
                    baseDirectionVector = new Vector2(step, 0);
                    break;
            }
            newPosition = rb.position + baseDirectionVector;
            startWandering = true;
            //if (!Internals.battleStarted) {
                
            //}
        }
        
    }
}
