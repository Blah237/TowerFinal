using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScript : MonoBehaviour
{

	public static bool playerWin;
	//public float restartDelay = 5f;
	private GameObject NextLevel;
	private GameObject LevelSelectWin;

	private Animator anim;
	//private float restartTimer;
	
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		NextLevel = GameObject.Find("Next Level");
		LevelSelectWin = GameObject.Find("Button");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerWin)
		{
			anim.SetTrigger("Win");
			CreateLevelSelect.buttonMap[GameManagerScript.levelName] = true;
			NextLevel.GetComponent<Button>().interactable = true;
			LevelSelectWin.GetComponent<Button>().interactable = true;
			//playerWin = false;
			//b.GetComponent<Image>().color = Color.green;
			//GameManagerScript.inputReady = false;
            //restartTimer += Time.deltaTime;

            //if (restartTimer >= restartDelay) {
            //SceneManager.LoadScene(0);
            //}
            if (Input.GetKeyDown(KeyCode.Return)) {
                new NextLevelScript().LoadScene(); 
            }
		}
	}
}
