using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    // Variables
    protected int dmg;

    protected int magSize;
    protected int magFill;

    protected float fireInterval;
    protected float fireCounter;

    static protected float bulletSpeed = 50;
    static protected float bulletGravity = 10;

    static protected GameObject bulletPrefab;

    public virtual void OnAwake()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Bullet") as GameObject;
        }
    }

    void Awake()
    {
        OnAwake();
    }

    void  Update()
    {
        fireCounter += Time.deltaTime;
    }

    public void shootNVI(Transform frame)
    {
        if(fireCounter >= fireInterval && magFill > 0)
        {
            shoot(frame);
            fireCounter = 0;
            //magFill--;
        }
    }

    public virtual void reload()
    {
        magFill = magSize;
    }

    protected abstract void shoot(Transform frame);

    public virtual string Description () {
        return "Fire rate: " + (1.0f/fireInterval).ToString("0.00") + "\n Damage: " + dmg;
    }

    public virtual string WeaponName () {
        return "Weapon";
    }
}
