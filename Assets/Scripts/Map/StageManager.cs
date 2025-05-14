using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    private int MaxStage = 5;
    public int CurrentStage { get; private set; } = 1;
    public float HpMultiplier => 1f + 0.3f * (CurrentStage - 1);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void NextStage()
    {
        if (CurrentStage < MaxStage)
        {
            CurrentStage++;
        }
        else
        {
            //finish
            //GameEnd popup 띄워주기
        }
    }
}
