using UnityEngine;

public class Pistol : Weapon
{
    protected new AudioSource audio;
    protected s_GameManager.UpgradeSettings.UpgradeObject pistolUpgradeObj;

    public Pistol()
    {
        dmg = 12;
        magSize = 12;
        magFill = magSize;
        fireInterval = 0.35f;
        fireCounter = 0;
    }

    public override void OnAwake()
    {
        audio = GetComponent<AudioSource>();
        base.OnAwake();
    }

    void Start () {
        pistolUpgradeObj = s_GameManager.Instance.upgradeSettings.upgrades[s_GameManager.UpgradeSettings.UpgradeTypes.PistolDamage];
    }

    protected override void shoot(Transform frame) {
        GameObject bullet = Object.Instantiate(Weapon.bulletPrefab, frame.position + frame.forward, frame.rotation) as GameObject;
        bullet.GetComponent<Bullet>().gravity = bulletGravity;
        bullet.GetComponent<Bullet>().damage = Damage();
        bullet.GetComponent<Rigidbody>().AddForce(frame.forward * bulletSpeed, ForceMode.Impulse);
        audio.Play();
    }

    protected override int Damage () {
        return dmg + dmg + (int)(pistolUpgradeObj.progress_cur * pistolUpgradeObj.stepSize);
    }

    public override string WeaponName() {
        return "Pistol";
    }
}
