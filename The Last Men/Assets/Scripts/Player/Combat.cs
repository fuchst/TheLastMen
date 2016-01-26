﻿using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Combat : MonoBehaviour {
    
    List<GameObject> weapons = new List<GameObject>();

    public Transform cameraTransform;

    protected int activeWeaponIdx = 0;
    protected GameObject currWeaponModel;

    public string GetCurrentWeaponDescription () {
        return weapons[activeWeaponIdx].GetComponent<Weapon>().Description();
    }
    public string GetCurrentWeaponName () {
        return weapons[activeWeaponIdx].GetComponent<Weapon>().WeaponName();
    }

    public int ActiveWeaponIndex { get { return activeWeaponIdx; } }

    void Awake()
    {
        Vector3 pos, scale;
        Quaternion rot;
        GameObject prefab = null;
        string file;

        // Pistol
        pos = new Vector3(cameraTransform.position.x + 0.3F, cameraTransform.position.y - 0.2F, cameraTransform.position.z + 1.0F);
        rot = Quaternion.identity;
        scale = new Vector3(0.1F, 0.1F, 0.1F);
        file = "PistolPrefab";
        prefab = Resources.Load(file) as GameObject;
        if(prefab != null)
        {
            AddWeapon(prefab, pos, rot, scale, cameraTransform);
        }
        else
        {
            Debug.Log("Resource " + file + " not found!");
        }

        // Shotgun
        pos = new Vector3(cameraTransform.position.x + 0.3F, cameraTransform.position.y - 0.2F, cameraTransform.position.z + 0.6F);
        rot = Quaternion.Euler(-90, 0, 0);
        scale = new Vector3(0.5F, 0.5F, 0.5F);
        file = "ShotgunPrefab";
        prefab = Resources.Load(file) as GameObject;
        if (prefab != null)
        {
            AddWeapon(prefab, pos, rot, scale, cameraTransform);
        }
        else
        {
            Debug.Log("Resource " + file + " not found!");
        }

        weapons[activeWeaponIdx].SetActive(true);
    }

    void AddWeapon(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
    {
        GameObject newWeapon = Instantiate(prefab, position, rotation) as GameObject;
        if (newWeapon == null)
        {
            Debug.Log("Unable to instantiate weapon!");
        }
        newWeapon.transform.localScale = scale;
        newWeapon.transform.SetParent(parent);
        newWeapon.SetActive(false);
        weapons.Add(newWeapon);
    }

    public void OnHit (int dmg) {
        s_GameManager.Instance.HurtPlayer(dmg);
    }

    void Update()
    {
        if(s_GameManager.Instance.gamePaused || s_GameManager.Instance.BastionMenu)
        {
            return; //don't fight in menus!
        }

        if (CrossPlatformInputManager.GetButton("Shoot"))
        {
            Transform frame = this.transform.GetChild(0);

            weapons[activeWeaponIdx].GetComponent<Weapon>().shootNVI(frame);
        }

        if (CrossPlatformInputManager.GetButtonDown("Reload"))
        {
            weapons[activeWeaponIdx].GetComponent<Weapon>().reload();
        }

        if (CrossPlatformInputManager.GetButtonDown("NextWeapon"))
        {
            weapons[activeWeaponIdx].SetActive(false);
            activeWeaponIdx = (activeWeaponIdx + 1) % weapons.Count;
            weapons[activeWeaponIdx].SetActive(true); 
        }

        if (CrossPlatformInputManager.GetButtonDown("PrevWeapon"))
        {
            weapons[activeWeaponIdx].SetActive(false);
            activeWeaponIdx = (activeWeaponIdx + weapons.Count - 1) % weapons.Count;
            weapons[activeWeaponIdx].SetActive(true);
        }
    }
}
