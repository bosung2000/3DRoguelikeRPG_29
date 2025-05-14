using UnityEngine;
using UnityEngine.UI;

public class PortalUI : MonoBehaviour
{
    [SerializeField] Button button1to2;
    [SerializeField] Button button2to3;
    [SerializeField] Button button3to4;
    [SerializeField] Button button4to5;
    [SerializeField] Button button5to6;
    [SerializeField] Button button6to7;

    [SerializeField] Portal _portal;

    public void Start()
    {
        button1to2.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom2"));
        button2to3.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom3"));
        button3to4.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom4"));
        button4to5.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom5"));
        button5to6.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom6"));
        button6to7.onClick.AddListener(() => _portal.TryUsePortal("PortalToRoom7"));
    }
}
