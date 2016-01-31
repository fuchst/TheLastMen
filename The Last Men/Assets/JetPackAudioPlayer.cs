using UnityEngine;
using System.Collections.Generic;

public class JetPackAudioPlayer : MonoBehaviour {
    [SerializeField]protected List<AudioClip> jetpackSounds;
    protected new AudioSource audio;
    protected bool jetpacking = false;
	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        audio.volume = 0.5f;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (jetpacking && !audio.isPlaying)
        {
            audio.clip = jetpackSounds[Random.Range(0, jetpackSounds.Count)];
            audio.Play();
        }
        else if(audio.isPlaying && !jetpacking)
        {
            audio.Stop();
        }
    }
    public void UpdateJetpackState(bool jetpacking)
    {
        this.jetpacking = jetpacking;
    }
}
