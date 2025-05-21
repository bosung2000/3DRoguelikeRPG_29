using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
#if !UNITY_WEBGL
    private string savePath;
#endif

    private void Awake()
    {
#if !UNITY_WEBGL
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
#endif
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);

#if UNITY_WEBGL
        PlayerPrefs.SetString("GameData", json);
        PlayerPrefs.Save();
#else
        File.WriteAllText(savePath, json);
#endif
    }

    public GameData LoadData()
    {
#if UNITY_WEBGL
        if (PlayerPrefs.HasKey("GameData"))
        {
            string json = PlayerPrefs.GetString("GameData");
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            return new GameData(); // 초기값 리턴
        }
#else
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            return new GameData(); // 초기값 리턴
        }
#endif
    }

    public void ClearData()
    {
#if UNITY_WEBGL
        PlayerPrefs.DeleteKey("GameData");
#else
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
#endif
    }
}
