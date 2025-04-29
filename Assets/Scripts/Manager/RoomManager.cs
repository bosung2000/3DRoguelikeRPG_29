using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomZone startingRoom;

    private void Start()
    {
        if (startingRoom != null)
        {
            startingRoom.ActivateRoom();
        }
    }
}
