using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private int [] mainPointerIndex = { 0,0 };
    private int [] mainMenuSize = {7,7};

    [SerializeField]
    GameObject [] mainMenuOptions;
    [SerializeField]
    GameObject[] flyMenuOptions;
    [SerializeField]
    GameObject mainMenuOptionSelector;
    [SerializeField]
    GameObject flyMenuOptionSelector;
    [SerializeField]
    GameObject[] windowGroups;
    [SerializeField]
    TeleportPoint teleporter;
    void processSelection() {
        // Fly Selection


        // Main Selection
        if (mainPointerIndex[0] == 0)
        {
            if (mainPointerIndex[1] == 0)
            {
                mainPointerIndex[0] = 1;
                windowGroups[1].SetActive(!windowGroups[1].activeSelf);
                setPointerPosition();
            }
            if (mainPointerIndex[1] >= 1 && mainPointerIndex[1] <= 4)
            {
                windowGroups[0].SetActive(!windowGroups[0].activeSelf);
            }
            if (mainPointerIndex[1] == 5)
            {
                Internals.allowMapMovement = true;
                Time.timeScale = 1;
                Internals.teleported = false;
                SceneManager.LoadScene("TitleScreen");
            }
            if (mainPointerIndex[1] == 6)
            {
                Internals.allowMapMovement = true;
                Time.timeScale = 1;
                this.gameObject.SetActive(false);
            }
        }
        else
        {

            switch (mainPointerIndex[1]) {
                case 0:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(-11f, -4.85f), "MapScene");
                    break;
                case 1:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(-9f, 9.5f), "MapScene");
                    break;
                case 2:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(-36.27f, 27.7f), "MapSceneVolcano");
                    break;
                case 3:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(-24.5f, 25), "MapSceneVolcano");
                    break;
                case 4:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(-24.61f, 27.26f), "MapSceneSnow");
                    break;
                case 5:
                    GetComponent<CanvasGroup>().alpha = 0;
                    teleporter.TeleportTo(new Vector2(4.27f, 32.23f), "MapSceneSnow");
                    break;
                case 6:
                    mainPointerIndex[0] = 0;
                    mainPointerIndex[1] = 0;
                    windowGroups[1].SetActive(!windowGroups[1].activeSelf);
                    setPointerPosition();
                    break;

            }
        }

    }
    void setPointerPosition() {
        //Debug.Log(mainPointerIndex);
        switch (mainPointerIndex[0]) {
            case 0:
                mainMenuOptionSelector.transform.position =
                mainMenuOptions[mainPointerIndex[1]].transform.position;
                mainMenuOptionSelector.transform.position = new Vector2(
                    mainMenuOptionSelector.transform.position.x - 50,
                    mainMenuOptionSelector.transform.position.y);
                break;
            case 1:
                flyMenuOptionSelector.transform.position =
flyMenuOptions[mainPointerIndex[1]].transform.position;
                flyMenuOptionSelector.transform.position = new Vector2(
                    flyMenuOptionSelector.transform.position.x - 108,
                    flyMenuOptionSelector.transform.position.y);
                break;
            default:
                break;
                
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        setPointerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            mainPointerIndex[1] = mainPointerIndex[1].Next(mainMenuSize[mainPointerIndex[0]]);
            setPointerPosition();
        }
        if (Input.GetKeyDown("w"))
        {
            mainPointerIndex[1] = mainPointerIndex[1].Previous(mainMenuSize[mainPointerIndex[0]]);
            setPointerPosition();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            processSelection();
        }
    }
}
