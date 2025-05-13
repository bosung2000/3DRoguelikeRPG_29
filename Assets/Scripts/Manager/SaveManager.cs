using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveData(GameData data)
    {
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = Path.Combine(Application.persistentDataPath, "save.json");
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"저장 완료: {savePath}");
    }

    public GameData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.Log("저장 파일 없음. 새로 생성");
            return new GameData();        
        }
    }
}
