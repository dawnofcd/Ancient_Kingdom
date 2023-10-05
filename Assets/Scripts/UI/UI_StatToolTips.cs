using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatToolTips : UI_ToolTips
{
    [SerializeField] private TextMeshProUGUI description;   

    public void ShowStatToolTip(string _text)
    {
        description.text = _text;
        AdjustPosition();
        // AdJustFontSize(description);
        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    {   
        description.text = "";
        gameObject.SetActive(false);
    }

}
