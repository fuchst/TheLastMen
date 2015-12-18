using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Combat : MonoBehaviour {

    Weapon[] weapons = { new Pistol(), new Shotgun() };
    protected int activeWeaponIdx = 0;

    void OnHit(int dmg)
    {
        s_GameManager.Instance.healthpoints -= dmg;
        if (s_GameManager.Instance.healthpoints <= 0)
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
