using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemDataEquipments : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;

    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive stats")]
    public int maxHealth;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMetarials;

    private int descriptionLength;
    

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExcuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifiers(strength);
        playerStats.agility.AddModifiers(agility);
        playerStats.intelligence.AddModifiers(intelligence);

        playerStats.damage.AddModifiers(damage);
        playerStats.critChance.AddModifiers(critChance);
        playerStats.critPower.AddModifiers(critPower);

        playerStats.maxHealth.AddModifiers(maxHealth);
        playerStats.armor.AddModifiers(armor);
        playerStats.evasion.AddModifiers(evasion);
        playerStats.magicResistance.AddModifiers(magicResistance);

        playerStats.fireDamage.AddModifiers(fireDamage);
        playerStats.iceDamage.AddModifiers(iceDamage);
        playerStats.lightningDamage.AddModifiers(lightningDamage);


    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifiers(strength);
        playerStats.agility.RemoveModifiers(agility);
        playerStats.intelligence.RemoveModifiers(intelligence);

        playerStats.damage.RemoveModifiers(damage);
        playerStats.critChance.RemoveModifiers(critChance);
        playerStats.critPower.RemoveModifiers(critPower);

        playerStats.maxHealth.RemoveModifiers(maxHealth);
        playerStats.armor.RemoveModifiers(armor);
        playerStats.evasion.RemoveModifiers(evasion);
        playerStats.magicResistance.RemoveModifiers(magicResistance);

        playerStats.fireDamage.RemoveModifiers(fireDamage);
        playerStats.iceDamage.RemoveModifiers(iceDamage);
        playerStats.lightningDamage.RemoveModifiers(lightningDamage);


    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");
        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Crit Chance");
        AddItemDescription(critPower, "Crit Power");
        AddItemDescription(maxHealth, "Health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(magicResistance, "Magic Resit.");
        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightningDamage, "Lightning Damage");

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if(itemEffects[i].effectDescription.Length > 0)
            {   
                sb.AppendLine();
                sb.AppendLine("Unique: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }

        if(descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if(_value != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();

            if(_value > 0)
                sb.Append("+ " + _value + " " + _name);

            descriptionLength++;
        }
    }

}
