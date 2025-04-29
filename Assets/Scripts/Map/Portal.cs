using Cinemachine;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    [SerializeField] CinemachineConfiner _confiner;
    [SerializeField] List<PortalData> _portalList = new List<PortalData>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _enteredPlayer = other.transform;
        _enteredPortalID = GetPortalID(other.transform);
    }

    private Transform _enteredPlayer;
    private string _enteredPortalID;

    public void TryUsePortal()
    {
        if (_enteredPlayer == null)
        {
            Debug.LogWarning("플레이어가 포탈에 닿지 않았습니다.");
            return;
        }

        PortalData portalData = FindPortalData(_enteredPortalID);
        if (portalData == null)
        {
            Debug.LogWarning($"포탈 ID {_enteredPortalID}를 찾을 수 없습니다.");
            return;
        }

        if (GameManager.Instance.PortalManager.IsPortalUnlocked(portalData.portalID))
        {
            _confiner.m_BoundingVolume = portalData.targetCollider;
            _confiner.InvalidatePathCache();
            _enteredPlayer.position = portalData.targetSpawnPoint.position;
        }
        else
        {
            Debug.Log("포탈이 아직 잠겨있습니다.");
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
