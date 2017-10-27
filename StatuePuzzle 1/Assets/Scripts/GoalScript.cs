using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour {
    [SerializeField]
    ParticleSystem win;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color winColor; 

    public bool isWin = false; 

    public void ToggleParticles() {
        if (isWin && !win.isPlaying) {
            win.Play();
            GetComponent<SpriteRenderer>().color = winColor; 
        } else if (!isWin && win.isPlaying) {
            win.Stop();
            GetComponent<SpriteRenderer>().color = normalColor; 
        }
    }
}
