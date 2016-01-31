using UnityEngine;
using System.Collections.Generic;

public class RopeAudioPlayer : MonoBehaviour {
    [SerializeField]
    protected List<AudioClip> ropeSounds;
    protected new AudioSource audio;
    protected bool hooked = false;
    // Use this for initialization
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hooked && !audio.isPlaying)
        {
            audio.Play();
        }
        else if (audio.isPlaying && !hooked)
        {
            audio.Stop();
        }
    }
    public void UpdateRopeState(bool hooked)
    {
        this.hooked = hooked;
    }
}
