using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour {

	public static AudioManagerScript instance { get; private set; }

    public AudioSource music;
	public AudioSource soundFx;
	public AudioSource mirrorGoal;
    int mirrorPlaying = 0;
	public AudioSource mimicGoal;
    int mimicPlaying = 0;
    public bool isEffectsMuted;
    public bool isMuted {
        get; private set;
    }
		

    // Use this for initialization
    void Start () {
		isMuted = false;
		isEffectsMuted = false;
        music.Play();
        mimicGoal.Play();
        mirrorGoal.Play();
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
		mirrorGoal.mute = isEffectsMuted || mirrorPlaying <= 0;
		mimicGoal.mute = isEffectsMuted || mimicPlaying <= 0;
	}

	public void stopEffects() {
        while(mimicPlaying > 0) {
            removeMimicPlaying();
        }
        while(mirrorPlaying > 0) {
            removeMirrorPlaying();
        }
		//mirrorGoal.Stop ();
		//mimicGoal.Stop ();
		soundFx.Stop ();
	}

    public void addMimicPlaying() {
        mimicPlaying++;
        mimicGoal.mute = mimicPlaying <= 0 || isMuted;
    }

    public void removeMimicPlaying() {
        mimicPlaying--;
        mimicPlaying = Mathf.Max(mimicPlaying, 0);
        mimicGoal.mute = mimicPlaying <= 0 || isMuted;
    }

    public void addMirrorPlaying() {
        mirrorPlaying++;
        mirrorGoal.mute = mirrorPlaying <= 0 || isMuted;
    }

    public void removeMirrorPlaying() {
        mirrorPlaying--;
        mirrorPlaying = Mathf.Max(mirrorPlaying, 0);
        mirrorGoal.mute = mirrorPlaying <= 0 || isMuted;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
