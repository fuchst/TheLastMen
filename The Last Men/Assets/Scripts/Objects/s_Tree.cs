using UnityEngine;
using System.Collections;

public class s_Tree : MonoBehaviour {
    [SerializeField]protected Collider groundCollider;

    [SerializeField]protected int health = 100;
    [SerializeField]protected int numberOfLogs = 1;
    [SerializeField]protected GameObject logPrefab;
    [SerializeField]protected GameObject leafParticlePrefab;

    protected bool destroyed = false;

    public void Hit (int damage) {
        health -= damage;
        if (health < 0 && !destroyed && logPrefab) {
            Burst();
        }
    }

    protected void Burst () {
        destroyed = true;
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
