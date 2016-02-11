using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class RopeAudioPlayer : MonoBehaviour {
    [SerializeField]
    protected List<AudioClip> ropeSounds;
    protected new AudioSource audio;
    protected bool hooked = false;
    protected RigidbodyFirstPersonControllerSpherical controller;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        controller = transform.parent.gameObject.GetComponent<RigidbodyFirstPersonControllerSpherical>();
        audio.volume = 0.5f;
    }

    void Update () {
        if (s_GameManager.Instance.gamePaused && audio.isPlaying) {
            audio.Stop();
            return;
        }

        audio.volume = Mathf.Min(0.9f, 0.075f * controller.Velocity.magnitude);
        if (hooked && !audio.isPlaying)
        {
            audio.clip = ropeSounds[Random.Range(0, ropeSounds.Count)];
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
