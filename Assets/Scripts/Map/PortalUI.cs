using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PortalUI : MonoBehaviour
{
    [SerializeField] Button button1to2;
    [SerializeField] Portal _portal;

    public void Start()
    {
        button1to2.onClick.AddListener(() => _portal.TryUsePortal());
    }
}
