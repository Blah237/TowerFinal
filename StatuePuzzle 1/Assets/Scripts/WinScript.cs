using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScript : MonoBehaviour
{

	public static bool playerWin;
	//public float restartDelay = 5f;
	public GameObject NextLevel;
	public GameObject LevelSelectWin;
	public GameObject RestartWin;

	private Animator anim;
	//private float restartTimer;
	
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerWin)
		{
			anim.SetTrigger("Win");
			PlayerPrefs.SetInt (GameManagerScript.levelName, 1); //Set to 1 to indicate a win
			NextLevel.GetComponent<Button>().interactable = true;
			LevelSelectWin.GetComponent<Button>().interactable = true;
			RestartWin.GetComponent<Button>().interactable = true;
			RestartWin.GetComponent<Image>().raycastTarget = true;
			
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
