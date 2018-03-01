using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement; // for restart function

// https://www.studica.com/blog/create-ui-unity-tutorial

using UnityEngine.UI; //Need this for calling UI scripts


public class UIManager : MonoBehaviour {

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    [SerializeField]
    Transform CutAndFillPanel; //Will assign our panel to this variable so we can enable/disable it

    [SerializeField]
    Transform DesignPanel; //Will assign our panel to this variable so we can enable/disable it

	[SerializeField]
	Transform ConfigPanel; //Will assign our panel to this variable so we can enable/disable it

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
        CutAndFillPanel.gameObject.SetActive(false);
        DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    void Update()
    {
        //If player presses escape and game is not paused. Pause game.
        //If game is paused and player presses escape, unpause.
        if (Input.GetKeyDown(KeyCode.Escape) && !UIPanel.gameObject.activeSelf)
            Pause();
        else if (Input.GetKeyDown(KeyCode.Escape) && UIPanel.gameObject.activeSelf)
            UnPause();
    }

    public void Depth()
    {
        Debug.Log("Depth Mode Clicked");
        ModeManager.dMode = DisplayMode.Depth;
		//Make sure windows from alternate modes don't display
		CutAndFillPanel.gameObject.SetActive(false);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void Design()
    {
        Debug.Log("Design Mode Clicked");

        ModeManager.dMode = DisplayMode.Design;

        DesignPanel.gameObject.SetActive(!DesignPanel.gameObject.activeSelf);
		CutAndFillPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void CutAndFill()
    {
        Debug.Log("Cut and Fill Mode Clicked");

        ModeManager.dMode = DisplayMode.CutFill;

        CutAndFillPanel.gameObject.SetActive(!CutAndFillPanel.gameObject.activeSelf);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void Calibrate()
    {
        Debug.Log("Calibrate Mode Clicked");
        ModeManager.dMode = DisplayMode.Calibrate;

		//Make sure windows from alternate modes don't display
		CutAndFillPanel.gameObject.SetActive(false);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (true);
    }

    public void Pause()
    {
        UIPanel.gameObject.SetActive(true); //turn on the pause menu
    }

    public void UnPause()
    {
        UIPanel.gameObject.SetActive(false); //turn off pause menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
