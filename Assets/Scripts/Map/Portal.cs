using UnityEngine;

public class Portal : MonoBehaviour
{
    public string portalID;                   //포탈의 고유 id ex)room1,room2
    public Transform targetSpawnPoint;        // 이동할 위치



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.PortalManager.IsPortalUnlocked(portalID))
            {
                other.transform.position = targetSpawnPoint.position;
            }
            else
            {
                Debug.Log("포탈이 아직 잠겨있습니다.");
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.PortalManager.UnlockPortal("Room1ToRoom2");
            Debug.Log("포탈 오픈됨");
        }
    }
}
