using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq; 
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;
    [SerializeField] private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;


    [ContextMenu("Delete Save File")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {   
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {   
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);  // Application.persistentDataPath: lưu đường dẫn dữ liệu liên tục
        saveManagers = FindAllSaveManagers(); 

        LoadGame();
    }


    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {   
        // game data = data from data handler

        gameData = dataHandler.Load();

        if(this.gameData == null)
        {   
            Debug.Log("No saved data load");
            NewGame();
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        //data handler save gamedata
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager>  saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers); //trả về danh sách quản lí và cung cấp cho danh sách đó. 
    }


    public bool HasSavedData()
    {
        if(dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }

}
