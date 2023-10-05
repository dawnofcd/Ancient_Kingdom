using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{   
    [Header("Dodge")]
    [SerializeField] UI_SkillTreeSlot unlockDodgeButton;
    public bool dodgeUnlocked;

    [Header("Mirage dodge")]
    [SerializeField] UI_SkillTreeSlot unlockMirageDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeMirageUnlocked;


    protected override void Start()
    {
        base.Start();
        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    protected override void CheckUnlocked()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    private void UnlockDodge()
    {
        if(unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifiers(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }

    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodgeButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if(dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(3 * player.facingDir, 0));
    }

}
