using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    float movementSpeed;
    [SerializeField]
    Animator animator;
    [SerializeField]
    AreaMonsterGenerator areaMonsterGenerator;

    public Rigidbody2D rb;
    Vector2 movement;
    // Start is called before the first frame update
    void Start()
    {
    }

    void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    // Update is called once per frame
    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!Internals.battleStarted && Internals.allowBattle &&
            collision.gameObject.CompareTag("Monster")) {
            Debug.Log("COL");
            Internals.battleStarted = true;

            int index = collision.gameObject.GetComponent<Monster>().listIndex;
            areaMonsterGenerator.BattleEnds(index);
        }
       
    }
}
