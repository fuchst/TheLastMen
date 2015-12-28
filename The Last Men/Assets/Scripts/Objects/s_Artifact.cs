using UnityEngine;
using System.Collections;

public class s_Artifact : s_Collectible {
    
    [Range(0, 2)] public int artifactType;

    void Awake () {
        if(0 == artifactType) {
            artifactType = Random.Range(1, 3); //3 is exclusive to the range for int!
        }
    }

	protected override void Collect () {
        if(artifactType == 1)
            s_GameManager.Instance.artifact1CountCur++;
        else if(artifactType == 2)
            s_GameManager.Instance.artifact2CountCur++;
    }
}
