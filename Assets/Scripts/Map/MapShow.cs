using UnityEngine;
using UnityEngine.UI;

public class MapShow : MonoBehaviour
{

    [SerializeField] private RoomZone CurrentroomZone;
    [SerializeField] private Button MapBtn;

    void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (CurrentroomZone.ClearOK())
        {
            MapBtn.gameObject.SetActive(true);
        }

        if ((Input.GetKeyDown(KeyCode.E))
            && CurrentroomZone.ClearOK()
            )
        {
            UIManager.Instance.ShowPopupUI<UIMap>();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (CurrentroomZone.ClearOK())
        {
            MapBtn.gameObject.SetActive(false);
        }
    }




}
