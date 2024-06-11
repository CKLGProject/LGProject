using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    public float AttackPower;
    public float DefensePower;
    public float MoveSpeed;
    public float AttackSpeed;
}
