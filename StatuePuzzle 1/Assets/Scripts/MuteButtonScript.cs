using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class MuteButtonScript : MonoBehaviour {

    public Sprite SoundOn;
    public Sprite SoundOff;
    Image i;

	// Use this for initialization
	void Start () {
        i = this.GetComponent<Image>();
        i.sprite = (AudioManagerScript.instance.isMuted ? SoundOff : SoundOn);
	}

    public void toggleMute() {
		AudioManagerScript.instance.toggleMute();
		i.sprite = (AudioManagerScript.instance.isMuted ? SoundOff : SoundOn);
    }
}
