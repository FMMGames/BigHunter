using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Melee, Ranged };

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : ScriptableObject
{
    public WeaponType weaponType;
    public int weaponDamage;
    public float attackRate;
    public GameObject weaponObject;
    public GameObject idleVersion;
}
