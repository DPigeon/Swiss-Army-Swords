﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSword : Sword
{
    public ParticleSystem particleFire = null;
    float attackTimeElapsed;
    bool charging = false;
    [SerializeField] float chargeDuration;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        base.swordType = SwordType.FIRE;
        particleFire = GetComponentsInChildren<ParticleSystem>()[0];
        particleFire.Stop();
    }

    public override void Update()
    {
        base.Update();
        if (!gameObject.activeInHierarchy)
        {
            attackTimeElapsed = 0;
            charging = false;
            particleFire.Stop();
        }
        swordAnimator.SetBool("charging", charging);
        if (Input.GetButtonDown("Fire2"))
        {
            attackTimeElapsed = 0;
            particleFire.Play();
            charging = true;
        }
        if (charging && Input.GetButton("Fire2"))
        {
            timeCharge();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            swordCollider.enabled = true;
            makeAttackSound();
            damage = Mathf.FloorToInt((1 + attackTimeElapsed));
            damaging = true;
            damageDelay = damageDuration;
            charging = false;
            particleFire.Stop();
        }
    }

    private void timeCharge()
    {
        if (attackTimeElapsed < chargeDuration)
        {
            attackTimeElapsed += Time.deltaTime;
        }
    }
}
