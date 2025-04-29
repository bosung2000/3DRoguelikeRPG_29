using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string portalID; //고유 포탈 ID ex) Room1ToRoom2

    [SerializeField] private CinemachineConfiner _confiner;
    [SerializeField] private List<PortalData> _portalList = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PortalData portalData = FindPortalData(portalID);

        if (portalData == null)
        {
            Debug.LogWarning($"[Portal] {portalID} 에 대한 데이터 없음");
            return;
        }

        if (GameManager.Instance.PortalManager.IsPortalUnlocked(portalID))
        {
            if (_confiner != null && portalData.targetCollider != null)
            {
                _confiner.m_BoundingVolume = portalData.targetCollider;
                _confiner.InvalidatePathCache();
            }

            other.transform.position = portalData.targetSpawnPoint.position;
        }
        else
        {
            Debug.Log($"[Portal] {portalID} 아직 잠김");
        }
    }

    private PortalData FindPortalData(string id)
    {
        return _portalList.Find(p => p.portalID == id);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.PortalManager.UnlockPortal(portalID);
            Debug.Log($"포탈 {portalID} 오픈(테스트)");
        }
    }
}
