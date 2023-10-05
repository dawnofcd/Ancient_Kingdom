using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{       
    [Header("End Screen")]
    [SerializeField] private  UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    
    public UI_SkillToolTip skillToolTip;
    public UI_StatToolTips statToolTips;
    public UI_ItemToolTips itemToolTips;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings; 

    [SerializeField] private TextMeshProUGUI currentSouls;    

    void Awake()
    {
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);
        itemToolTips.gameObject.SetActive(false);
        statToolTips.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        currentSouls.text = PlayerManager.instance.GetCurency().ToString("#,#");        
        if(Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);

        if(Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if(Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if(Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(optionsUI);
        
    }

    public void SwitchTo(GameObject _menu)
    {   

        for (int i = 0; i < transform.childCount; i++)
        {       
            bool isFadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;
            if(!isFadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);

        }

        if(_menu != null)
        {   
            AudioManager.instance.PlaySFX(7,null);
            _menu.SetActive(true);
        }   

        if(GameManager.instance != null)
        {
            if(_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {   
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);

            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {       
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());

    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1);
        restartButton.SetActive(true);
    }
    public void RestartGameButton() => GameManager.instance.RestartScene();    

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach(UI_VolumeSlider item in volumeSettings)
            {
                if(item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach(UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }

}
