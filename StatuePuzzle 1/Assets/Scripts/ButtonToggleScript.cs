using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{

	[SerializeField]
	private Material onMaterial;
	[SerializeField]
	private Material offMaterial;
	
    //a reference to the laser that this button controls 
	public Laser laser;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void TogglePressed () {
        laser.gameObject.SetActive(!laser.gameObject.activeInHierarchy);
        if (laser.gameObject.activeInHierarchy){
			GetComponent<MeshRenderer>().material = onMaterial;
            
		} else {
			GetComponent<MeshRenderer>().material = offMaterial;
		}
    }

    public void InitButton () {
        if (laser.state == 1) {
            GetComponent<MeshRenderer>().material = onMaterial;
        }
        else {
            GetComponent<MeshRenderer>().material = offMaterial;
        }
    }
	
	
}
