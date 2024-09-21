using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponParent;  // Reference to Weapon_R

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime = 0.5f;

    private Transform[] weapons;
    private int selectedWeapon;
    private float timeSinceLastSwitch;

    private void Start()
    {
        SetWeapons();
        SelectWeapon(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    private void SetWeapons()
    {
        // Ensure weaponParent is assigned
        if (weaponParent == null)
        {
            Debug.LogError("Weapon parent (Weapon_R) is not assigned.");
            return;
        }

        weapons = new Transform[weaponParent.childCount];

        for (int i = 0; i < weaponParent.childCount; i++)
        {
            weapons[i] = weaponParent.GetChild(i);
        }

        if (keys == null || keys.Length == 0)
        {
            keys = new KeyCode[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                keys[i] = KeyCode.Alpha1 + i;  // Automatically assign Alpha1, Alpha2, etc.
            }
        }
    }

    private void Update()
    {
        timeSinceLastSwitch += Time.deltaTime;

        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
            {
                selectedWeapon = i;
                break;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon(selectedWeapon);
        }
    }

    private void SelectWeapon(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected()
    {
        print("Selected new weapon: " + weapons[selectedWeapon].name);
    }
}
