using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    
    private Transform player;

    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closetsCheckpointId;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;


    private void Awake()
    {   
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
            
    }

    private void Start()
    {
        checkPoints = FindObjectsOfType<CheckPoint>();
        player = PlayerManager.instance.player.transform;
    }

    public void RestartScene()
    {   
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));
    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkPoints)
        {
            foreach (CheckPoint checkpoint in checkPoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActiveCheckPoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }   

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }


    public void SaveData(ref GameData _data)
    {       
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if(FindClossetCheckPoint() != null)
            _data.clossetCheckPointId = FindClossetCheckPoint().id;

        _data.checkPoints.Clear();

        foreach(CheckPoint checkPoint in checkPoints)
        {
            _data.checkPoints.Add(checkPoint.id, checkPoint.activationStatus);
        }
    }

    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.clossetCheckPointId == null)
            return;


        closetsCheckpointId = _data.clossetCheckPointId;

        foreach (CheckPoint checkpoint in checkPoints)
        {
            if (closetsCheckpointId == checkpoint.id)
                player.position = checkpoint.transform.position;
        }

    }

    private CheckPoint FindClossetCheckPoint()
    {
        float clossetDistance = Mathf.Infinity;
        CheckPoint clossetCheckPoint  = null;

        foreach(var checkPoint in checkPoints)
        {
            float distanceToCheckPoint = Vector2.Distance(player.position, checkPoint.transform.position);

            if(distanceToCheckPoint < clossetDistance && checkPoint.activationStatus == true)
            {
                clossetDistance = distanceToCheckPoint;
                clossetCheckPoint = checkPoint;
            }
        }

        return clossetCheckPoint;

    }

    public void PauseGame(bool _pause)
    {
        if(_pause)
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;
    }

}
