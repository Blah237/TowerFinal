using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDown2 : MonoBehaviour {

	private Animator anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponentInParent<Animator>();
	}
	
	public void ScrollDown()
	{
		anim.SetTrigger("ScrollDown2");
	}
}
