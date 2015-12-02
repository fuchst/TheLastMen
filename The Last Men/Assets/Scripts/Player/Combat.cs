using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Combat : MonoBehaviour {

    public int hp = 100;
    public float bulletSpeed;
    public float bulletGravity;
    public static int damage = 20;

    GameObject bulletPrefab;

    void Awake()
    {
        bulletPrefab = Resources.Load("Bullet") as GameObject;
    }

    void OnHit(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
             Debug.Log("Player died");
        }
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Shoot"))
        {
            Transform cam = this.transform.GetChild(0);

            GameObject bullet = Instantiate(bulletPrefab, cam.position + cam.forward, cam.rotation) as GameObject;
            bullet.GetComponent<Bullet>().gravity = bulletGravity;
            bullet.GetComponent<Rigidbody>().AddForce(cam.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}
