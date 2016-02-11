using UnityEngine;
using System.Collections.Generic;

public class s_Artifact : s_Collectible {
    [SerializeField]protected Material materialType1;
    [SerializeField]protected Material materialType2;
    [SerializeField]protected List<ParticleEmitter> artifactBeaconParticles = new List<ParticleEmitter>();
    [SerializeField]protected List<Light> artifactBeaconLights = new List<Light>();

    [Tooltip("if set to 0, it will be set to 1 or 2 later by script")] [Range(0, 2)] public int artifactType;

    void Awake () {
        audio = GetComponent<AudioSource>();
        if (0 == artifactType) {
            artifactType = Random.Range(1, 3); //3 is exclusive to the range for int!
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers) {
                r.material = 1 == artifactType ? materialType1 : materialType2;
            }
        }
    }

	protected override void Collect () {
        if (artifactType == 1) {
            s_GameManager.Instance.artifact1CountCur++;
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Artifact1);
        }
        else if (artifactType == 2) {
            s_GameManager.Instance.artifact2CountCur++;
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Artifact2);
        }
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Artifact);
        if (collectSounds.Count > 0) {
            audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
            audio.Play();
        }
        foreach (ParticleEmitter beacon in artifactBeaconParticles) {
            beacon.Emit(100);
            beacon.emit = false;
        }

        foreach (Light light in artifactBeaconLights) {
            //light.enabled = false;
            Destroy(light, 2.0f);
        }
    }
}
