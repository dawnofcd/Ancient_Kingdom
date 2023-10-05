using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health Effect", menuName = "Data/Item Effect/Health Effect")]
public class Health_Effect : ItemEffect
{      
    [Range(0f,1f)]
    [SerializeField] private float healPercent;


    public override void ExcuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        playerStats.InCreaseHealthBy(healAmount);
    }
}
