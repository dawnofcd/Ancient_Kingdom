using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoubleJumpState : PlayerState
{
    public PlayerDoubleJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.velocity = new Vector2(rb.velocity.x, player.doubleJumpForce);
    }

    public override void Update()
    {
        base.Update();
    

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
        if(rb.velocity.y <0)
            stateMachine.ChangeState(player.airState);
    }

    public override void Exit()
    {
        base.Exit();
    }


}
