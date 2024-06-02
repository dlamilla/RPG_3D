using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject sword;
    public List<Items> weapons;
    public List<Items> items;
    public bool swordUse;
    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        weapons = new List<Items>();
        items = new List<Items>();
    }

    public void swordActive(Items item)
    {
        sword.SetActive(true);
        anim.SetFloat("WeaponN", 0);
        if (!swordUse)
        {
            swordUse = true;
            anim.SetBool("Weapon", true);
        }
        anim.SetTrigger("SwitchWeapon");
    }
}
