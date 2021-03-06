using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AreaType {
    Grass,
    Volcano,
    Snow,
}

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

    public GameObject battleCanvas;
    public BattleSystem battleSystem;

    [SerializeField]
    public AreaType playerAreaType = AreaType.Grass;

    [SerializeField]
    public MonsterBase [] areaMosnterBase;

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

    public GameObject Menu;
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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Time.timeScale != 0)
            {
                if (Internals.allowMapMovement) {
                    Internals.allowMapMovement = false;
                    Time.timeScale = 0;
                    Menu.SetActive(true);
                    Menu menu = Menu.GetComponent<Menu>();
                    menu.mainPointerIndex[1] = 0;
                    menu.mainPointerIndex[0] = 0;
                    menu.setPointerPosition();
                }

            }
            else {
                Internals.allowMapMovement = true;
                Time.timeScale = 1;
                Menu.SetActive(false);
            }
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
        Debug.Log("Start Battle routine called!");
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
        //SceneManager.LoadScene(destinationSceneName);

        if (battleCanvas != null) {
            battleCanvas.SetActive(true);

            int index1 = Random.Range(0, areaMosnterBase.Length);
            int index2 = Random.Range(0, areaMosnterBase.Length);


            int maxLevel = -1;
            foreach (MonsterUnit monsterUnit in Internals.partyMonsters) {
                if (monsterUnit.Level >= maxLevel) {
                    maxLevel = monsterUnit.Level;
                }
            }

            int level = Random.Range(1, maxLevel);


            MonsterUnit monster1 = new MonsterUnit();
            Debug.Log(index1);
            Debug.Log(index1);
            monster1.BaseState = areaMosnterBase[index1];
            monster1.Level = level;
            monster1.Init();
            MonsterUnit monster2 = new MonsterUnit();
            monster2.BaseState = areaMosnterBase[index2];
            monster2.Level = level;
            monster2.Init();

            battleSystem.intializeBattle(Internals.partyMonsters,monster1,monster2);
        }
    }
}
