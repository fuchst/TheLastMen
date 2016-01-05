using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Combat : MonoBehaviour {

    Weapon[] weapons = new Weapon[2];
    protected int activeWeaponIdx = 0;

    void Awake()
    {
        //JingYi: not optimal but atleast we dont have errors anymore.
        weapons[0] = gameObject.AddComponent<Pistol>();
        weapons[1] = gameObject.AddComponent<Shotgun>();
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
            Transform frame = this.transform.GetChild(0);

            weapons[activeWeaponIdx].shootNVI(frame);
        }

        if (CrossPlatformInputManager.GetButtonDown("Reload"))
        {
            weapons[activeWeaponIdx].reload();
        }

        if (CrossPlatformInputManager.GetButtonDown("NextWeapon"))
        {
            activeWeaponIdx = (activeWeaponIdx + 1) % weapons.Length;
        }

        if (CrossPlatformInputManager.GetButtonDown("PrevWeapon"))
        {
            activeWeaponIdx = (activeWeaponIdx + weapons.Length - 1) % weapons.Length;
        }
    }
}
