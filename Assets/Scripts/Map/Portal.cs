using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    [SerializeField] CinemachineConfiner _confiner;
    [SerializeField] List<PortalData> _portalList = new List<PortalData>();
    [SerializeField] Player _player;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    _enteredPlayer = other.transform;
    //    _enteredPortalID = GetPortalID(other.transform);
    //}

    //private Transform _enteredPlayer;
    //private string _enteredPortalID;

    public void TryUsePortal(string targetPortalID)
    {
        StartCoroutine(TeleportWithConfiner(targetPortalID));
    }
    private IEnumerator TeleportWithConfiner(string portalID)
    {
        yield return new WaitForFixedUpdate();

        PortalData portalData = FindPortalData(portalID);
        if (portalData == null) yield break;

        var player = _player;
        if (player == null) yield break;

        player.transform.position = portalData.targetSpawnPoint.position;

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return null;
        _confiner.m_BoundingVolume = portalData.targetCollider;
        _confiner.InvalidatePathCache();
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
            //GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom3");
            //GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom4");
            //GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom5");
            //GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom6");
            //GameManager.Instance.PortalManager.UnlockPortal("PortalToRoom7");
            Debug.Log("포탈 오픈");
        }
    }
}
