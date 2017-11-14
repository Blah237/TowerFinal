using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour {

	public static AudioManagerScript instance { get; private set; }

    public AudioSource music;
	public AudioSource soundFx;
	public AudioSource mirrorGoal;
	public AudioSource mimicGoal;

    public bool isMuted {
        get; private set;
    }

    // Use this for initialization
    void Start () {
		isMuted = false;
        music.Play();
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

    public void toggleMute() {

		// Log mute or unmute
		if (LoggingManager.instance != null)
		{
			if (LoggingManager.instance.GetLevelStarted () == true) {
				if (isMuted) {
					LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.UNMUTE, "Unmute");
				} else {
					LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.MUTE, "Mute");
				}
			}
		}

        isMuted = !isMuted;
        music.mute = isMuted;
        soundFx.mute = isMuted;
        mirrorGoal.mute = isMuted;
        mimicGoal.mute = isMuted;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
