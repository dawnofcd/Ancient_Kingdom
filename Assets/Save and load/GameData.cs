using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData 
{
    public int currency; //tiền

    public SerializableDictionary<string, bool> skillTree;  //skill trees
    public SerializableDictionary<string, int> inventory;      //ID item
    public List<string> equipmentID; //equipments

    public SerializableDictionary<string, bool> checkPoints;
    public string clossetCheckPointId;

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSettings;
    public GameData()
    {   
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;


        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentID = new List<string>();

        clossetCheckPointId = string.Empty;
        checkPoints = new SerializableDictionary<string, bool>();
        volumeSettings = new SerializableDictionary<string, float>();

    }

    

}
