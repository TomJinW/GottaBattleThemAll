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
    [SerializeField] private bool winState = false;

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

        ResetExecuteStageData();
    }
    private void ResetExecuteStageData()
    {
        monsterOneData.actionType = ExecuteStageData.ActionType.invalid;
        monsterOneData.nextMove = null;
        monsterOneData.nextMoveTarget = null;
        monsterOneData.nextMonster = null;

        monsterTwoData.actionType = ExecuteStageData.ActionType.invalid;
        monsterTwoData.nextMove = null;
        monsterTwoData.nextMoveTarget = null;
        monsterTwoData.nextMonster = null;
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

    [Header("Execute Routine Information")]
    private bool isExecutionRoutineActive = false;

    List<MonsterUnit> party;
    MonsterUnit p_Monster1;
    MonsterUnit p_Monster2;
    MonsterUnit o_Monster1;
    MonsterUnit o_Monster2;
    MonsterUnit o_Monster1_persistent;
    MonsterUnit o_Monster2_persistent;
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
    private void AddMonstersToScrollableUI(List<MonsterUnit> monsterList,GameObject buttonPrefab, Transform buttonMommy,buttonFunc funcToCall)
    {
        int counter = 0;
        foreach (MonsterUnit monster in monsterList)
        {
            GameObject newMonsterButton = Instantiate(buttonPrefab, buttonMommy);
            
            Button button = newMonsterButton.GetComponent<Button>();
            int localCounter = counter;
            button.onClick.AddListener(() => funcToCall(localCounter));
            Text buttonText = newMonsterButton.GetComponentInChildren<Text>();
            buttonText.text = monster.BaseState.Name + " " + (monster.getNormalizedHP() * 100)+"%";
            counter++;
        }
    }
    #endregion

    #region Main Battle Routine and it's helpers
    public void intializeBattle(List<MonsterUnit> partyMonsters, MonsterUnit opMonster1, MonsterUnit opMonster2)
    {
        //player setup
        this.party = partyMonsters;
        p_Monster1 = partyMonsters[0];
        p_Monster2 = partyMonsters[1];

        //opponent setup
        this.o_Monster1 = this.o_Monster1_persistent = opMonster1;
        this.o_Monster2 = this.o_Monster2_persistent = opMonster2;

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
                {
                    if(CreateHealthyMonsterList().Count!=0)
                    {
                        if(status==BATTLE.choosingFirstActionType)
                            monsterOneData.actionType = ExecuteStageData.ActionType.party;
                        else if(status==BATTLE.choosingSecondActionType)
                            monsterTwoData.actionType = ExecuteStageData.ActionType.party;
                    }
                    continue;
                }

                SetActiveMonsterColor();
                currentRoutineReference = StartCoroutine(ActionRoutine());
                yield return waitUntilActionRoutineComplete;
            }
            else if (status == BATTLE.choosingFirstActionType || status == BATTLE.choosingSecondActionType)
            {
                status++; //choose action of chosen type for first/second monster
                if ((status == BATTLE.firstAction && p_Monster1 == null && monsterOneData.actionType == ExecuteStageData.ActionType.invalid) 
                        || (status == BATTLE.secondAction && p_Monster2 == null && monsterTwoData.actionType == ExecuteStageData.ActionType.invalid))
                    continue;

                SetActiveMonsterColor();
                int actionIndex = (int)(status == BATTLE.firstAction? monsterOneData.actionType : monsterTwoData.actionType);
                currentRoutineReference = StartCoroutine(branchingActionRoutines[actionIndex]());
                yield return branchingRoutineYields[actionIndex];

                //move action requires addition target selection step if it is against single opponent
                ExecuteStageData monsterData = status == BATTLE.firstAction ? monsterOneData : monsterTwoData;
                if (actionIndex != System.Array.IndexOf(branchingActionRoutines, MoveRoutine) || monsterData.nextMove == null || monsterData.nextMove.Target != Target.singOp)
                    continue;
               
                currentRoutineReference = StartCoroutine(MoveTargetSelectRoutine());
                yield return waitUnitMoveTargetSelectRoutineComplete;
            }
            else if (status == BATTLE.secondAction)
            {
                status++; //second action complete, time to execute!

                WaitUntil waitUntilExecutionRoutineComplete = new WaitUntil(() => !isExecutionRoutineActive);
                ResetPlayerMonsterColors();

                StartCoroutine(ExecuteRoutine());
                yield return waitUntilExecutionRoutineComplete;
            }
            else if(status == BATTLE.execution)
            {
                //execution complete, check status of battle
                
                ResetExecuteStageData(); //reset data collected for previous execution stage

                if(isBattleWon())
                {
                    status++;
                    winState = true;
                    continue;
                }
                if(isBattleLost())
                {
                    status++;
                    winState = false;
                    continue;
                }
                
                //Battle not over yet, keep going!
                status = BATTLE.beginLoop;
            }
            else if(status == BATTLE.finished)
            {
                //battle win or loss stuff
                if(winState)
                {
                    party.Add(o_Monster1_persistent);
                    party.Add(o_Monster2_persistent);
                    Debug.Log(o_Monster1_persistent.BaseState.Name);
                    Debug.Log(o_Monster2_persistent.BaseState.Name);
                    printDialogues(new string[] {"The battle has been won!","The new monsters have been added to your party." });
                    yield return waitUntilDialogueComplete;
                }
                else
                {
                    printDialogues(new string[] { "Battle lost..", "Better luck next time hunter!" });
                    yield return waitUntilDialogueComplete;
                }
                //reseting battle system
                ResetBattleSystem();
            }
        }
    }
    private void SetActiveMonsterColor()
    {
        ResetPlayerMonsterColors();

        Image monsterImage=null;
        if (status == BATTLE.firstAction || status == BATTLE.choosingFirstActionType)
        {
            if (p_Monster1 != null)
                monsterImage = playerUnit.PokemonOneSprite;
            else
                return;
        }
        if (status == BATTLE.secondAction || status == BATTLE.choosingSecondActionType)
        {
            if (p_Monster2 != null)
                monsterImage = playerUnit.PokemonTwoSprite;
            else
                return;
        }

        Color ogColor = monsterImage.color;
        monsterImage.color = new Color(1, ogColor.g/2, ogColor.b/2);
    }
    private void ResetPlayerMonsterColors()
    {
        SetUnitUIToActiveMonsters();
    }
    private void ResetBattleSystem()
    {
        foreach (MonsterUnit m in party)
        {
            m.ResetMonster();
            m.levelUp();
        }
        
        winState = false;
        status = BATTLE.inactive;
        isBattleActive = false;
    }
    #endregion

    #region Action Routine and its helpers
    private IEnumerator ActionRoutine()
    {
        isActionRoutineActive = true;

        WaitUntil waitUntilActionButtonPressed = new WaitUntil(() => isActionButtonPressed);
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);
        MonsterUnit monster = status == BATTLE.choosingFirstActionType ? p_Monster1 : (status == BATTLE.choosingSecondActionType ? p_Monster2 : null);
       
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
        MonsterUnit monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);

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
    private void assignMovesToMoveButtons(MonsterUnit monster)
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
        MonsterUnit monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        List<MonsterUnit> targetMonsterList = new List<MonsterUnit>();

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
        List<MonsterUnit> targetMonsters = CreateTargetMonsterList();
        AddMonstersToScrollableUI(targetMonsters, targetButtonPrefab, targetButtonMommy,chooseTarget);
        MakeTargetButtonsHighlightMonster();
        
        yield return waitUntilTargetSelectButtonPressed;
        ResetUISelectors();

        if (status == BATTLE.firstAction)
            monsterOneData.nextMoveTarget = new List<MonsterUnit> {targetMonsters[chosenTargetIndex] };
        else if (status == BATTLE.secondAction)
            monsterTwoData.nextMoveTarget = new List<MonsterUnit> { targetMonsters[chosenTargetIndex] };

        isMoveTargetSelectRoutineActive = false;
    }
    public List<MonsterUnit> CreateTargetMonsterList()
    {
        MonsterUnit monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        List<MonsterUnit> targetMonsters = new List<MonsterUnit>();

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
        MonsterUnit monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);
        MonsterUnit[] activeMonster = { o_Monster1, o_Monster2, p_Monster1, p_Monster2 };
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
        MonsterUnit monster = status == BATTLE.firstAction ? p_Monster1 : (status == BATTLE.secondAction ? p_Monster2 : null);

        if(monster!=null)
            printDialogues(new string[] { "Choose a monster to replace " + monster.BaseState.Name+"..."});
        else
            printDialogues(new string[] { "Choose a monster to send out..."});

        yield return waitUntilDialogueRoutineComplete;

        EnablePartySelectorUI();
        List<MonsterUnit> choosableMonsters = CreateHealthyMonsterList();
        AddMonstersToScrollableUI(choosableMonsters,partyButtonPrefab,partyButtonMommy,chooseParty);

        yield return waitUntilPartyButtonPressed;
        ResetUISelectors();

        if (status == BATTLE.firstAction)
            monsterOneData.nextMonster = choosableMonsters[chosenMonsterIndex];
        else if (status == BATTLE.secondAction)
            monsterTwoData.nextMonster = choosableMonsters[chosenMonsterIndex];

        isPartyRoutineActive = false;
    }
    private List<MonsterUnit> CreateHealthyMonsterList()
    {
        MonsterUnit possibleDuplicate = null;
        if (status == BATTLE.secondAction && monsterOneData.actionType == ExecuteStageData.ActionType.party)
            possibleDuplicate = monsterOneData.nextMonster;

        List<MonsterUnit> healthyList = new List<MonsterUnit>();
        
        party.ForEach(delegate (MonsterUnit m){
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

    #region Execute Routine(s) and its helpers
    private IEnumerator ExecuteRoutine()
    {
        isExecutionRoutineActive = true;

        #region Switching Region
        WaitForSeconds waitAfterSwitching = new WaitForSeconds(0.5f);
        if(monsterOneData.actionType==ExecuteStageData.ActionType.party)
        {
            if(p_Monster1 != null)
                printDialogues(new string[] {p_Monster1.BaseState.name+" is being switched to "+monsterOneData.nextMonster.BaseState.name});
            else
                printDialogues(new string[] {"Sending out " + monsterOneData.nextMonster.BaseState.name });

            yield return waitUntilDialogueRoutineComplete;

            p_Monster1 = monsterOneData.nextMonster;
            SetUnitUIToActiveMonsters();
            yield return waitAfterSwitching;
        }
        if (monsterTwoData.actionType == ExecuteStageData.ActionType.party)
        {
            if (p_Monster2 != null)
                printDialogues(new string[] { p_Monster2.BaseState.name + " is being switched to " + monsterTwoData.nextMonster.BaseState.name });
            else
                printDialogues(new string[] { "Sending out " + monsterTwoData.nextMonster.BaseState.name });
            
            yield return waitUntilDialogueRoutineComplete;

            p_Monster2 = monsterTwoData.nextMonster;
            SetUnitUIToActiveMonsters();
            yield return waitAfterSwitching;
        }
        #endregion

        #region Move's region
        WaitUntil waitUntilSelfMoveRoutineComplete = new WaitUntil(() => !isSelfMoveRoutineActive);
        WaitUntil waitUntilOtherMoveRoutineComplete = new WaitUntil(() => !isOtherMoveRoutineActive);
        
        List<MonsterUnit> inBattleMonster = new List<MonsterUnit>() { p_Monster1, p_Monster2, o_Monster1, o_Monster2 };
        inBattleMonster.Sort(delegate (MonsterUnit m1, MonsterUnit m2)
        {
           if(m1==null)
                return -1;
            if (m2 == null)
                return 1;
            return m2.LeveledStats.speed - m1.LeveledStats.speed;
        }); //sorted by speed
        foreach(MonsterUnit m in inBattleMonster)
        {
            if (m == null)
                continue;
            
            else if(m==p_Monster1 && monsterOneData.actionType==ExecuteStageData.ActionType.move)
            {
                if (monsterOneData.nextMove.Target == Target.self)
                    StartCoroutine(selfMoveRoutine(p_Monster1, monsterOneData.nextMove, playerUnit.PokemonOneSprite));

                else
                    StartCoroutine(otherMoveRoutine(p_Monster1, monsterOneData.nextMove, monsterOneData.nextMoveTarget, playerUnit.PokemonOneSprite));
            }

            else if (m == p_Monster2 && monsterTwoData.actionType == ExecuteStageData.ActionType.move)
            {
                if (monsterTwoData.nextMove.Target == Target.self)
                    StartCoroutine(selfMoveRoutine(p_Monster2, monsterTwoData.nextMove, playerUnit.PokemonTwoSprite));
                else
                    StartCoroutine(otherMoveRoutine(p_Monster2, monsterTwoData.nextMove, monsterTwoData.nextMoveTarget, playerUnit.PokemonTwoSprite));
            }
            else if(m== o_Monster1 || m == o_Monster2)
            {
                MoveBase opMove = m.BaseState.Moves[Random.Range(0,4)];
                if(opMove.Target == Target.self)
                    StartCoroutine(selfMoveRoutine(m, opMove, FindTargetImage(m),true));
                else
                {
                    List<MonsterUnit> targetForOpponents = CreateTargetListForOpponentMonster(m,opMove);
                    StartCoroutine(otherMoveRoutine(m, opMove, targetForOpponents, FindTargetImage(m),true));
                }
            }
            
            yield return waitUntilSelfMoveRoutineComplete;
            yield return waitUntilOtherMoveRoutineComplete;
        }
        #endregion

        isExecutionRoutineActive = false;
    }
    private bool isBattleWon()
    {
        if (o_Monster1 == null && o_Monster2 == null)
            return true;
        return false;
    }
    private bool isBattleLost()
    {
        foreach(MonsterUnit monster in party)
        {
            if (!monster.IsFainted)
                return false;
        }
        return true;
    }
    private bool isSelfMoveRoutineActive = false;
    private IEnumerator selfMoveRoutine(MonsterUnit self, MoveBase move, Image selfImage,bool isOpMonster=false)
    {
        isSelfMoveRoutineActive = true;

        WaitForSeconds waitAfterMove = new WaitForSeconds(0.5f);
        Color ogColor = selfImage.color;
        selfImage.color = new Color(ogColor.r / 2, 1, ogColor.b / 2);
        
        self.takeDamage(move.BaseDamage);
        self.takeTemporaryStatEffects(move.StatEffects);
        if(!isOpMonster)
            printDialogues(new string[]{self.BaseState.name+" used "+move.name});
        else
            printDialogues(new string[] {"Opposing "+self.BaseState.name + " used " + move.name});
        yield return waitUntilDialogueRoutineComplete;

        selfImage.color = ogColor;
        postMoveStateUpdate();
        
        yield return waitAfterMove;

        isSelfMoveRoutineActive = false;
    }
   
    private bool isOtherMoveRoutineActive = false;
    private IEnumerator otherMoveRoutine(MonsterUnit self, MoveBase move, List<MonsterUnit> targets, Image selfImage,bool isOpMonster=false)
    {
        isOtherMoveRoutineActive = true;

        WaitForSeconds waitBetweenTargets = new WaitForSeconds(0.5f);
        Color ogColor = selfImage.color;

        //opponent vars
        Color targetColor;
        Image targetImage;

        foreach (MonsterUnit target in targets)
        {
            if (target.IsFainted)
                continue;

            targetImage = FindTargetImage(target);
            targetColor = targetImage.color;

            selfImage.color = new Color(1, ogColor.g/2, ogColor.b / 2);
            targetImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, targetColor.a / 2);
            
            target.takeDamage(move.BaseDamage);
            target.takeTemporaryStatEffects(move.StatEffects);

            if(!isOpMonster)
                printDialogues(new string[] { self.BaseState.name + " used " + move.name + " on "+target.BaseState.Name});
            else
                printDialogues(new string[] {"Foe "+self.BaseState.name + " used " + move.name + " on " + target.BaseState.Name });
            yield return waitUntilDialogueRoutineComplete;

            selfImage.color = ogColor;
            targetImage.color = targetColor;

            postMoveStateUpdate();
            yield return waitBetweenTargets;
        }

        isOtherMoveRoutineActive = false;
    }
    private Image FindTargetImage(MonsterUnit m)
    {
        if (m == p_Monster1)
            return playerUnit.PokemonOneSprite;
        if (m == p_Monster2)
            return playerUnit.PokemonTwoSprite;

        if (m == o_Monster1)
            return opponentUnit.PokemonOneSprite;
        if (m == o_Monster2)
            return opponentUnit.PokemonTwoSprite;

        return null;
    }
    private List<MonsterUnit> CreateTargetListForOpponentMonster(MonsterUnit currentOp, MoveBase opMove)
    {
        List<MonsterUnit> targets = new List<MonsterUnit>();
        
        if(opMove.Target==Target.singOp)
            targets.Add(Random.Range(0, 2) == 0 ? p_Monster1 : p_Monster2);
        else if(opMove.Target==Target.doubOp)
        {
            targets.Add(p_Monster1);
            targets.Add(p_Monster2);
        }
        else if(opMove.Target==Target.all)
        {
            targets.Add(p_Monster1);
            targets.Add(p_Monster2);
            targets.Add(currentOp == o_Monster1 ? o_Monster1 : o_Monster2);
        }
        return targets;
    }
    private void postMoveStateUpdate()
    {
        if (p_Monster1 != null && p_Monster1.IsFainted)
            p_Monster1 = null;
        if (p_Monster2 != null && p_Monster2.IsFainted)
            p_Monster2 = null;
        if (o_Monster1 != null && o_Monster1.IsFainted)
            o_Monster1 = null;
        if (o_Monster2 != null && o_Monster2.IsFainted)
            o_Monster2 = null;
        
        SetUnitUIToActiveMonsters();
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
        if (Input.GetKeyDown(KeyCode.Escape) && !escapeKeyPressed && (status>=BATTLE.firstAction && status<=BATTLE.secondAction) && !isDialogueRoutineActive)
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
            
            if(escapeKeyPressed)
            {
                escapeKeyPressed = false;

                if (status < BATTLE.firstAction || status > BATTLE.secondAction)
                    continue;
                yield return waitUntilDialogueRoutineComplete;
                
                status-=2; 
                ResetRoutines();
                ResetDialogueUIElements();
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
        move,items,party,run,invalid
    }
    
    public ActionType actionType;
    public bool? runAway;
    public MoveBase nextMove;
    public List<MonsterUnit> nextMoveTarget;
    public MonsterUnit nextMonster;
}

