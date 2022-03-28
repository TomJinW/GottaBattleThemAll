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

    [SerializeField] GameObject actionSelection;
    [SerializeField] GameObject moveSelection;


    #region Monster Variables
    List<Monster> party;
    Monster p_Monster1;
    Monster p_Monster2;
    Monster o_Monster1;
    Monster o_Monster2;
    #endregion

    #region Routine Controllers
    private bool isDialoguesActive = false;
    private bool isActionChoiceEnabled = false;
    private int actionIndex;
    private Coroutine ChoiceRoutineController = null;
    #endregion

    [Header("Battle State")]
    [SerializeField] private BattleState status;

    private void OnValidate()
    {
        status = BattleState.inactive;
    }

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
        actionSelection.SetActive(false);
        moveSelection.SetActive(false);
        #endregion

        //begin battle
        StartCoroutine(BattleRoutine());
    }

    public IEnumerator BattleRoutine()
    {
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialoguesActive);
        WaitUntil waitUntilActionChoiceComplete = new WaitUntil(() => ChoiceRoutineController == null);

        status = BattleState.begun;
        printDialogues(new string[] { "A battle has begun!"});
        yield return waitUntilDialogueComplete;
        
        while (isBattleActive)
        {
            ChoiceRoutineController = StartCoroutine(ChoiceRoutine(1));
            yield return waitUntilActionChoiceComplete;
            
            if(this.actionIndex==0)
            {
                printDialogues(new string[] { "What will " + p_Monster1.getName() + "do?" });
            }
        }

    }

    public IEnumerator ChoiceRoutine(int choiceIndex)
    {
        WaitUntil waitUntilActionButtonPressed = new WaitUntil(() => !isActionChoiceEnabled);
        WaitUntil waitUntilDialogueComplete = new WaitUntil(() => !isDialoguesActive);

        status = choiceIndex == 1 ? BattleState.firstChoice : BattleState.secondChoice;
        Monster monster = choiceIndex == 1 ? p_Monster1 : p_Monster2;
        
        printDialogues(new string[] { "What will " + monster.getName() + "do?" });
        yield return waitUntilDialogueComplete;
        actionSelection.SetActive(true);
        
        isActionChoiceEnabled = true;
        yield return waitUntilActionButtonPressed;

        actionSelection.SetActive(false);
        ChoiceRoutineController = null;
    }

    public void setUnitUIElements()
    {
        //Setting Sprites
        playerUnit.setSprites(p_Monster1.getSprite(), p_Monster2.getSprite());
        opponentUnit.setSprites(o_Monster1.getSprite(), o_Monster2.getSprite());

        //Setting HP
        playerUnit.setHPs(p_Monster1.getNormalizedHP(), p_Monster2.getNormalizedHP());
        opponentUnit.setHPs(o_Monster1.getNormalizedHP(), o_Monster2.getNormalizedHP());

        //Setting Names
        playerUnit.setNames(p_Monster1.getName(), p_Monster2.getName());
        opponentUnit.setNames(o_Monster1.getName(), o_Monster2.getName());
    }
    public void printDialogues(string[] dialogues)
    {
        StartCoroutine(DialogueRoutine(dialogues));
    }
    public IEnumerator DialogueRoutine(string[] dialogues)
    {
        isDialoguesActive = true;
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
        isDialoguesActive = false;
    }
    
    public void chooseAction(int actionIndex)
    {
        this.actionIndex = actionIndex;
        isActionChoiceEnabled = false;
    }

    
}


public enum BattleState
{
    inactive,begun,firstChoice,secondChoice,execution,stateUpdate
}

