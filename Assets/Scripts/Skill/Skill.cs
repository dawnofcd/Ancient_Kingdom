using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float coolDown;
    public   float coolDownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        CheckUnlocked();
    }

    protected virtual void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    protected virtual void CheckUnlocked()
    {

    }

    public virtual bool CanUseSkill()
    {
        if(coolDownTimer <0)
        {
            //Use Skill
            UseSkill();
            coolDownTimer = coolDown;
            return true;
        }
        Debug.Log("Skill is on cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
        //do some skill spesific thing
    }

    protected virtual Transform FindClosetEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closetDistance = Mathf.Infinity;
        Transform closetEnemy = null;

        foreach( var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if( distanceToEnemy < closetDistance)
                {
                    closetDistance = distanceToEnemy;
                    closetEnemy = hit.transform;
                }
            }
        }    
        
        return closetEnemy;
    }


}
