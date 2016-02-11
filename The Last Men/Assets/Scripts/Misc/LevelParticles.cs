using UnityEngine;
using System.Collections;

public class LevelParticles : MonoBehaviour {
    private ParticleSystem particles;
    private float levelRadius;
    private float blackHoleRadius;
    private float time;

	// Use this for initialization
	void Start () {
        particles = GetComponent<ParticleSystem>();
        levelRadius = LevelManager.Instance.levelVariables[LevelManager.Instance.CurLvl].radius;
        blackHoleRadius = 0.5f * LevelManager.Instance.BlackHole.transform.localScale.x;
        time = s_GameManager.Instance.roundDuration;
        Destroy(gameObject, time);
	}
	
	// Update is called once per frame
	void Update () {
        float curScale = Mathf.Lerp(blackHoleRadius, levelRadius, s_GameManager.Instance.RemainingTime / time) + 5.0f;
        transform.localScale = curScale * Vector3.one;
        //transform.Rotate(Vector3.up, 10.0f * Time.deltaTime, Space.World);
        float newParticleLifetime = (curScale - blackHoleRadius) / -particles.startSpeed;

        if (newParticleLifetime > 0.5f) {
            particles.enableEmission = true;
            particles.startLifetime = newParticleLifetime;
        }
        else {
            particles.enableEmission = false;
        }

        //lifetime * speed = particleDistance = emitterRadius - blackHoleRadius
        //lifetime = (emitterRadius - blackHoleRadius)/speed
        //internal ParticleSystem emitterRadius is set to 1.0f on prefab
        //effective emitterRadius thus is 1.0f * scale = scale
    }
}
