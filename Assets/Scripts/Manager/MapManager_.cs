using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject normalRoomPrefab;
    [SerializeField] private GameObject eliteRoomPrefab;
    [SerializeField] private GameObject weaponShopPrefab;
    [SerializeField] private GameObject relicShopPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject bossRoomPrefab; // 보스방 프리팹 추가
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject Shop_weapon;
    [SerializeField] private GameObject Shop_Relics;
    [SerializeField] private GameObject Treasure;


    [SerializeField] private Transform mapContainer;
    [SerializeField] Portal _portal;
    
    // 방 인덱스별 RoomZone 참조를 저장할 딕셔너리 추가
    private Dictionary<int, RoomZone> roomZones = new Dictionary<int, RoomZone>();
    
    // 현재 활성화된 특수 오브젝트(상점, 보물 등)
    private GameObject currentActiveObject;
    
    private List<Room> rooms;
    private int currentRoomIndex = 0;  // 현재 플레이어가 위치한 방

    // 방 타입별 개수 설정
    [SerializeField] private int normalRoomCount = 7;
    [SerializeField] private int eliteRoomCount = 4;
    [SerializeField] private int weaponShopCount = 1;
    [SerializeField] private int relicShopCount = 1;
    [SerializeField] private int treasureRoomCount = 1;
    // 보스방은 항상 1개이므로 별도 변수 필요 없음

    private void Awake()
    {
        rooms = new List<Room>();
        // 씬에서 모든 RoomZone을 찾아서 딕셔너리에 저장
        FindAllRoomZones();
    }
    
    // 모든 RoomZone을 찾아서 이름에 따라 딕셔너리에 저장하는 메서드
    private void FindAllRoomZones()
    {
        RoomZone[] allRoomZones = FindObjectsOfType<RoomZone>();
        foreach(RoomZone rz in allRoomZones)
        {
            string roomName = rz.roomName;
            // Room_0, Room_1 형식에서 인덱스 추출
            if(roomName.StartsWith("Room"))
            {
                string[] parts = roomName.Split('_');
                if(parts.Length > 1 && int.TryParse(parts[1], out int roomIndex))
                {
                    roomZones[roomIndex] = rz;
                    Debug.Log($"Room {roomIndex} 매핑됨: {roomName}");
                }
            }
        }
    }

    // 맵 레이아웃 생성
    private void Start()
    {
        _portal = FindObjectOfType<Portal>();
        
        // Awake에서 모든 방이 발견되지 않았을 수 있으므로 한 번 더 확인
        if (roomZones.Count == 0)
        {
            Debug.LogWarning("Start에서 RoomZone 재검색 실행...");
            FindAllRoomZones();
        }
        
        InitializeMapLayout();
        AssignRoomTypes();
        CreateMapUI();
        UpdateAccessibleRooms();
    }

    // 맵 레이아웃 초기화 (고정된 위치와 연결 구조)
    private void InitializeMapLayout()
    {
        // 14개 방의 위치 설정
        Vector2[] positions = new Vector2[]
        {
            new Vector2(0, 0),      // 시작 방 (인덱스 0)
            new Vector2(-2, 1),     // 첫 번째 층 왼쪽
            new Vector2(0, 1),      // 첫 번째 층 중앙
            new Vector2(2, 1),      // 첫 번째 층 오른쪽
            new Vector2(-3, 2),     // 두 번째 층 왼쪽 끝
            new Vector2(-1, 2),     // 두 번째 층 왼쪽 중앙
            new Vector2(1, 2),      // 두 번째 층 오른쪽 중앙
            new Vector2(3, 2),      // 두 번째 층 오른쪽 끝
            new Vector2(-2, 3),     // 세 번째 층 왼쪽
            new Vector2(0, 3),      // 세 번째 층 중앙
            new Vector2(2, 3),      // 세 번째 층 오른쪽
            new Vector2(-1, 4),     // 네 번째 층 왼쪽
            new Vector2(1, 4),      // 네 번째 층 오른쪽
            new Vector2(0, 5)       // 마지막 방 (인덱스 13)
        };

        // 방 생성 및 연결 관계 설정
        for (int i = 0; i < 14; i++)
        {
            Room room = new Room
            {
                position = positions[i],
                isVisited = (i == 0),  // 시작 방은 방문한 상태로 설정
                isAccessible = (i == 0) // 시작 방만 처음에 접근 가능
            };
            rooms.Add(room);
        }

        // 연결 관계 설정 (경로 구성)
        SetupConnections();
    }

    // 방 사이의 연결 관계 설정
    private void SetupConnections()
    {
        //첫번째 방은 시작 방으로 비활성화
        rooms[0].isAccessible = false;
        // 시작 방(0)은 첫 번째 층(1,2,3)과 연결
        rooms[0].connectedRooms.AddRange(new List<int> { 1, 2, 3 });

        // 첫 번째 층과 두 번째 층 연결
        rooms[1].connectedRooms.AddRange(new List<int> { 4, 5 });
        rooms[2].connectedRooms.AddRange(new List<int> { 5, 6 });
        rooms[3].connectedRooms.AddRange(new List<int> { 6, 7 });

        // 두 번째 층과 세 번째 층 연결
        rooms[4].connectedRooms.Add(8);
        rooms[5].connectedRooms.AddRange(new List<int> { 8, 9 });
        rooms[6].connectedRooms.AddRange(new List<int> { 9, 10 });
        rooms[7].connectedRooms.Add(10);

        // 세 번째 층과 네 번째 층 연결
        rooms[8].connectedRooms.Add(11);
        rooms[9].connectedRooms.AddRange(new List<int> { 11, 12 });
        rooms[10].connectedRooms.Add(12);

        // 네 번째 층과 마지막 방 연결
        rooms[11].connectedRooms.Add(13);
        rooms[12].connectedRooms.Add(13);

        // 양방향 연결 설정 (선택 사항)
        //SetupBidirectionalConnections();
    }

    // 양방향 연결 설정
    private void SetupBidirectionalConnections()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (int connectedRoom in rooms[i].connectedRooms)
            {
                if (!rooms[connectedRoom].connectedRooms.Contains(i))
                {
                    rooms[connectedRoom].connectedRooms.Add(i);
                }
            }
        }
    }

    // 방 타입 할당 (요구사항에 맞게 랜덤 배치)
    private void AssignRoomTypes()
    {
        // 시작 방은 항상 Normal로 설정
        rooms[0].type = RoomType.Normal;

        // 마지막 방(13)은 항상 보스방으로 설정
        rooms[13].type = RoomType.Boss;

        List<RoomType> roomTypes = new List<RoomType>();

        // 타입별 방 개수 설정
        for (int i = 0; i < normalRoomCount - 1; i++) roomTypes.Add(RoomType.Normal);
        for (int i = 0; i < eliteRoomCount; i++) roomTypes.Add(RoomType.Elite);
        for (int i = 0; i < weaponShopCount; i++) roomTypes.Add(RoomType.WeaponShop);
        for (int i = 0; i < relicShopCount; i++) roomTypes.Add(RoomType.RelicShop);
        for (int i = 0; i < treasureRoomCount; i++) roomTypes.Add(RoomType.Treasure);

        // 나머지 방들에 랜덤으로 타입 할당
        System.Random random = new System.Random();
        for (int i = 1; i < rooms.Count - 1; i++)
        {
            int randomIndex = random.Next(0, roomTypes.Count);
            rooms[i].type = roomTypes[randomIndex];
            roomTypes.RemoveAt(randomIndex);
        }
    }

    // UI 생성 - 방 아이콘과 연결선
    private void CreateMapUI()
    {
        // 방 UI 생성
        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject prefab = GetRoomPrefab(rooms[i].type);
            GameObject roomObj = Instantiate(prefab, mapContainer);
            roomObj.transform.localPosition = new Vector3(rooms[i].position.x * 150, rooms[i].position.y * 150, 0);

            // 버튼 컴포넌트 설정
            Button button = roomObj.GetComponent<Button>();
            int roomIndex = i;
            button.onClick.AddListener(() => OnRoomSelected(roomIndex));

            // Room 객체에 UI 참조 저장
            rooms[i].uiObject = roomObj;

            // 초기에 접근 불가능한 방은 비활성화
            UpdateRoomUI(i);
        }

        
    }

    // 방 타입에 맞는 프리팹 반환
    private GameObject GetRoomPrefab(RoomType type)
    {
        switch (type)
        {
            case RoomType.Normal: return normalRoomPrefab;
            case RoomType.Elite: return eliteRoomPrefab;
            case RoomType.WeaponShop: return weaponShopPrefab;
            case RoomType.RelicShop: return relicShopPrefab;
            case RoomType.Treasure: return treasurePrefab;
            case RoomType.Boss: return bossRoomPrefab; // 보스방 프리팹 반환 추가
            default: return normalRoomPrefab;
        }
    }

    // 연결선 생성
    private void CreateConnectionLines()
    {
        // 이미 생성된 선들을 관리할 리스트 (필요시)
        List<GameObject> connectionLines = new List<GameObject>();

        // 각 방에서 연결된 방들로 선 그리기
        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (int connectedRoomIndex in rooms[i].connectedRooms)
            {
                // 중복 선 방지 (i < connectedRoomIndex인 경우만 선 그리기)
                if (i < connectedRoomIndex)
                {
                    GameObject line = Instantiate(linePrefab, mapContainer);
                    LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

                    Vector3 startPos = new Vector3(rooms[i].position.x * 150, rooms[i].position.y * 150, 0);
                    Vector3 endPos = new Vector3(rooms[connectedRoomIndex].position.x * 150, rooms[connectedRoomIndex].position.y * 150, 0);

                    lineRenderer.SetPosition(0, startPos);
                    lineRenderer.SetPosition(1, endPos);

                    connectionLines.Add(line);
                }
            }
        }
    }

    // 접근 가능한 방 업데이트
    private void UpdateAccessibleRooms()
    {
        
        // 현재 방에서 연결된 방들을 접근 가능하게 설정
        foreach (int connectedRoomIndex in rooms[currentRoomIndex].connectedRooms)
        {
            rooms[connectedRoomIndex].isAccessible = true;
            UpdateRoomUI(connectedRoomIndex);
        }
    }

    // 방 UI 업데이트 (활성/비활성 상태)
    private void UpdateRoomUI(int roomIndex)
    {
        if (rooms[roomIndex].uiObject != null)
        {
            Button button = rooms[roomIndex].uiObject.GetComponent<Button>();
            button.interactable = rooms[roomIndex].isAccessible;

            // 방문한 방은 색상 변경 등 시각적 표시 (선택사항)
            if (rooms[roomIndex].isVisited)
            {
                // 방문한 방 표시 코드
            }

            // 보스방 특별 표시
            if (rooms[roomIndex].type == RoomType.Boss)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    // 보스방 강조 (예시)
                    image.color = new Color(1f, 0.5f, 0.5f);
                }
            }
        }
    }

    // 방 선택 시 호출될 메서드
    public void OnRoomSelected(int roomIndex)
    {
        if (rooms[roomIndex].isAccessible)
        {
            // 보스방 입장 전 확인 (필요시)
            //if (rooms[roomIndex].type == RoomType.Boss && !CanEnterBossRoom())
            //{
            //    Debug.Log("보스방 입장 조건을 충족하지 못했습니다!");
            //    return;
            //}
            
            // 이전 방에서 연결된 모든 방 비활성화
            foreach (int connectedRoom in rooms[currentRoomIndex].connectedRooms)
            {
                rooms[connectedRoom].isAccessible = false;
                UpdateRoomUI(connectedRoom);
            }
            
            // 시작 방(0번째 방) 추가 처리 - 첫 번째 층(1,2,3)에 도달했을 때 0번 방 비활성화
            if (roomIndex == 1 || roomIndex == 2 || roomIndex == 3)
            {
                rooms[0].isAccessible = false;
                UpdateRoomUI(0);
                Debug.Log("시작 방 접근 비활성화");
            }
            
            // 현재 방 업데이트
            currentRoomIndex = roomIndex;
            rooms[currentRoomIndex].isVisited = true;
            UpdateRoomUI(currentRoomIndex);
            
            // 새로운 방에서 접근 가능한 방 활성화
            UpdateAccessibleRooms();
            
            // 선택한 방 타입에 따른 게임 로직 처리
            HandleRoomAction(roomIndex);
        }
    }

    // 보스방 입장 조건 확인 메서드 (게임 로직에 맞게 수정)
    private bool CanEnterBossRoom()
    {
        // 방문한 방의 개수가 일정 이상인지 확인 (예시)
        int visitedCount = 0;
        foreach (Room room in rooms)
        {
            if (room.isVisited) visitedCount++;
        }
        
        // 예: 최소 7개의 방을 방문해야 보스방 입장 가능
        return visitedCount >= 7;
        
        // 또는 항상 입장 가능하게 설정
        // return true;
    }
        
    // 방 타입에 따른 게임 로직 처리
    private void HandleRoomAction(int roomIndex)
    {
        _portal.TryUsePortal($"potal_{roomIndex}");
        UIManager.Instance.ClosePopupUI<UIMap>();
        
        // 이전에 생성된 특수 오브젝트가 있으면 파괴
        CleanupCurrentActiveObject();
        
        // 선택한 방 인덱스에 해당하는 RoomZone 찾기
        if(roomZones.TryGetValue(roomIndex, out RoomZone targetRoom))
        {
            // 해당 방의 타입에 맞는 처리
            switch (rooms[roomIndex].type)
            {
                case RoomType.Normal:
                    targetRoom.ActivateRoom();
                    break;
                case RoomType.Elite:
                    // 엘리트용 설정 적용 후 방 활성화
                    targetRoom.spawnConfig.spawnElite = true;
                    targetRoom.ActivateRoom();
                    break;
                case RoomType.WeaponShop:
                    // 무기 상점 생성
                    if (Shop_weapon != null)
                    {
                        currentActiveObject = SpawnPrefabAtRandomPoint(Shop_weapon, targetRoom);
                        Debug.Log("무기 상점 생성됨");
                    }
                    else
                    {
                        Debug.LogError("무기 상점 프리팹이 설정되지 않았습니다!");
                    }
                    break;
                case RoomType.RelicShop:
                    // 유물 상점 생성
                    if (Shop_Relics != null)
                    {
                        currentActiveObject = SpawnPrefabAtRandomPoint(Shop_Relics, targetRoom);
                        Debug.Log("유물 상점 생성됨");
                    }
                    else
                    {
                        Debug.LogError("유물 상점 프리팹이 설정되지 않았습니다!");
                    }
                    break;
                case RoomType.Treasure:
                    // 보물 생성
                    if (Treasure != null)
                    {
                        currentActiveObject = SpawnPrefabAtRandomPoint(Treasure, targetRoom);
                        Debug.Log("보물 생성됨");
                    }
                    else
                    {
                        Debug.LogError("보물 프리팹이 설정되지 않았습니다!");
                    }
                    break;
                case RoomType.Boss:
                    // 보스 방 활성화
                    targetRoom.spawnConfig.spawnBoss = true;
                    targetRoom.ActivateRoom();
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError($"Room_{roomIndex}에 해당하는 RoomZone을 찾을 수 없습니다!");
        }
    }

    // RoomZone의 spawnPoints 중에서 랜덤으로 하나 선택하여 프리팹 생성
    private GameObject SpawnPrefabAtRandomPoint(GameObject prefab, RoomZone targetRoom)
    {
        if (targetRoom == null)
        {
            Debug.LogError("타겟 룸이 null입니다!");
            return null;
        }

        if (targetRoom.spawnPoints == null || targetRoom.spawnPoints.Count == 0)
        {
            Debug.LogError($"방 '{targetRoom.roomName}'에 spawnPoints가 없습니다!");
            return null;
        }

        // spawnPoints 중에서 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, targetRoom.spawnPoints.Count);
        Transform spawnPoint = targetRoom.spawnPoints[randomIndex];
        
        if (spawnPoint == null)
        {
            Debug.LogError("선택된 스폰 포인트가 null입니다!");
            return null;
        }

        // 선택된 위치에 프리팹 생성
        Vector3 spawnPosition = spawnPoint.position;
        // y축으로 -12만큼 내림
        spawnPosition.y -= 3f;
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnPoint.rotation);
        
        Debug.Log($"프리팹 '{prefab.name}' 생성됨 - 위치: {spawnPosition}, 방: {targetRoom.roomName}, 스폰 포인트 인덱스: {randomIndex}");
        
        return spawnedObject;
    }

    // 보스 전투 시작 메서드
    private void StartBossBattle()
    {
        // 보스 전투 관련 로직 구현
        // 예: 보스 씬 로드 또는 보스 스폰
        Debug.Log("보스 전투 시작!");

        // 보스 전투 UI 표시 또는 보스 전투 씬 로드
        // SceneManager.LoadScene("BossBattle");

        // 또는 이벤트 발생
        // OnBossBattleStart?.Invoke();
    }

    // 보스 처치 후 게임 종료/다음 단계 진행 메서드 (필요시)
    public void OnBossDefeated()
    {
        Debug.Log("보스 처치! 게임 클리어 또는 다음 단계로 진행");
        // 게임 클리어 UI 표시 또는 다음 레벨 진행
    }

    // 현재 활성화된 특수 오브젝트 정리
    private void CleanupCurrentActiveObject()
    {
        if (currentActiveObject != null)
        {
            Debug.Log($"이전 오브젝트 파괴: {currentActiveObject.name}");
            Destroy(currentActiveObject);
            currentActiveObject = null;
        }
    }
}