using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Path = System.IO.Path;

[Serializable]
public struct SaveData
{
    public int id;
    public string data;
}

[Serializable]
public struct DataCollection
{
    public List<SaveData> dataCollection;
}
public class SaveManager : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO _saveOrderChannel,_loadOrderChannel;
    [SerializeField] private string _saveDataKey = "saveData";
    [SerializeField] private string _saveFileName = "mySaveData.dat";
    private List<SaveData> _unUsedData = new List<SaveData>();

    private void OnEnable()
    {
        _saveOrderChannel.OnValueEvent += HandleSaveOrder;
        _loadOrderChannel.OnValueEvent += HandleLoadOrder;
    }
    private void HandleLoadOrder(bool isSaveToFile)
    {
        if (isSaveToFile)
        {
            if (LoadDataFromFile(_saveFileName, out string loadData))
            {
                RestoreData(loadData);
            }
        }
        else
        {
            string loadData = PlayerPrefs.GetString(_saveDataKey);
            RestoreData(loadData);
        }
    }

    private bool LoadDataFromFile(string gameSaveFileName, out string data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, gameSaveFileName);
        data = string.Empty;
        
        if(File.Exists(fullPath)==false) return false;

        try
        {
            data = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(gameSaveFileName);
            return false;
        }
    }

    private void RestoreData(string data)
    {
        DataCollection collection = string.IsNullOrEmpty(data) ? new DataCollection():JsonUtility.FromJson<DataCollection>(data);
        
        IEnumerable<ISavable> savableObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();
        
        List<SaveData> dataList = collection.dataCollection;
       
        _unUsedData.Clear();
            
        if(dataList==null) return;
        

        foreach (SaveData saveData in dataList)
        {
            var savable = savableObjects.FirstOrDefault(obj=>obj.IdData.saveID==saveData.id);
            
            if(savable != null)
                    savable.RestoreData(saveData.data);
            else
                _unUsedData.Add(saveData);
        }
    }

    private bool WriteToFile(string gameSaveFileName, string dataJson)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, gameSaveFileName);
        Debug.Log(fullPath);

        try
        {
            File.WriteAllText(fullPath, dataJson);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
    private void HandleSaveOrder(bool isLoadFromFile)
    {
        string dataJson = GetDataToSave();

        if (isLoadFromFile)
        {
            if (!WriteToFile(_saveFileName, dataJson))
            {
                Debug.Log($"Fail to save game{_saveFileName}");
            }
        }
        else
        {
            PlayerPrefs.SetString(_saveDataKey,dataJson);
            Debug.Log(dataJson);
        }
    }

    private string GetDataToSave()
    {
        IEnumerable<ISavable> savableObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();
        
        List<SaveData> toSaveData = new List<SaveData>();

        foreach (var savable in savableObjects)
        {
            toSaveData.Add(new SaveData{id = savable.IdData.saveID,data=savable.GetSaveData()});
        }
        
        toSaveData.AddRange(_unUsedData);
        DataCollection dataCollection = new DataCollection{dataCollection = toSaveData};
            
        return JsonUtility.ToJson(dataCollection);
    }
    private void OnDisable()
    {
        _saveOrderChannel.OnValueEvent -= HandleSaveOrder;
        _loadOrderChannel.OnValueEvent -= HandleLoadOrder;
    }
}
