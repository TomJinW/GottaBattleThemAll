using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public static bool isBattleActive = false;

    [Header("Unit Information")]
    [SerializeField] UnitControl playerUnit;
    [SerializeField] UnitControl opponentUnit;

    [Header("Dialogue Information")]
    [SerializeField] Text dialogueText;
    [SerializeField] private float letterPrintSpeed;
    [SerializeField] private float pauseBetweenDialogues;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] Text[] moveButtonText;

    #region Monster Variables
    List<Monster> party;
    Monster p_Monster1;
    Monster p_Monster2;
    Monster o_Monster1;
    Monster o_Monster2;
    #endregion

    #region Battle Loop Controllers
    private bool isDialogueRoutineActive = false; //reflects if dialogue routine is active

    private bool isActionChoiceRoutineActive = false; //for action choice routine
    private bool isActionButtonPressed = false; //to check if action button is pressed
    private bool isMoveButtonPressed = false; //to check if move button is pressed
    private bool isHandleActionRoutineActive = false; //for action choice routine
    private ACTION monsterOneAction;
    private ACTION monsterTwoAction;

    private void EnableActionSelector()
    {
        ResetSelectors();
        actionSelector.SetActive(true);
        isActionButtonPressed = false;
    }
    private void EnableMoveSelector()
    {
        ResetSelectors();
        moveSelector.SetActive(true);
        isMoveButtonPressed = false;
    }
    public void ResetSelectors()
    {
        //Action Selection
        actionSelector.SetActive(false);
        isActionButtonPressed = false;

        //Moves Selection
        moveSelector.SetActive(false);
        isMoveButtonPressed = false;
    }
    #endregion

    [Header("Battle State")]
    [SerializeField] private BATTLE status = BATTLE.inactive;

    #region Initilaizers
    public void intializeBattle(List<Monster> partyMonsters, Monster opMonster1, Monster opMonster2)
    {
        #region Unit Setup
        //player setup
        this.party = partyMonsters;
        p_Monster1 =  partyMonsters[0];
        p_Monster2 = partyMonsters[1];

        //opponent setup
        this.o_Monster1 = opMonster1;
        this.o_Monster2 = opMonster2;

        setUnitUIElements();
        #endregion

        #region Dialogue Panel Setup
        dialogueText.text = "";
        actionSelector.SetActive(false);
        moveSelector.SetActive(false);
        #endregion

        //begin battle
        StartCoroutine(BattleRoutine());
    }
    public void setUnitUIElements()
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
    #endregion

    public IEnumerator BattleRoutine()
    {
        isBattleActive = true;
        status = BATTLE.begun;

        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);
        WaitUntil waitUntilActionChoiceComplete = new WaitUntil(() => !isActionChoiceRoutineActive);
        WaitUntil waitUntilActionHandleComplete = new WaitUntil(() => !isHandleActionRoutineActive);

        printDialogues(new string[] { "A battle has begun!"});
        yield return waitUntilDialogueComplete;
        
        while (isBattleActive)
        {
            status = BATTLE.firstChoice;
            StartCoroutine(ActionChoiceRoutine());
            yield return waitUntilActionChoiceComplete;
            StartCoroutine(HandleActionRoutine());
            yield return waitUntilActionHandleComplete;
            
            status = BATTLE.secondChoice;
            StartCoroutine(ActionChoiceRoutine());
            yield return waitUntilActionChoiceComplete;
            StartCoroutine(HandleActionRoutine());
            yield return waitUntilActionHandleComplete;

            isBattleActive = false;
        }
        Debug.Log("Battle Over!");
    }

    #region Action Choice functions
    public IEnumerator ActionChoiceRoutine()
    {
        isActionChoiceRoutineActive = true;

        WaitUntil waitUntilActionButtonPressed = new WaitUntil(() => isActionButtonPressed);
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);        
        Monster monster = status == BATTLE.firstChoice ? p_Monster1 : (status == BATTLE.secondChoice ? p_Monster2 : null);
       
        printDialogues(new string[] { "What will " + monster.BaseState.Name + " do?" });
        yield return waitUntilDialogueComplete;
        
        EnableActionSelector();
        yield return waitUntilActionButtonPressed;
        ResetSelectors();
        isActionChoiceRoutineActive = false;
    }
    public void chooseAction(int actionIndex)
    {
        isActionButtonPressed = true;

        if (status == BATTLE.firstChoice)
            monsterOneAction.actionType = (ACTION.TYPE)actionIndex;
        else if (status == BATTLE.secondChoice)
            monsterTwoAction.actionType = (ACTION.TYPE)actionIndex;
    }

    public void chooseMove(int moveIndex)
    {
        isMoveButtonPressed = true;

        if (status == BATTLE.firstChoice)
            monsterOneAction.nextMove = p_Monster1.BaseState.Moves[moveIndex];
        else if (status == BATTLE.secondChoice)
            monsterTwoAction.nextMove = p_Monster2.BaseState.Moves[moveIndex];
    }
    public IEnumerator HandleActionRoutine()
    {
        isHandleActionRoutineActive = true;

        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialogueRoutineActive);
        WaitUntil waitUntilMoveButtonPressed = new WaitUntil(() => isMoveButtonPressed);

        Monster monster = status == BATTLE.firstChoice ? p_Monster1 : (status == BATTLE.secondChoice ? p_Monster2 : null);
        ACTION.TYPE action = status == BATTLE.firstChoice ? monsterOneAction.actionType : monsterTwoAction.actionType;
        
        if(action==ACTION.TYPE.fight)
        {
            printDialogues(new string[] { "Which attack will " + monster.BaseState.Name + " use?" });
            yield return waitUntilDialogueComplete;

            assignMovesToMoveButtons(monster);
            EnableMoveSelector();

            yield return waitUntilMoveButtonPressed;
            ResetSelectors();
        }
        else if(action == ACTION.TYPE.items)
        {
            //items shit
        }
        else if(action == ACTION.TYPE.party)
        {
            //party shit
        }
        else
        {
            //run
        }

        isHandleActionRoutineActive = false;
    }

    public void assignMovesToMoveButtons(Monster monster)
    {
        MoveBase[] monsterMoves = monster.BaseState.Moves;
        
        int counter = 0;
        foreach (Text buttonText in moveButtonText)
        {
            buttonText.text = monsterMoves[counter].Name;
            counter++;
        }
    }
    #endregion

    #region Dialogue functions
    public void printDialogues(string[] dialogues)
    {
        StartCoroutine(DialogueRoutine(dialogues));
    }
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
    #endregion

}


public enum BATTLE
{
    inactive,begun,firstChoice,secondChoice,execution,stateUpdate
}



public struct ACTION
{
    public enum TYPE
    {
        fight,items,party,run
    }
    
    public TYPE actionType;
    public bool runAway;
    public MoveBase nextMove;
    public Monster nextMonster;
}
