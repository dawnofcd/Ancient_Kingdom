using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{      
    [Header("Attack details")]
    public Vector2[] attackMovements;
    public float counterAttackDuration = .2f;

    
    public bool isBusy { get; private set;}

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce = 13f;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    public float swordReturnImpact;
    [HideInInspector] public bool canDoubleJump;
    public float doubleJumpForce;

    [Header("Jump Buffer")]
    public bool jumpBuffered;
    private float jumpBufferTime = 0.2f;
    private float currentJumpBuffer = 0;

    [Header("Coyote Jump")]
    public float coyoteDuration = 0.15f;
    public float coyoteTime;


    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }


    #region State
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState   { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerDoubleJumpState doubleJump { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackHoleState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    #endregion

    protected override void Awake()
    {   
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump");
        doubleJump = new PlayerDoubleJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackHoleState(this, stateMachine, _animBoolName: "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");


    }

    protected override void Start()
    {      
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;

    }

    protected override void Update()
    {   
        if(Time.timeScale == 0)
            return;

        base.Update();
        
        stateMachine.currentState.Update();        
        CheckForDashInput();
        JumpBufferCheck();
        CoyoteTimeCheck();

        if(Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlock)
            skill.crystal.CanUseSkill();

        if(Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();

    }  

    private void CoyoteTimeCheck()
    {
        if(coyoteTime > 0)
        {
            coyoteTime -= Time.deltaTime;
        }
    }

    private void JumpBufferCheck()
    {
        if(jumpBuffered)
        {
            currentJumpBuffer -= Time.deltaTime;
            if(currentJumpBuffer <= 0)
            {
                jumpBuffered = false;
            }
        }
    }

    public void SetupJumBuffer()
    {
        jumpBuffered = true;
        currentJumpBuffer = jumpBufferTime;
    }


    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);        
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public void AssignedNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    private void CheckForDashInput()
    {     
        if(IsWallDetected())
            return;

        if(skill.dash.dashUnlocked == false)
            return;

        if(Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir ==0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);

        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }

}
