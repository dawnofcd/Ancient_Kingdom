using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHoleHotKeysController : MonoBehaviour
{   
    private SpriteRenderer sr;
    private KeyCode myhotKeys;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private BlackHoleSkillController blackHole;

    public void SetupHotKey(KeyCode _myHotKey, Transform _myEnemy, BlackHoleSkillController _myBlackHole)
    {   
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackHole;

        myhotKeys = _myHotKey;
        myText.text = _myHotKey.ToString();
    }

    void Update()
    {
        if(Input.GetKeyDown(myhotKeys))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
