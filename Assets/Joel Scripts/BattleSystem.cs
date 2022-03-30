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
    
    [Header("Target Select Routine Information")]
    [SerializeField] GameObject moveTargetSelector;
    [SerializeField] GameObject targetButtonPrefab;
    [SerializeField] Transform targetButtonMommy;
    private int chosenTargetIndex = 0;
    private bool isMoveTargetSelectRoutineActive = false;
    private bool isTargetSelectButtonPressed = false;
    private delegate void buttonFunc(int index);

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
    private void EnableTargetSelectorUI()
    {
        ResetUISelectors();
        moveTargetSelector.SetActive(true);
        isTargetSelectButtonPressed = false;
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

        //Target Selection
        moveTargetSelector.SetActive(false);
        isTargetSelectButtonPressed = false;
        foreach (Transform child in targetButtonMommy)
        {
            child.GetComponent<MonsterButtonHighlighter>().OnPointerExit(null);
            GameObject.Destroy(child.gameObject);
        }

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
        if (p_Monster1 != null)
            playerUnit.setAllFirst(p_Monster1.BaseState.Sprite, p_Monster1.getNormalizedHP(), p_Monster1.BaseState.Name);
        else
            playerUnit.setFirstEmpty();

        if (p_Monster2 != null)
            playerUnit.setAllSecond(p_Monster2.BaseState.Sprite, p_Monster2.getNormalizedHP(), p_Monster2.BaseState.Name);
        else
            playerUnit.setSecondEmpty();


        if (o_Monster1 != null)
            opponentUnit.setAllFirst(o_Monster1.BaseState.Sprite, o_Monster1.getNormalizedHP(), o_Monster1.BaseState.Name);
        else
            opponentUnit.setFirstEmpty();

        if (o_Monster2 != null)
            opponentUnit.setAllSecond(o_Monster2.BaseState.Sprite, o_Monster2.getNormalizedHP(), o_Monster2.BaseState.Name);
        else
            opponentUnit.setSecondEmpty();
    }
    private void AddMonstersToScrollableUI(List<Monster> monsterList,GameObject buttonPrefab, Transform buttonMommy,buttonFunc funcToCall)
    {
        int counter = 0;
        foreach (Monster monster in monsterList)
        {
            GameObject newMonsterButton = Instantiate(buttonPrefab, buttonMommy);
            
            Button button = newMonsterButton.GetComponent<Button>();
            button.onClick.AddListener(() => funcToCall(counter++));

            Text buttonText = newMonsterButton.GetComponentInChildren<Text>();
            buttonText.text = monster.BaseState.Name + " " + (monster.getNormalizedHP() * 100)+"%";
        }
    }
    #endregion

    #region Main Battle Routine and it's helpers
    public void intializeBattle(List<Monster> partyMonsters, Monster opMonster1, Monster opMonster2)
    {
        //player setup
        this.party = partyMonsters;
        p_Monster1 = partyMonsters[0];
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
        WaitUntil waitUnitMoveTargetSelectRoutineComplete = new WaitUntil(() => !isMoveTargetSelectRoutineActive);
        
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
                if ((status == BATTLE.choosingFirstActionType && p_Monster1 == null) || (status == BATTLE.choosingSecondActionType && p_Monster2 == null))
                    continue;

                SetActiveMonsterColor();
                currentRoutineReference = StartCoroutine(ActionRoutine());
                yield return waitUntilActionRoutineComplete;
            }
            else if (status == BATTLE.choosingFirstActionType || status == BATTLE.choosingSecondActionType)
            {
                status++; //choose action of chosen type for first/second monster
                if ((status == BATTLE.firstAction && p_Monster1 == null) || (status == BATTLE.secondAction && p_Monster2 == null))
                    continue;

                SetActiveMonsterColor();
                int actionIndex = (int)(status == BATTLE.firstAction? monsterOneData.actionType : monsterTwoData.actionType);
                currentRoutineReference = StartCoroutine(branchingActionRoutines[actionIndex]());
                yield return branchingRoutineYields[actionIndex];

                //move action requires addition target selection step if it is against single opponent
                if(actionIndex==System.Array.IndexOf(branchingActionRoutines,MoveRoutine))
                {
                    ExecuteStageData monsterData = status == BATTLE.firstAction ? monsterOneData : monsterTwoData;
                    if (monsterData.nextMove.Target != Target.singOp)
                        continue;
                    
                    currentRoutineReference = StartCoroutine(MoveTargetSelectRoutine());
                    yield return waitUnitMoveTargetSelectRoutineComplete;
                }
            }
            else if (status == BATTLE.secondAction)
            {
                status++; //second action complete, time to execute!
                
                ResetPlayerMonsterColors();
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
    private void SetActiveMonsterColor()
    {
        ResetPlayerMonsterColors();

        Image monsterImage=null;
        if (status==BATTLE.firstAction || status == BATTLE.choosingFirstActionType)
            monsterImage = playerUnit.PokemonOneSprite;
        if (status == BATTLE.secondAction || status == BATTLE.choosingSecondActionType)
            monsterImage = playerUnit.PokemonTwoSprite;

        Color ogColor = monsterImage.color;
        monsterImage.color = new Color(1, ogColor.g/2, ogColor.b/2);
    }
    private void ResetPlayerMonsterColors()
    {
        SetUnitUIToActiveMonsters();
    }
    #endregion

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

    #region Move Routine(s) and its helpers
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

        if(status==BATTLE.firstAction)
            assignTargetToMove(ref monsterOneData);
        else
            assignTargetToMove(ref monsterTwoData);

        isMoveRoutineActive = false;
    }
    private void assignMovesToMoveButtons(Monster monster)
    {
        MoveBase[] monsterMoves = monster.BaseState.Moves;

        for (int i = 0; i < monsterMoves.Length; i++)
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
    private void assignTargetToMove(ref ExecuteStageData monsterData)
    {
        Monster monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        List<Monster> targetMonsterList = new List<Monster>();

        if (monsterData.nextMove.Target==Target.self)
            targetMonsterList.Add(monster);
        else if(monsterData.nextMove.Target==Target.doubOp)
        {
            if (o_Monster1 != null)
                targetMonsterList.Add(o_Monster1);
            if (o_Monster2 != null)
                targetMonsterList.Add(o_Monster2);
        }
        else if(monsterData.nextMove.Target==Target.all)
        {
            if (o_Monster1 != null)
                targetMonsterList.Add(o_Monster1);
            if (o_Monster2 != null)
                targetMonsterList.Add(o_Monster2);

            if (p_Monster1 != null && p_Monster1 != monster)
                targetMonsterList.Add(p_Monster1);
            
            if (p_Monster2 != null && p_Monster2 != monster)
                targetMonsterList.Add(p_Monster2);

        }

        monsterData.nextMoveTarget = targetMonsterList;
    }
    private IEnumerator MoveTargetSelectRoutine()
    {
        isMoveTargetSelectRoutineActive = true;

        WaitUntil waitUntilTargetSelectButtonPressed = new WaitUntil(() => isTargetSelectButtonPressed);

        printDialogues(new string[] { "Which monster should this attack hit?" });
        yield return waitUntilDialogueRoutineComplete;

        EnableTargetSelectorUI();
        List<Monster> targetMonsters = CreateTargetMonsterList();
        AddMonstersToScrollableUI(targetMonsters, targetButtonPrefab, targetButtonMommy,chooseTarget);
        MakeTargetButtonsHighlightMonster();
        
        yield return waitUntilTargetSelectButtonPressed;
        ResetUISelectors();

        if (status == BATTLE.firstAction)
            monsterOneData.nextMoveTarget = new List<Monster> {targetMonsters[chosenMonsterIndex] };
        else if (status == BATTLE.secondAction)
            monsterTwoData.nextMoveTarget = new List<Monster> { targetMonsters[chosenMonsterIndex] };

        isMoveTargetSelectRoutineActive = false;
    }
    public List<Monster> CreateTargetMonsterList()
    {
        Monster monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        List<Monster> targetMonsters = new List<Monster>();

        if (o_Monster1 != null)
            targetMonsters.Add(o_Monster1);
        if (o_Monster2 != null)
            targetMonsters.Add(o_Monster2);

        if (p_Monster1 != null && p_Monster1 != monster)
            targetMonsters.Add(p_Monster1);

        if (p_Monster2 != null && p_Monster2 != monster)
            targetMonsters.Add(p_Monster2);
        
        return targetMonsters;
    }
    public void MakeTargetButtonsHighlightMonster()
    {
        Monster monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        Monster[] activeMonster = { o_Monster1, o_Monster2, p_Monster1, p_Monster2 };
        Image[] UI_Images = { opponentUnit.PokemonOneSprite, opponentUnit.PokemonTwoSprite, playerUnit.PokemonOneSprite, playerUnit.PokemonTwoSprite };
        int activeMonsterCounter = 0;
        
        foreach(Transform child in targetButtonMommy)
        {
            while(activeMonster[activeMonsterCounter] == null || activeMonster[activeMonsterCounter]==monster)
                activeMonsterCounter++;
            
            child.GetComponent<MonsterButtonHighlighter>().setImage(UI_Images[activeMonsterCounter]);
            activeMonsterCounter++;
        }
    }
    public void chooseTarget(int targetIndex)
    {
        isTargetSelectButtonPressed = true;
        chosenTargetIndex = targetIndex;
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
        AddMonstersToScrollableUI(choosableMonsters,partyButtonPrefab,partyButtonMommy,chooseParty);

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
        if (status == BATTLE.secondAction && p_Monster1!=null && monsterOneData.actionType == ExecuteStageData.ActionType.party)
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
        if (isActionRoutineActive || isMoveRoutineActive || isMoveTargetSelectRoutineActive || isItemRoutineActive 
                || isPartyRoutineActive || isRunRoutineActive)
            return true;
        else
            return false;
    }
    private void ResetRoutines()
    {
        if (!CheckRoutineBooleans())
            return;
        
        StopCoroutine(currentRoutineReference);
        isActionRoutineActive = isMoveRoutineActive = isMoveTargetSelectRoutineActive 
                = isItemRoutineActive = isPartyRoutineActive = isRunRoutineActive = false;
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
    public List<Monster> nextMoveTarget;
    public Monster nextMonster;
}
