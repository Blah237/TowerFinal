using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{

	public bool pressed;
	[SerializeField]
	private Material onMaterial;
	[SerializeField]
	private Material offMaterial;
	
	//TODO: Fix button to laser framework
	private Laser laser;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pressed){
			GetComponent<MeshRenderer>().material = offMaterial;
			//TODO: Turn laser on.
		} else {
			GetComponent<MeshRenderer>().material = onMaterial;
			//TODO: Turn laser off.
		}
	
	}
	
	
}
