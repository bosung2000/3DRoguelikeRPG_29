using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public HashSet<string> _unlockedPortals = new HashSet<string>();

    //포탈 조건 충족
    public void UnlockPortal(string portalID)
    {
        _unlockedPortals.Add(portalID);
    }

    //포탈 사용 가능 여부
    public bool IsPortalUnlocked(string portalID)
    {
        return _unlockedPortals.Contains(portalID);
    }
}
