using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour {

	public static AudioManagerScript instance { get; private set; }

    public AudioSource music;
	public AudioSource soundFx;
	public AudioSource mirrorGoal;
	public AudioSource mimicGoal;
	public bool isEffectsMuted;
    public bool isMuted {
        get; private set;
    }
		

    // Use this for initialization
    void Start () {
		isMuted = false;
		isEffectsMuted = false;
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

		toggleEffectsMute ();
        isMuted = !isMuted;
        music.mute = isMuted;
    }

	public void toggleEffectsMute() {
		isEffectsMuted = !isEffectsMuted;
		soundFx.mute = isEffectsMuted;
		mirrorGoal.mute = isEffectsMuted;
		mimicGoal.mute = isEffectsMuted;
	}

	public void stopEffects() {
		mirrorGoal.Stop ();
		mimicGoal.Stop ();
		soundFx.Stop ();
	}

    // Update is called once per frame
    void Update () {
		
	}
}
