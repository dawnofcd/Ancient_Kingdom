using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{   
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordThrowImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image useItemImage;
    [SerializeField] private Image attackImage;

    private SkillManager skills;



    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increseaRate = 5000; 
    void Start()
    {
        if(playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlock)
            SetCooldownOf(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldownOf(swordThrowImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackHole.blackHoleUnlocked)
            SetCooldownOf(blackHoleImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(useItemImage);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            SetCooldownOf(attackImage);

        CheckCoolDownOf(dashImage, skills.dash.coolDown);
        CheckCoolDownOf(parryImage, skills.parry.coolDown);
        CheckCoolDownOf(crystalImage, skills.crystal.coolDown);
        CheckCoolDownOf(swordThrowImage, skills.sword.coolDown);
        CheckCoolDownOf(blackHoleImage, skills.blackHole.coolDown);
        CheckCoolDownOf(useItemImage, Inventory.instance.flaskCooldown);
        CheckCoolDownOf(attackImage, 0.5f);


    }

    public void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurency())
            soulsAmount += Time.deltaTime * increseaRate;
        else
            soulsAmount = PlayerManager.instance.GetCurency();


        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if(_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCoolDownOf(Image _image, float _cooldown)
    {
        if(_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
        
    }



}
