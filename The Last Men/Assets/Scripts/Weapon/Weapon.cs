using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    // Variables
    protected int dmg;

    protected int magSize;
    protected int magFill;

    protected float firerate;
    protected float timestamp;

    static protected float bulletSpeed = 50;
    static protected float bulletGravity = 10;

    static protected GameObject bulletPrefab;

    public Weapon()
    {
        if(bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Bullet") as GameObject;
        }
    }

    public void shootNVI(Transform frame)
    {
        if((Time.time - timestamp) >= firerate && magFill > 0)
        {
            shoot(frame);
            magFill--;
        }
    }

    public virtual void reload()
    {
        magFill = magSize;
    }

    protected abstract void shoot(Transform frame);
}
