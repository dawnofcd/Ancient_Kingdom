using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{   
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;    

    [Header("Crystal Simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlock {get; private set;}

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode; 

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStack;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosion);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);


    }

    #region Unlock skill region
    protected override void CheckUnlocked()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosion();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }

    private void UnlockCrystal()
    {
        if(unlockCrystalButton.unlocked)
            crystalUnlock = true;
    }

    private void UnlockCrystalMirage()
    {
        if(unlockCloneInsteadButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosion()
    {
        if(unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if(unlockMultiStackButton.unlocked)
            canUseMultiStack = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal())
            return;

        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {      
            if(canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if(cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalSkillController>().FinishCrystal();
                
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(currentCrystal.transform), player);
        
    }

    public void CurrentCrystalChosseRandomTarget()
    {
        currentCrystal.GetComponent<CrystalSkillController>().ChooseRandomEnemy();
    }

    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStack)
        {
            if(crystalLeft.Count > 0)
            {       
                if(crystalLeft.Count == amountOfStacks)
                        Invoke("ResetAbility", useTimeWindow);

                coolDown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count -1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<CrystalSkillController>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(newCrystal.transform), player);
            
                if(crystalLeft.Count <= 0)
                {
                    coolDown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            }
        }

        return false;
    }

    private void RefilCrystal()
    {   
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if(coolDownTimer > 0)
            return;

        coolDownTimer = multiStackCooldown;
        RefilCrystal();
    }

}
