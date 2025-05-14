using System.Collections.Generic;
using UnityEngine;


public enum RoomType
{
    Normal,     // 일반 방
    Elite,      // 엘리트 방
    WeaponShop, // 무기 상점
    RelicShop,  // 유물 상점
    Treasure,   // 보물 방
    Boss        // 보스 방 추가
}

[System.Serializable]
public class Room
{
    public RoomType type;
    public List<int> connectedRooms = new List<int>();
    public Vector2 position;
    public bool isVisited;
    public bool isAccessible;
    public GameObject uiObject;  // 맵에 표시될 UI 오브젝트
}
