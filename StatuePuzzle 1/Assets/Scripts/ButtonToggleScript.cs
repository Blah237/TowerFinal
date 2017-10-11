using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{

	[SerializeField]
	private Sprite onSprite;
	[SerializeField]
	private Sprite offSprite;

    //a reference to the laser that this button controls
	public Laser laser;

	// Use this for initialization
	void Start () {

	}

	public void TogglePressed () {
        laser.gameObject.SetActive(!laser.gameObject.activeInHierarchy);
        if (laser.gameObject.activeInHierarchy){
			GetComponent<SpriteRenderer>().sprite = onSprite;

		} else {
			GetComponent<SpriteRenderer>().sprite = offSprite;
		}
    }

    public void InitButton () {
        if (laser.state == 1) {
            GetComponent<SpriteRenderer>().sprite = onSprite;
        }
        else {
            GetComponent<SpriteRenderer>().sprite = offSprite;
        }
    }


}
