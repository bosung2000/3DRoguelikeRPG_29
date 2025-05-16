using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialSteps;
    private int currentStepIndex = 0;

    void Start()
    {
        //첫 단계만 활성화
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(i == 0);
        }
    }

    //다음 단계
    public void NextStep()
    {
        if (currentStepIndex < tutorialSteps.Length - 1)
        {
            tutorialSteps[currentStepIndex].SetActive(false);
            currentStepIndex++;
            tutorialSteps[currentStepIndex].SetActive(true);
        }
        else
        {
            Debug.Log("튜토리얼 완료");
            tutorialSteps[currentStepIndex].SetActive(false);
        }
    }
}
