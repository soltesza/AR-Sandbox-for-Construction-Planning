using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement; // For restart function

using UnityEngine.UI; //Need this for calling UI scripts


public class UIManager : MonoBehaviour {

    [SerializeField]
    Transform UIPanel; 

    [SerializeField]
    Transform CutAndFillPanel; 

    [SerializeField]
    Transform DesignPanel; 

	[SerializeField]
	Transform ConfigPanel;

    void Start()
    {
		//Make sure all UI panels are disabled at the start
        UIPanel.gameObject.SetActive(false); 
        CutAndFillPanel.gameObject.SetActive(false);
        DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    void Update()
    {
        //General keyboard shortcut catcher and handlers
        if (Input.GetKeyDown(KeyCode.Escape) && !UIPanel.gameObject.activeSelf)
            Pause();
        else if (Input.GetKeyDown(KeyCode.Escape) && UIPanel.gameObject.activeSelf)
            UnPause();
		if (Input.GetKeyDown(KeyCode.Q))
			Depth();
		if (Input.GetKeyDown(KeyCode.W))
			Design();
		if (Input.GetKeyDown(KeyCode.E))
			CutAndFill();
		if (Input.GetKeyDown(KeyCode.R))
			Calibrate();
    }

    public void Depth()
    {
        ModeManager.dMode = DisplayMode.Depth;
		//Make sure windows from alternate modes don't display
		CutAndFillPanel.gameObject.SetActive(false);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void Design()
    {
        ModeManager.dMode = DisplayMode.Design;

		//Make sure windows from alternate modes don't display
        DesignPanel.gameObject.SetActive(!DesignPanel.gameObject.activeSelf);
		CutAndFillPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void CutAndFill()
    {
        ModeManager.dMode = DisplayMode.CutFill;

		//Make sure windows from alternate modes don't display
        CutAndFillPanel.gameObject.SetActive(!CutAndFillPanel.gameObject.activeSelf);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (false);
    }

    public void Calibrate()
    {
        ModeManager.dMode = DisplayMode.Calibrate;

		//Make sure windows from alternate modes don't display
		CutAndFillPanel.gameObject.SetActive(false);
		DesignPanel.gameObject.SetActive(false);
		ConfigPanel.gameObject.SetActive (true);
    }

    public void Help()
    {
        // Open project GitHub webpage
        System.Diagnostics.Process.Start("https://github.com/soltesza/AR-Sandbox-for-Construction-Planning/");
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
