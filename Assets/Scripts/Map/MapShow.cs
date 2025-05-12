using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShow : MonoBehaviour
{

    [SerializeField] private RoomZone CurrentroomZone;

    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {

        if (Input.GetKeyDown(KeyCode.E) && CurrentroomZone.ClearOK())
        {
            UIManager.Instance.ShowPopupUI<UIMap>();
        }
    }


}
