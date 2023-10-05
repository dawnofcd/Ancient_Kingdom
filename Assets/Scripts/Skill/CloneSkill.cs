using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{   
    [Header("Clone info")]
    [SerializeField] private float attackMulitplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone Attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive Clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMulitplier;
    public bool canApplyOnHitEffect {get; private set;}
 
    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot mulitpleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        mulitpleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMulipleClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);

    }



    #region Unlock region

    protected override void CheckUnlocked()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockCrystalInstead();
        UnlockMulipleClone();
    }
    private void UnlockCloneAttack()
    {
        if(cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMulitplier = cloneAttackMultiplier;
        }
        
    }

    private void UnlockAggresiveClone()
    {
        if(aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect =  true;
            attackMulitplier = aggresiveCloneAttackMulitplier;
        }
    }

    private void UnlockMulipleClone()
    {
        if(mulitpleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMulitplier = multiCloneAttackMultiplier;
        }
    }   

    private void UnlockCrystalInstead()
    {
        if(crystalInsteadUnlockButton.unlocked)
            crystalInsteadOfClone = true;
    }


    #endregion



    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {   
        if(crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            SkillManager.instance.crystal.CurrentCrystalChosseRandomTarget();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetUpClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosetEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player, attackMulitplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3( 2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform, _offset);
    }

}
