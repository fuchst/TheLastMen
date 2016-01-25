using UnityEngine;

public class Pistol : Weapon
{
    public Pistol()
    {
        dmg = 20;
        magSize = 12;
        magFill = magSize;
        fireInterval = 0.25f;
        fireCounter = 0;
    }

    public override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void shoot(Transform frame)
    {
        GameObject bullet = Object.Instantiate(Weapon.bulletPrefab, frame.position + frame.forward, frame.rotation) as GameObject;
        bullet.GetComponent<Bullet>().gravity = bulletGravity;
        bullet.GetComponent<Bullet>().damage = dmg;
        bullet.GetComponent<Rigidbody>().AddForce(frame.forward * bulletSpeed, ForceMode.Impulse);
    }

    public override string WeaponName() {
        return "Pistol";
    }
}
