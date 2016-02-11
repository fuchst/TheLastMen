using UnityEngine;
using System.Collections;

public class s_Tree : MonoBehaviour {
    [SerializeField]protected Collider groundCollider;
    [SerializeField]protected AudioClip soundHit;
    [SerializeField]protected GameObject soundDestroyPrefab;
    protected new AudioSource audio;

    [SerializeField]protected int health = 100;
    [SerializeField]protected int numberOfLogs = 1;
    [SerializeField]protected GameObject logPrefab;
    [SerializeField]protected GameObject leafParticlePrefab;

    protected bool destroyed = false;

    void Awake () {
        audio = GetComponent<AudioSource>();
    }

    public void Hit (int damage) {
        health -= damage;
        audio.clip = soundHit;
        audio.Play();
        if (health < 0 && !destroyed && logPrefab) {
            Burst();
        }
    }

    protected void Burst () {
        Instantiate(soundDestroyPrefab, transform.position, transform.rotation);

        s_GameManager.UpgradeSettings.UpgradeObject harvest = s_GameManager.Instance.upgradeSettings.upgrades[s_GameManager.UpgradeSettings.UpgradeTypes.ResourceHarvesting];
        destroyed = true;
        int tmp = 0;

        //have a chance on additional logs based on upgrades
        for(int i = 0; i < numberOfLogs; i++) {
            if (Random.value < harvest.progress_cur * harvest.stepSize) {
                tmp++;
            }
        }

        numberOfLogs += tmp;

        for(int i = 0; i < numberOfLogs; i++) {
            Instantiate(logPrefab, transform.position + (2.75f + 4.5f*i) * transform.up, Quaternion.identity);
        }
        GameObject leaves = Instantiate(leafParticlePrefab, transform.position + (10.0f * transform.localScale.x) * transform.up, Quaternion.identity) as GameObject;
        leaves.transform.localScale = Vector3.one * transform.localScale.x;
        leaves.transform.SetParent(transform.parent);
        gameObject.SetActive(false);
        /*HingeJoint hj = gameObject.AddComponent<HingeJoint>();
        hj.axis = transform.right;
        hj.breakForce = 40;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 50);
        groundCollider.enabled = false;
        gameObject.AddComponent<s_CentralGravity>();
        StartCoroutine(ShrinkTree());*/
        /*Collider[] cols = transform.GetComponentsInChildren<Collider>();
        foreach(Collider c in cols) {
            //c.enabled = false;
            Destroy(c.gameObject, 2.0f);
        }*/
        //gameObject.SetActive(false);
    }

    /*protected IEnumerator ShrinkTree () {
        float scale = transform.localScale.x;
        yield return new WaitForSeconds(1.0f);
        while (scale > 0.01f) {
            yield return new WaitForEndOfFrame();
            scale -= 0.33f * Time.deltaTime;
            transform.localScale = scale * Vector3.one;
        }
        gameObject.SetActive(false);
    }*/
}
