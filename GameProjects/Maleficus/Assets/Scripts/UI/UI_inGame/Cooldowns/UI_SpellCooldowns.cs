﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_SpellCooldowns : MaleficusMonoBehaviour
{

    private Image spellIcon;
    public ESpellSlot SpellSlot { get { return spellSlot; } }
    [SerializeField] ESpellSlot spellSlot;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
       
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        spellIcon = GetComponentWithCheck<Image>();

    }


   public void StartCooldown(float cooldown)
    {
        spellIcon.fillAmount = 0;
        StartCoroutine(CooldownCoroutine(cooldown));

    }

    private IEnumerator CooldownCoroutine(float cooldown)
    {
        float startTime = Time.time ;
        while (Time.time - startTime < cooldown)
        {
            spellIcon.fillAmount = (Time.time - startTime) / cooldown;
            yield return new WaitForEndOfFrame();
           
        }

    }

}