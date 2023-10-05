using TMPro;
using UnityEngine;
public class UI_SkillToolTip : UI_ToolTips
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

    public void ShowToolTip(string _text, string _skillName, int _price)
    {
        skillText.text = _text;
        skillName.text = _skillName;
        skillCost.text = "Cost: " + _price;

        AdjustPosition();
        AdJustFontSize(skillName);
        

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }


}
