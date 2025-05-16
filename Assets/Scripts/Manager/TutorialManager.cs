using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] steps; //각 단계 오브젝트
    private int currentStep = 0;

    private void Start()
    {
        InitSteps();
    }

    private void InitSteps()
    {
        //첫 단계만 켬
        for (int i = 0; i < steps.Length; i++)
            steps[i].SetActive(i == 0);
    }

    public void ProceedToNextStep()
    {
        if (currentStep < steps.Length - 1)
        {
            steps[currentStep].SetActive(false);
            currentStep++;
            steps[currentStep].SetActive(true);
        }
        else
        {
            Debug.Log("튜토리얼 종료");
        }
    }
}
