using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    #region Battle System State
    public static bool isBattleActive = false;
    
    [Header("Loop Information")]
    [SerializeField] private BATTLE status;

    
    private delegate IEnumerator BranchedAction();
    private BranchedAction[] branchingActionRoutines;
    private WaitUntil[] branchingRoutineYields;
    private void Start()
    {
        status = BATTLE.inactive;
        
        branchingActionRoutines = new BranchedAction[4];
        branchingActionRoutines[0] = MoveRoutine;
        branchingActionRoutines[1] = ItemRoutine;
        branchingActionRoutines[2] = PartyRoutine;
        branchingActionRoutines[3] = RunRoutine;

        branchingRoutineYields = new WaitUntil[4];
        branchingRoutineYields[0] = new WaitUntil(() => !isMoveRoutineActive);
        branchingRoutineYields[1] = new WaitUntil(() => !isItemRoutineActive);
        branchingRoutineYields[2] = new WaitUntil(() => !isPartyRoutineActive);
        branchingRoutineYields[3] = new WaitUntil(() => !isRunRoutineActive);

        waitUntilDialogueRoutineComplete = new WaitUntil(() => !isDialogueRoutineActive);
    }

    [Header("Unit Information")]
    [SerializeField] UnitControl playerUnit;
    [SerializeField] UnitControl opponentUnit;

    [Header("Dialogue Information")]
    [SerializeField] Text dialogueText;
    [SerializeField] private float letterPrintSpeed;
    [SerializeField] private float pauseBetweenDialogues;
    private bool isDialogueRoutineActive = false;
    private WaitUntil waitUntilDialogueRoutineComplete;

    [Header("Action Routine Information")]
    [SerializeField] GameObject actionSelector;
    private bool isActionRoutineActive = false; 
    private bool isActionButtonPressed = false; 

    [Header("Move Routine Information")]
    [SerializeField] GameObject moveSelector;
    [SerializeField] Text[] moveButtonText;
    private bool isMoveRoutineActive = false;
    private bool isMoveButtonPressed = false;

    [Header("Item Routine Information")]
    [SerializeField] GameObject itemSelector;
    private bool isItemRoutineActive = false;
    private bool isItemButtonPressed = false;

    [Header("Party Routine Information")]
    [SerializeField] GameObject partySelector;
    [SerializeField] GameObject partyButtonPrefab;
    [SerializeField] Transform partyButtonMommy;
    private int chosenMonsterIndex = 0;
    private bool isPartyRoutineActive = false;
    private bool isPartyButtonPressed = false;

    [Header("Run Routine Information")]
    [SerializeField] GameObject runSelector;
    private bool isRunRoutineActive = false;
    private bool isRunButtonPressed = false;

    private ExecuteStageData monsterOneData;
    private ExecuteStageData monsterTwoData;
    
    List<Monster> party;
    Monster p_Monster1;
    Monster p_Monster2;
    Monster o_Monster1;
    Monster o_Monster2;
    #endregion

    #region UI Functions
    private void EnableActionSelectorUI()
    {
        ResetUISelectors();
        actionSelector.SetActive(true);
        isActionButtonPressed = false;
    }
    private void EnableMoveSelectorUI()
    {
        ResetUISelectors();
        moveSelector.SetActive(true);
        isMoveButtonPressed = false;
    }
    private void EnableItemSelectorUI()
    {
        ResetUISelectors();
        itemSelector.SetActive(true);
        isItemButtonPressed = false;
    }
    private void EnablePartySelectorUI()
    {
        ResetUISelectors();
        partySelector.SetActive(true);
        isPartyButtonPressed = false;
    }
    private void EnableRunSelectorUI()
    {
        ResetUISelectors();
        runSelector.SetActive(true);
        isRunButtonPressed = false;
    }
    public void ResetUISelectors()
    {
        //Action Selection
        actionSelector.SetActive(false);
        isActionButtonPressed = false;

        //Moves Selection
        moveSelector.SetActive(false);
        isMoveButtonPressed = false;

        //Item Selection (Archit code required)
        /*itemSelector.SetActive(false);
        isItemButtonPressed = false;*/

        //Party Sellection
        partySelector.SetActive(false);
        isPartyButtonPressed = false;
        foreach(Transform child in partyButtonMommy)
            GameObject.Destroy(child.gameObject);

        //Run Selection
        /*runSelector.SetActive(false);
        isRunButtonPressed = false;*/
    }
    public void ResetDialogueUIElements()
    {
        dialogueText.text = "";
        ResetUISelectors();
    }
    public void SetUnitUIToActiveMonsters()
    {
        //Setting Sprites
        playerUnit.setSprites(p_Monster1.BaseState.Sprite, p_Monster2.BaseState.Sprite);
        opponentUnit.setSprites(o_Monster1.BaseState.Sprite, o_Monster2.BaseState.Sprite);

        //Setting HP
        playerUnit.setHPs(p_Monster1.getNormalizedHP(), p_Monster2.getNormalizedHP());
        opponentUnit.setHPs(o_Monster1.getNormalizedHP(), o_Monster2.getNormalizedHP());

        //Setting Names
        playerUnit.setNames(p_Monster1.BaseState.Name, p_Monster2.BaseState.Name);
        opponentUnit.setNames(o_Monster1.BaseState.Name, o_Monster2.BaseState.Name);
    }
    public void AddMonstersToPartyUI(List<Monster> monsterList)
    {
        int counter = 0;
        foreach (Monster monster in monsterList)
        {
            GameObject newMonsterButton = Instantiate(partyButtonPrefab, partyButtonMommy);
            
            Button button = newMonsterButton.GetComponent<Button>();
            button.onClick.AddListener(() => chooseParty(counter++));

            Text buttonText = newMonsterButton.GetComponentInChildren<Text>();
            buttonText.text = monster.BaseState.Name + " " + (monster.getNormalizedHP() * 100)+"%";
        }
    }
    #endregion

    public void intializeBattle(List<Monster> partyMonsters, Monster opMonster1, Monster opMonster2)
    {
        //player setup
        this.party = partyMonsters;
        p_Monster1 =  partyMonsters[0];
        p_Monster2 = partyMonsters[1];

        //opponent setup
        this.o_Monster1 = opMonster1;
        this.o_Monster2 = opMonster2;

        //UI setup
        SetUnitUIToActiveMonsters();
        ResetDialogueUIElements();

        //begin battle
        StartCoroutine(BattleRoutine());
    }
    
    public IEnumerator BattleRoutine()
    {
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);
        WaitUntil waitUntilActionRoutineComplete = new WaitUntil(() => !isActionRoutineActive);

        isBattleActive = true;
        status = BATTLE.beginLoop;
        StartCoroutine(InterruptRoutine());

        printDialogues(new string[] { "A battle has begun!"});
        yield return waitUntilDialogueComplete;
        
        while (isBattleActive)
        {
            if (status == BATTLE.beginLoop || status==BATTLE.firstAction)
            {
                status++; //choose first/second monsters action type
                currentRoutineReference = StartCoroutine(ActionRoutine());
                yield return waitUntilActionRoutineComplete;
            }
            else if (status == BATTLE.choosingFirstActionType || status == BATTLE.choosingSecondActionType)
            {
                status++; //choose action of chosen type for first/second monster
                int actionIndex = (int) (status == BATTLE.firstAction? monsterOneData.actionType : monsterTwoData.actionType);
                currentRoutineReference = StartCoroutine(branchingActionRoutines[actionIndex]());
                yield return branchingRoutineYields[actionIndex];
            }
            else if (status == BATTLE.secondAction)
            {
                status++; //second action complete, time to execute!
                
                //execution routine called here
            }
            else if(status == BATTLE.execution)
            {
                status++; //for now default to battle finished

                //execution complete, go to start if battle not over, else exit battle and update state based on battle result
            }
            else if(status == BATTLE.finished)
            {
                isBattleActive = false;
                status = BATTLE.inactive;
            }
        }
        Debug.Log("Battle Over!");
    }

    #region Action Routine and its helpers
    private IEnumerator ActionRoutine()
    {
        isActionRoutineActive = true;

        WaitUntil waitUntilActionButtonPressed = new WaitUntil(() => isActionButtonPressed);
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);
        Monster monster = status == BATTLE.choosingFirstActionType ? p_Monster1 : (status == BATTLE.choosingSecondActionType ? p_Monster2 : null);
       
        printDialogues(new string[] { "What will " + monster.BaseState.Name + " do?" });
        yield return waitUntilDialogueComplete;
        
        EnableActionSelectorUI();
        yield return waitUntilActionButtonPressed;

        ResetUISelectors();
        isActionRoutineActive = false;
    }
    public void chooseAction(int actionIndex)
    {
        isActionButtonPressed = true;

        if (status == BATTLE.choosingFirstActionType)
            monsterOneData.actionType = (ExecuteStageData.ActionType)actionIndex;
        else if (status == BATTLE.choosingSecondActionType)
            monsterTwoData.actionType = (ExecuteStageData.ActionType)actionIndex;
    }
    #endregion

    #region Move Routine and its helpers
    private IEnumerator MoveRoutine()
    {
        isMoveRoutineActive = true;
        
        WaitUntil waitUntilMoveButtonPressed = new WaitUntil(() => isMoveButtonPressed);
        Monster monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        
        printDialogues(new string[] { "Which attack will " + monster.BaseState.Name + " use?" });
        yield return waitUntilDialogueRoutineComplete;

        assignMovesToMoveButtons(monster);
        EnableMoveSelectorUI();

        yield return waitUntilMoveButtonPressed;
        ResetUISelectors();

        isMoveRoutineActive = false;
    }
    public void assignMovesToMoveButtons(Monster monster)
    {
        MoveBase[] monsterMoves = monster.BaseState.Moves;

        for(int i = 0;i<monsterMoves.Length;i++)
            moveButtonText[i].text = monsterMoves[i].Name;
    }
    public void chooseMove(int moveIndex)
    {
        isMoveButtonPressed = true;

        if (status == BATTLE.firstAction)
            monsterOneData.nextMove = p_Monster1.BaseState.Moves[moveIndex];
        else if (status == BATTLE.secondAction)
            monsterTwoData.nextMove = p_Monster2.BaseState.Moves[moveIndex];
    }
    #endregion

    #region Item Routine and its helpers
    private IEnumerator ItemRoutine()
    {
        //Requires Archit code
        yield return null;
    }
    #endregion

    #region Party Routine and its helpers
    private IEnumerator PartyRoutine()
    {
        isPartyRoutineActive = true;
        
        WaitUntil waitUntilPartyButtonPressed = new WaitUntil(() => isPartyButtonPressed);
        Monster monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);

        printDialogues(new string[] { "Choose a monster to replace " + monster.BaseState.Name+"..."});
        yield return waitUntilDialogueRoutineComplete;

        EnablePartySelectorUI();
        List<Monster> choosableMonsters = CreateHealthyMonsterList();
        AddMonstersToPartyUI(choosableMonsters);

        yield return waitUntilPartyButtonPressed;
        ResetUISelectors();

        if (status == BATTLE.firstAction)
            monsterOneData.nextMonster = choosableMonsters[chosenMonsterIndex];
        else if (status == BATTLE.secondAction)
            monsterTwoData.nextMonster = choosableMonsters[chosenMonsterIndex];

        isPartyRoutineActive = false;
    }
    private List<Monster> CreateHealthyMonsterList()
    {
        Monster possibleDuplicate = null;
        if (status == BATTLE.secondAction && monsterOneData.actionType == ExecuteStageData.ActionType.party)
            possibleDuplicate = monsterOneData.nextMonster;

        List<Monster> healthyList = new List<Monster>();
        
        party.ForEach(delegate (Monster m){
            if (!m.IsFainted && m!=p_Monster1 && m!=p_Monster2 && m!=possibleDuplicate)
                healthyList.Add(m);
        });
        return healthyList;
    }
    public void chooseParty(int partyIndex)
    {
        isPartyButtonPressed = true;
        chosenMonsterIndex = partyIndex;
    }
    #endregion

    #region Run Routine and its helpers
    private IEnumerator RunRoutine()
    {
        yield return null;
    }
    #endregion

    #region Dialogue Routine and its helpers
    public IEnumerator DialogueRoutine(string[] dialogues)
    {
        isDialogueRoutineActive = true;
        foreach (string dialogue in dialogues)
        {
            dialogueText.text = "";
            foreach (char dChar in dialogue)
            {
                dialogueText.text += dChar;
                yield return new WaitForSeconds(1 / letterPrintSpeed);
            }
            yield return new WaitForSeconds(pauseBetweenDialogues);
        }
        isDialogueRoutineActive = false;
    }
    public void printDialogues(string[] dialogues)
    {
        StartCoroutine(DialogueRoutine(dialogues));
    }
    #endregion

    #region Backtracking mechanism
    private bool escapeKeyPressed = false;
    private Coroutine currentRoutineReference;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escapeKeyPressed)
            escapeKeyPressed = true;
    }

    private bool CheckRoutineBooleans()
    {
        if (isActionRoutineActive || isMoveRoutineActive || isItemRoutineActive || isPartyRoutineActive || isRunRoutineActive)
            return true;
        else
            return false;
    }
    private void ResetRoutines()
    {
        if (!CheckRoutineBooleans())
            return;
        
        StopCoroutine(currentRoutineReference);
        isActionRoutineActive = isMoveRoutineActive = isItemRoutineActive = isPartyRoutineActive = isRunRoutineActive = false;
    }
    private IEnumerator InterruptRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while(isBattleActive)
        {
            yield return wait;
            
            if (status < BATTLE.firstAction || status > BATTLE.secondAction)
                continue;
            
            if(escapeKeyPressed)
            {
                yield return waitUntilDialogueRoutineComplete;
                
                status-=2; 
                ResetRoutines();
                ResetDialogueUIElements();

                escapeKeyPressed = false;
            }
        }
    }
    #endregion
}

public enum BATTLE
{
    inactive,beginLoop,choosingFirstActionType,firstAction,choosingSecondActionType,secondAction,execution,finished
}
public struct ExecuteStageData
{
    public enum ActionType
    {
        move,items,party,run
    }
    
    public ActionType actionType;
    public bool runAway;
    public MoveBase nextMove;
    public Monster nextMonster;
}
