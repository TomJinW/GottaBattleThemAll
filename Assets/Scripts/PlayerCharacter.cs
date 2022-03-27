using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    float movementSpeed;
    [SerializeField]
    Animator animator;
    [SerializeField]
    public AreaMonsterGenerator areaMonsterGenerator;

    [SerializeField]
    float transtionTime = 1f;
    [SerializeField]
    string transtionAnimationName = "Window";
    [SerializeField]
    Animator transitionAnimator = null;

    public Rigidbody2D rb;
    Vector2 movement;

    private string destinationSceneName = "DummyBattleScene";
    // Start is called before the first frame update
    void Start()
    {
        if (Internals.teleported) {
            Internals.teleported = false;
            transform.position = Internals.teleportedLocation;
        }
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
        if (Internals.allowMapMovement)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (Input.GetKey(KeyCode.RightShift)) {
                movement.x *= 1.8f;
                movement.y *= 1.8f;
                animator.speed = 2.0f;
            }
            else {
                animator.speed = 1.0f;
            }
        }
        else {
            movement.x = 0;
            movement.y = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!Internals.battleStarted && Internals.allowBattle &&
            collision.gameObject.CompareTag("Monster")) {
            collision.gameObject.GetComponent<Monster>().allowWandering = false;
            Debug.Log("COL");
            Internals.battleStarted = true;

            StartCoroutine(StartBattle(collision));

        }
       
    }


    IEnumerator StartBattle(Collision2D collision)
    {
        if (transitionAnimator != null)
        {
            Internals.transitionName = transtionAnimationName;
            transitionAnimator.SetTrigger(transtionAnimationName + "_Start");
        }
        Internals.allowMapMovement = false;
        yield return new WaitForSeconds(transtionTime);
        Internals.teleported = true;
        Internals.teleportedLocation = transform.position;
        Internals.lastBattleSceneName = SceneManager.GetActiveScene().name;
        Internals.lastBattleMonsterIndex = collision.gameObject.GetComponent<Monster>().listIndex;
        SceneManager.LoadScene(destinationSceneName);
    }
}
