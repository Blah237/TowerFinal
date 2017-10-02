using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{

	public bool playerWin;
	public float restartDelay = 5f;

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
		if (playerWin) {
			anim.SetTrigger("Win");
			GameManagerScript.inputReady = false;
			//restartTimer += Time.deltaTime;

			//if (restartTimer >= restartDelay) {
			//SceneManager.LoadScene(0);
			//}

		}
	}
}
