﻿using UnityEngine;

public class Shotgun : Weapon
{
    private int numBullets;
    private float maxSpread;
    protected new AudioSource audio;

    public Shotgun()
    {
        dmg = 6;
        magSize = 4;
        magFill = magSize;
        fireInterval = 1;
        fireCounter = 0;

        numBullets = 8;     
        maxSpread = 0.15f;
    }

    public override void OnAwake()
    {
        audio = GetComponent<AudioSource>();
        base.OnAwake();
    }

    protected override void shoot(Transform frame)
    {
        for(int i = 0; i < numBullets; i++)
        {
            Vector3 spreadX = Random.Range(-maxSpread, maxSpread) * frame.right;
            Vector3 spreadY = Random.Range(-maxSpread, maxSpread) * frame.up;

            GameObject bullet = Object.Instantiate(Weapon.bulletPrefab, frame.position + frame.forward, frame.rotation) as GameObject;
            bullet.GetComponent<Bullet>().gravity = bulletGravity;
            bullet.GetComponent<Bullet>().damage = dmg;
            bullet.GetComponent<Rigidbody>().AddForce((frame.forward + spreadX + spreadY).normalized * bulletSpeed, ForceMode.Impulse);
        }
        audio.Play();
    }

    public override string Description () {
        return base.Description() + "\n   " + numBullets + " Shots";
    }

    public override string WeaponName() {
        return "Shotgun";
    }
}
