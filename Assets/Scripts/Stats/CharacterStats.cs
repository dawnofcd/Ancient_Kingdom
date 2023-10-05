using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    maxHealth,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightningDamage


}

public class CharacterStats : MonoBehaviour
{       
    private Entity_FX fx;

    [Header("Major stats")]
    public Stat strength; // sức mạnh: 1 điểm sẽ tăng sát thương lên 1 điểm và sức mạnh chí mạng thêm 1%.
    public Stat agility; // nhanh nhẹn: 1 điểm sẽ + 1 né tránh và + 1 tấn công chí mạng.
    public Stat intelligence; //thông minh =)): 1 điểm sẽ tăng sát thương phép lên 1 và kháng phép thêm 3( hoặc...)
    public Stat vitality; // sức sống: 1 điểm sẽ tăng máu lên 3 hoặc 5...hoặc nhiều hơn 

    [Header("Offensive stats")]
    public Stat damage; //sát thương
    public Stat critChance; // tỉ lệ chí mạng             
    public Stat critPower; // sát thương chí mạng           //Mặc định là 150%

    [Header("Defensive stats")]
    public Stat maxHealth; // máu
    public Stat armor; // giáp
    public Stat evasion; // né
    public Stat magicResistance; //kháng phép

    [Header("Magic stats")]
    public Stat fireDamage; // lửa
    public Stat iceDamage;  // băng
    public Stat lightningDamage; // sét


    public bool isIgnited; // thiêu đốt: Sát thương theo thời gian 
    public bool isChilled; // làm chậm và giảm giáp đi 20%
    public bool isShocked; // giảm độ chính xác 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;




    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;



    public float currentHealth;
    public System.Action onHealthChanged;

    public bool isDead {get; private set; }
    public bool isInvincible {get; private set;}
    private bool isVulnerable;


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<Entity_FX>();

    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;


        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCoroutine(_duration));
    }

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreseaStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StartModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StartModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifiers(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifiers(_modifier);


    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);
        int totalDamage = damage.GetValue() + strength.GetValue();

        if(CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
        

    }

    #region Magical Damage and Ailments
    public virtual void DoMagicalDamage(CharacterStats _targetStat)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();


        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStat, totalMagicalDamage);

        _targetStat.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;
        AttemptyToApplyAilments(_targetStat, _fireDamage, _iceDamage, _lightningDamage);

    }

    private void AttemptyToApplyAilments(CharacterStats _targetStat, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStat.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStat.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStat.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }


        }

        if (canApplyIgnite)
            _targetStat.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f)); //gây 20% sát thương lửa 1s trong 3s

        if (canApplyShock)
            _targetStat.ShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        _targetStat.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool CanApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool CanApplychill = !isIgnited && !isChilled && !isShocked;
        bool CanApplyshock = !isIgnited && !isChilled;

        if(_ignite && CanApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if(_chill && CanApplychill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if(_shock && CanApplyshock)
        {
            if(!isShocked)
            {
                ApplyShock(_shock);

            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();

            }

        }

    }

    public void ApplyShock(bool _shock)
    {   
        if(isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closetDistance = Mathf.Infinity;
        Transform closetEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closetDistance)
                {
                    closetDistance = distanceToEnemy;
                    closetEnemy = hit.transform;
                }
            }

            if (closetEnemy == null)
                closetEnemy = transform;
        }

        if (closetEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrikeController>().Setup(shockDamage, closetEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer <0 )
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void ShockStrikeDamage(int _damage) => shockDamage = _damage;    

    #endregion

    public virtual void TakeDamage(int _damage)
    {   
        if(isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        if(currentHealth <= 0 && !isDead)
            Die();
        
    }

    public virtual void InCreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if(onHealthChanged != null)
            onHealthChanged();
    }


    protected virtual void DecreaseHealthBy(int _damage)
    {   
        if(isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;

        if(onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if(!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    #region Stat caculations
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {   
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() *.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStat, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStat.magicResistance.GetValue() + (_targetStat.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if(isShocked)
            totalEvasion += 20;
        
        if (Random.Range(0, 100) < totalEvasion)
        {   
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion

    public Stat GetStat(StatType _statType)
    {
        if(_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.maxHealth) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicResistance) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightningDamage) return lightningDamage;

        return null;

        
    }


}
