using Cinemachine;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Portal : MonoBehaviour
{
    //public string portalID;                   //포탈의 고유 id ex)room1,room2
    //public Transform targetSpawnPoint;        // 이동할 위치
    [SerializeField] CinemachineConfiner _confiner;
    [SerializeField] List<PortalData> _portalList = new List<PortalData>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        string enteredPortalID = GetPortalID(other.transform);
        PortalData portalData = FindPortalData(enteredPortalID);

        {
            if (GameManager.Instance.PortalManager.IsPortalUnlocked(portalData.portalID))
            {
                _confiner.m_BoundingVolume = portalData.targetCollider;
                _confiner.InvalidatePathCache();
                other.transform.position = portalData.targetSpawnPoint.position;
            }
            else
            {
                Debug.Log("포탈이 아직 잠겨있습니다.");
            }
        }
    }
    private PortalData FindPortalData(string portalID)
    {
        return _portalList.Find(portal => portal.portalID == portalID);
    }
    private string GetPortalID(Transform player)
    {
        return this.name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom2");
            Debug.Log("포탈 오픈");
        }
    }
}
