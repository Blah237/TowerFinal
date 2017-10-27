using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour {

    public AudioClip music;
    public AudioSource audio;

    static AudioManagerScript instance;

    // Use this for initialization
    void Start () {
        audio = this.GetComponent(typeof(AudioSource)) as AudioSource;
        audio.clip = music;
        audio.Play();
    }

    private void Awake() {
        if (instance != null) {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        if (instance == null) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject); // Prevent the logging manager been destroyed accidentally.
        }
    }

        // Update is called once per frame
        void Update () {
		
	}
}
