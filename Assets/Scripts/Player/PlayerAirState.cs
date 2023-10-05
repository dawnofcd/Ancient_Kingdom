using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();
        
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.Space))
        {   
            if(player.canDoubleJump)
            {
                player.canDoubleJump = false;
                stateMachine.ChangeState(player.jumpState);
            }

            if(player.coyoteTime > 0)
            {
                stateMachine.ChangeState(player.jumpState);
                player.coyoteTime = 0;
            }
            else
                player.SetupJumBuffer();
        }
        
        if(player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        if(player.IsGroundDetected())
        {
            if(player.jumpBuffered)
            {
                player.jumpBuffered = false;
                player.canDoubleJump = true;
                stateMachine.ChangeState(player.jumpState);
            }
            else
                stateMachine.ChangeState(player.idleState);
        }
                   
    }

    public override void Exit()
    {
        base.Exit();


    }

}
