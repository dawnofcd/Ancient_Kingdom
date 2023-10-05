using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{      
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;
    private float cloneTimer;
    private float attackMulitplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;

    private Transform closetEnemy;
    private bool canDuplicateClone;
    private float chanceToDuplicate;
    private int facingDir = 1;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        if(cloneTimer <0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
            
            if(sr.color.a <= 0)
                Destroy(gameObject);
        }

    } 

    public void SetUpClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closetEnemy, bool _canDuplicateClone, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        if(_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1,4));
        }
        attackMulitplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closetEnemy = _closetEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        FaceClosetTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                // player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMulitplier);

                if(player.skill.clone.canApplyOnHitEffect)
                {
                    ItemDataEquipments weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if(weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                

                if(canDuplicateClone)
                {
                    if(Random.Range(0,100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosetTarget()
    {
        if(closetEnemy != null)
        {
            if(transform.position.x > closetEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }    
    }

}
