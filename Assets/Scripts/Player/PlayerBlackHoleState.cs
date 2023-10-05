using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{   
    private float flyTime = .4f;
    private bool skillUse;

    private float defaultGravity;
    public PlayerBlackHoleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;
        skillUse = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer > 0)
        {
            rb.velocity = new Vector2(0 ,15);
        }
        if(stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if(!skillUse)
            {
                if(player.skill.blackHole.CanUseSkill())
                    skillUse = true;
            }
        }

        if(player.skill.blackHole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
    }


}
