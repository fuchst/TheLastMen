using UnityEngine;
using System.Collections;

public class BlackHoleParticles : MonoBehaviour {
    private ParticleSystem particles;

    // Use this for initialization
    void Start () {
        particles = GetComponent<ParticleSystem>();
        //particles.startSize = 1.15f * LevelManager.Instance.BlackHole.transform.localScale.x;
    }

    // Update is called once per frame
    void Update () {
        particles.startSize = 15.0f + LevelManager.Instance.BlackHole.transform.localScale.x;
    }
}
