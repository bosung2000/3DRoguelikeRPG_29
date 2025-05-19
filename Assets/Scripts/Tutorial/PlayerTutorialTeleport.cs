using UnityEngine;

public class PlayerTutorialTeleport : MonoBehaviour
{
    public string tutorialPointName = "potal_TutorialRoom"; // 이동 지점 오브젝트 이름
    public KeyCode teleportKey = KeyCode.T; // 누를 키

    private void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            GameObject player = GameObject.FindWithTag("Player");
            GameObject target = GameObject.Find(tutorialPointName);

            if (player != null && target != null)
            {
                player.transform.position = target.transform.position;
                player.transform.rotation = target.transform.rotation;
                Debug.Log($"[튜토리얼] {teleportKey} 키 입력으로 플레이어를 TutorialStartPoint로 이동했습니다.");
            }
            else
            {
                Debug.LogWarning("Player 또는 TutorialStartPoint를 찾을 수 없습니다.");
            }
        }
    }
}
