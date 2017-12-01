using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Animation2D/Manager")]
public class Animation2DManager : MonoBehaviour {

    private Animation2D[] animations;

    private void Start()
    {
        animations = GetComponents<Animation2D>();
    }

    public void Play(string animationName, bool reverse = false, bool loop = false, bool restart = false)
    {
        foreach (Animation2D anim in animations)
        {
            if (anim.animationName == animationName)
            {
                anim.Play(reverse, loop, restart);
            }
        }
    }

    public void Play(int animationID, bool reverse = false, bool loop = false, bool restart = false) {
        if (animations != null) {
            if (0 <= animationID && animationID < animations.Length) {
                animations[animationID].Play(reverse, loop, restart);
            }
        }
    }

    public void Pause(int animationID) {
        if (animations != null) {
            if (0 <= animationID && animationID < animations.Length) {
                animations[animationID].Pause();
            }
        }
    }

    public void Pause(string animationName)
    {
        foreach (Animation2D anim in animations)
        {
            if (anim.animationName == animationName)
            {
                anim.Pause();
            }
        }
    }

    public void Resume(string animationName)
    {
        foreach (Animation2D anim in animations)
        {
            if (anim.animationName == animationName)
            {
                anim.Resume();
            }
        }
    }

    public bool isPlaying(string animationName)
    {
        foreach (Animation2D anim in animations)
        {
            if (anim.animationName == animationName)
            {
                return anim.isPlaying;
            }
        }
        return false;
    }

    public bool isPlaying(int animationID) {
        if (animations != null) {
            if (0 <= animationID && animationID < animations.Length) {
                return animations[animationID].isPlaying;
            }
        }
        return false; 
    }

    public void StopAllAnimations() {
        if (animations != null) {
            foreach (Animation2D anim in animations) {
                anim.Pause();
            }
        }
    }
}
