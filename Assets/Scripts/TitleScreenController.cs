using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

 
public class TitleScreenController : MonoBehaviour
{
    public GameObject anchor;
    public TitleScreenOptions currentSelectedOption = TitleScreenOptions.Start;
    private bool optionMenuActivated = false;
    public GameObject optionMenu;
    public void setOption()
    {
        anchor.transform.localPosition = Constants.titleScreenOptionPositions[(int)currentSelectedOption];
    }
    public void setNextOption()
    {
        currentSelectedOption = currentSelectedOption.Next();
        setOption();
    }
    public void setPreviousOption()
    {
        currentSelectedOption = currentSelectedOption.Previous();
        setOption();
    }
    public void processSelection()
    {
        switch (currentSelectedOption)
        {
            case TitleScreenOptions.Start:
                SceneManager.LoadScene("MapScene");
                break;
            case TitleScreenOptions.Option:
                optionMenuActivated = !optionMenuActivated;
                optionMenu.SetActive(optionMenuActivated);
                break;
            case TitleScreenOptions.Quit:
                Application.Quit();
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            setNextOption();
        }
        if (Input.GetKeyDown("w"))
        {
            setPreviousOption();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            processSelection();
        }
    }
}
