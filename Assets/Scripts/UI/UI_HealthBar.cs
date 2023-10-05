using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {   
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void OnDisable()
    {   
        if(entity != null)
            entity.onFlipped -= FlipUI;
        if(myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;

    }
    private void FlipUI() => myTransform.Rotate(0, 180, 0);



}
