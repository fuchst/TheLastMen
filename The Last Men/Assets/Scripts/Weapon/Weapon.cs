using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    // Variables
    protected int dmg;

    protected int magSize;
    protected int magFill;

    protected float firerate;
    protected float fireDeltaTime;

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
        fireDeltaTime += Time.deltaTime;
    }

    public void shootNVI(Transform frame)
    {
        if(fireDeltaTime >= firerate && magFill > 0)
        {
            shoot(frame);
            fireDeltaTime = 0;
            //magFill--;
        }
    }

    public virtual void reload()
    {
        magFill = magSize;
    }

    protected abstract void shoot(Transform frame);
}
