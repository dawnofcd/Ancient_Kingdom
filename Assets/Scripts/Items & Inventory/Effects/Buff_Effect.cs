using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff Effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExcuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreseaStatBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }



}
