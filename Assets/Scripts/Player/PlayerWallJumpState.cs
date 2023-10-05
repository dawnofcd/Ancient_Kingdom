using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = .3f;
        player.SetVelocity(player.facingDir, player.jumpForce);
    }

    public override void Update()
    {   
        base.Update();
        
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.Space) && player.canDoubleJump)
        {
            player.canDoubleJump = false;
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if(stateTimer <0)
            stateMachine.ChangeState(player.airState);
            
        if(player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
            

    }

    public override void Exit()
    {
        base.Exit();
    }
}
