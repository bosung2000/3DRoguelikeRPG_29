using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("SaveManager");
                    instance = obj.AddComponent<SaveManager>();
                }
            }
            return instance;
        }
    }

#if !UNITY_WEBGL
    private string savePath;
#endif

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

#if !UNITY_WEBGL
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
#endif
        Debug.Log("SaveManager 초기화 완료");
    }

    public void SaveData(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            Debug.Log($"저장 시도 데이터: {json}");

#if UNITY_WEBGL
            try
            {
                PlayerPrefs.SetString("GameData", json);
                PlayerPrefs.Save();
                Debug.Log("WebGL 저장 성공");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"WebGL 저장 실패: {e.Message}");
            }
#else
            try
            {
                File.WriteAllText(savePath, json);
                Debug.Log($"파일 저장 성공: {savePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"파일 저장 실패: {e.Message}");
            }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 직렬화 실패: {e.Message}");
        }
    }

    public GameData LoadData()
    {
#if UNITY_WEBGL
        try
        {
            if (PlayerPrefs.HasKey("GameData"))
            {
                string json = PlayerPrefs.GetString("GameData");
                Debug.Log($"WebGL 로드 데이터: {json}");
                return JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.Log("WebGL 저장된 데이터 없음");
                return new GameData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGL 로드 실패: {e.Message}");
            return new GameData();
        }
#else
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                Debug.Log($"파일 로드 데이터: {json}");
                return JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.Log("저장된 파일 없음");
                return new GameData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 로드 실패: {e.Message}");
            return new GameData();
        }
#endif
    }

    public void ClearData()
    {
#if UNITY_WEBGL
        try
        {
            PlayerPrefs.DeleteKey("GameData");
            Debug.Log("WebGL 데이터 삭제 성공");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGL 데이터 삭제 실패: {e.Message}");
        }
#else
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("파일 삭제 성공");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 삭제 실패: {e.Message}");
        }
#endif
    }
}
