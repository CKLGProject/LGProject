using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CardInfo cardInfo = new CardInfo(ECharacterType.Archer);
        var json = cardInfo.ConvertJson();
        Debug.Log(json);
    }
}

public enum ECharacterType
{
    None,
    Warrior,
    Archer,
    Mage
}

public class CardInfo
{
    public string Company;
    public string Id;
    public ECharacterType CharacterType;
    public bool Use;

    public CardInfo(ECharacterType characterType)
    {
        Company = "LG";
        Id =  Guid.NewGuid().ToString();
        CharacterType = characterType;
    }

    public string ConvertJson()
    {
        var json = JsonUtility.ToJson(this, true);
        return json;
    }
}
