using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum WeaponType{
    none,
    sword,
    shield,
    crossbow,
    heal,
    bomb
}

[CreateAssetMenu(fileName = "Items",menuName = "ScriptableObjetcs/Items")]
public class Items : ScriptableObject
{
    public string itemName;
    public WeaponType typeItem;
    public int pto;
    [TextArea] public string msg;
}
