using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.coyoteTime = 0;
        player.canDoubleJump = true;
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackHole.blackHoleUnlocked)
        {
            if(player.skill.blackHole.coolDownTimer > 0)
                return;
                
            stateMachine.ChangeState(player.blackHole);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSword);
        
        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttack);

        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        if( !player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }

        
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool HasNoSword()
    {
        if(!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
