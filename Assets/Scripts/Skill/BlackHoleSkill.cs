using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHoleSkill : Skill
{   

    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked {get; private set;}
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneAttackCooldown;
    [SerializeField] private float blackHoleDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    BlackHoleSkillController currentBlackHole;

    private void UnlockBlackHole()
    {
        if(blackHoleUnlockButton.unlocked)
            blackHoleUnlocked = true;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position,Quaternion.identity);

        currentBlackHole = newBlackHole.GetComponent<BlackHoleSkillController>();

        currentBlackHole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneAttackCooldown, blackHoleDuration);
        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    protected override void Start()
    {
        base.Start();
        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackHole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {   
        if(!currentBlackHole)
            return false;

        if(currentBlackHole.playerCanExitState)
        {   
            currentBlackHole = null;
            return true;
        }
        return false;
    }


    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }

    protected override void CheckUnlocked()
    {
        base.CheckUnlocked();
        UnlockBlackHole();
    }

}
