using UnityEngine;
using System.Collections.Generic;

public class JetPackAudioPlayer : MonoBehaviour {
    [SerializeField]protected List<AudioClip> jetpackSounds;
    protected new AudioSource audio;
    protected bool jetpacking = false;
    protected float jetpackForce = 0.0f;

	void Start () {
        audio = GetComponent<AudioSource>();
        audio.volume = 0.0f;
	}
	
    void Update () {
        if (s_GameManager.Instance.gamePaused && audio.isPlaying) {
            audio.Stop();
            return;
        }

        audio.volume = Mathf.Lerp(audio.volume, jetpacking ? (0.15f + 6 * jetpackForce) : 0.0f, 7.5f * Time.deltaTime);
        if (!audio.isPlaying) {
            audio.clip = jetpackSounds[Random.Range(0, jetpackSounds.Count)];
            audio.Play();
        }
    }

    public void UpdateJetpackState (bool jetpacking) {
        this.jetpacking = jetpacking;
        if (!jetpacking) {
            jetpackForce = 0.0f;
        }
    }

    public void UpdateJetpackForce (float force) {
        jetpackForce = force;
    }
}
