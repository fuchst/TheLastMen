using UnityEngine;
using System.Collections.Generic;
//using UnityStandardAssets.Characters.FirstPerson;

public class AudioPlayer : MonoBehaviour {

    [SerializeField]protected List<AudioClip> footstepSounds;
    protected bool walking = false;
    protected float speed = 0.0f;
    protected new AudioSource audio;

    void Awake () {
        audio = GetComponent<AudioSource>();
        audio.volume = 0.5f;
    }
	
	void Update () {
        if(walking && !audio.isPlaying) {
            audio.clip = footstepSounds[Random.Range(0, footstepSounds.Count)];
            audio.Play();
        }
        /*else if(!walking && audio.isPlaying) {
            audio.Stop();
        }*/
    }

    public void UpdateWalkingState (bool walking, float curSpeed = 0) {
        this.walking = walking;
        //Debug.Log(curSpeed);
        speed = curSpeed > 2 ? curSpeed-1 : 1;
        speed = 0.33f + 0.65f * Mathf.Log(speed);
        audio.pitch = speed;
        Debug.Log(speed);
    }
}
