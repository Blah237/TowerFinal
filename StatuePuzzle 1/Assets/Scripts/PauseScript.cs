using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{

	public bool paused = false;
	public bool unpaused;
	private Animator anim;
	private GameObject LevelSelectPause;
	private GameObject RestartPause;
	
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		unpaused = true;
		LevelSelectPause = GameObject.Find("LevelSelectPause");
		RestartPause = GameObject.Find("RestartLevelPause");
	}

	public void TogglePause ()
	{
		paused = !paused;
		unpaused = !unpaused;
		if (paused)
		{
			LevelSelectPause.GetComponent<Image>().raycastTarget = true;
			LevelSelectPause.GetComponent<Button>().interactable = true;
			RestartPause.GetComponent<Image>().raycastTarget = true;
			RestartPause.GetComponent<Button>().interactable = true;
			anim.SetBool("Unpaused", false);
			Debug.Log("Game Paused");
			anim.SetTrigger("Paused");
			GameManagerScript.inputReady = false;
		} else if (unpaused)
		{
			anim.SetBool("Paused", false);
			anim.SetTrigger("Unpaused");
			anim.SetTrigger("BackToStart");
			LevelSelectPause.GetComponent<Button>().interactable = false;
			LevelSelectPause.GetComponent<Image>().raycastTarget = false;
			RestartPause.GetComponent<Button>().interactable = false;
			RestartPause.GetComponent<Image>().raycastTarget = false;
			GameManagerScript.inputReady = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
