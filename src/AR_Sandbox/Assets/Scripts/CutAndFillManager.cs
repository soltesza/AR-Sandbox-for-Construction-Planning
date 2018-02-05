using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.studica.com/blog/create-ui-unity-tutorial

using UnityEngine.UI; //Need this for calling UI scripts

public class CutAndFillManager : MonoBehaviour {

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    bool isPaused; //Used to determine paused state

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
        //isPaused = false; //make sure isPaused is always false when our scene opens
    }

    void Update()
    {
    /*
        //If player presses escape and game is not paused. Pause game. If game is paused and player presses escape, unpause.
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
            Pause();
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
            UnPause();
    */
    }

    public void Pause()
    {
        //isPaused = true;
        UIPanel.gameObject.SetActive(true); //turn on the pause menu
    }

    public void UnPause()
    {
        //isPaused = false;
        UIPanel.gameObject.SetActive(false); //turn off pause menu
    }
}
