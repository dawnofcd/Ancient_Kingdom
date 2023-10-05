using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{   
    private Animator anim;
    private CircleCollider2D cd;
    private Player player;
    private float crystalExistTimer;
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closetTarget;

    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closetTarget, Player _player)
    {   
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closetTarget = _closetTarget;

    }

    public void ChooseRandomEnemy()
    {   
        float radius = SkillManager.instance.blackHole.GetBlackHoleRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy );

        if(colliders.Length > 0)
            closetTarget = colliders[Random.Range(0, colliders.Length)].transform;

    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        cd = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {   
        crystalExistTimer -= Time.deltaTime;
        if(crystalExistTimer < 0)
        {
            FinishCrystal();

        }

        if(canMove)
        {
            if (closetTarget == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position, closetTarget.position, moveSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position, closetTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if(canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                ItemDataEquipments equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                if(equipedAmulet != null)
                    equipedAmulet.Effect(hit.transform);
            }
        }    
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);


}
