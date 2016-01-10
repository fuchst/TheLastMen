using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Combat_backup : MonoBehaviour {
    
    public float bulletSpeed;
    public float bulletGravityModifier;
    public int damage = 20;
    private float bulletGravityStrength;

    GameObject bulletPrefab;

    void Awake()
    {
        bulletPrefab = Resources.Load("Bullet") as GameObject;
        bulletGravityStrength = bulletGravityModifier * Physics.gravity.magnitude;
    }

    void OnHit(int dmg)
    {
        s_GameManager.Instance.healthpointsCur -= dmg;
        if (s_GameManager.Instance.healthpointsCur <= 0)
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
            Bullet_backup b = bullet.GetComponent<Bullet_backup>();
            b.gravity = bulletGravityStrength;
            b.damage = damage;
            bullet.GetComponent<Rigidbody>().AddForce(cam.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}
