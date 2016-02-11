using UnityEngine;
using System.Collections;

public class s_Survivor : s_Collectible {

    public Transform survivorHead;
    public AnimationClip animationOnCollect;
    protected Transform playerTransform;
    protected bool collected = false;

    void Start () {
        playerTransform = LevelManager.Instance.player.transform;
        audio = GetComponent<AudioSource>();
    }

    void Update () {
        if(!collected && Vector3.Distance(playerTransform.position, transform.position) < 15.0f) {
            LookAtPlayer();
        }
    }

    protected void LookAtPlayer () {
        //survivorHead.localRotation = Quaternion.LookRotation(playerTransform.position - survivorHead.position, transform.up);
        transform.LookAt(playerTransform, transform.position.normalized);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    protected override void Collect () {
        collected = true;
        s_GameManager.Instance.survivorsCur++;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Health);
        s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Survivor);
        Bastion.Instance.UpdateSurvivors();
        s_GameManager.Instance.ConsumeEnergy(5.0f);

        Animation anim = GetComponent<Animation>();
        anim.clip = animationOnCollect;
        anim.wrapMode = WrapMode.Once;
        anim.Play();
        if (collectSounds.Count > 0) {
            audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
            audio.Play();
        }
        if (collectParticleEffect) {
            Instantiate(collectParticleEffect, transform.position, transform.rotation);
        }
        StartCoroutine(ShrinkAndLevitate());
    }

    protected IEnumerator ShrinkAndLevitate () {
        yield return new WaitForSeconds(2.0f);
        Vector3 initialScale = transform.localScale;
        float lerpParameter = 0;
        while (enabled) {
            yield return new WaitForEndOfFrame();
            lerpParameter += Time.deltaTime / (destructionDelay - 2.0f);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, lerpParameter * lerpParameter);
            transform.Translate(transform.up * 0.5f * Time.deltaTime);
            transform.Rotate(Vector3.up, 500.0f * lerpParameter * Time.deltaTime, Space.Self);
        }
    }
}
