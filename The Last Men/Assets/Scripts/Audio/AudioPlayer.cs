using UnityEngine;
using System.Collections.Generic;
//using UnityStandardAssets.Characters.FirstPerson;

public class AudioPlayer : MonoBehaviour {

    [SerializeField]protected List<AudioClip> footstepSounds;
    public AudioClip jetpackSound;
    protected bool walking = false;
    protected bool jetpacking = false;
    protected float speed = 0.0f;
    protected new AudioSource audio;
    private new JetPackAudioPlayer jetpackAudio;
    private new RopeAudioPlayer ropeAudio;

    void Awake () {
        audio = GetComponent<AudioSource>();
        audio.volume = 0.5f;
        jetpackAudio = GetComponentInChildren<JetPackAudioPlayer>();
        ropeAudio = GetComponentInChildren<RopeAudioPlayer>();
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
        //Debug.Log(speed);
    }

    public void UpdateJetpackState( bool jetpacking)
    {
        jetpackAudio.UpdateJetpackState(jetpacking);
    }

    public void UpdateHookState(bool hooked)
    {
        ropeAudio.UpdateRopeState(hooked);
    }
}
