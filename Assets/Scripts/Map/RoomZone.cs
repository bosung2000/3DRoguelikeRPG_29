using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomZone : MonoBehaviour
{
    public string roomName;
    public List<Transform> spawnPoints;

    public RoomSpawnConfig spawnConfig;
    public float spawnInterval = 0.1f; //일반 몬스터 반복 소환 간격

    public List<Enemy> spawnedEnemies;
    public bool ClearBool;

    private void Awake()
    {



        // 방 이름이 설정되지 않았으면 게임 오브젝트 이름을 사용
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = gameObject.name;
            //Debug.Log($"[RoomZone] 방 이름이 자동 설정됨: {roomName}");
        }
    }

    private void Start()
    {
    }

    public bool ClearOK()
    {
        return ClearBool;
    }

    public void ActivateRoom()
    {
        ClearBool = false;
        spawnedEnemies = new();
        //Debug.Log($"[RoomZone] {roomName} 시작됨");

        //튜토리얼 선택 팝업
        //if (roomName == "Room_0")
        //{
        //    var prompt = UIManager.Instance.ShowPopupUI<UITutorialPrompt>();
        //    prompt.tutorialStartPoint = GameObject.Find("potal_TutorialRoom")?.transform;
        //}

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        //일반몬스터
        if (spawnConfig.spawnnormal && spawnConfig.normalEnemyPrefab != null)
        {
            // 일반 몬스터 순차 생성
            for (int i = 0; i < spawnConfig.normalCount; i++)
            {
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                SpawnEnemy(spawnConfig.normalEnemyPrefab, point.position);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        // 엘리트 몬스터
        if (spawnConfig.spawnElite && spawnConfig.eliteEnemyPrefab != null)
        {
            for (int i = 0; i < spawnConfig.EliteCount; i++)
            {
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                SpawnEnemy(spawnConfig.eliteEnemyPrefab, point.position);
            }
        }
        // 보스 몬스터
        if (spawnConfig.spawnBoss && spawnConfig.bossEnemyPrefab != null)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnEnemy(spawnConfig.bossEnemyPrefab, point.position);
        }

        if (spawnedEnemies.Count == 0)
        {
            ClearBool = true;
            //nextRoom.ActivateRoom();
        }
    }

    private void SpawnEnemy(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject enemyGO = Instantiate(prefab, position, Quaternion.identity, transform);
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        //stage 별로 능력치 증가 수치 수정 

        if (enemy != null)
        {
            enemy.OnDeath += OnEnemyDeath;
            spawnedEnemies.Add(enemy);
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        spawnedEnemies.Remove(enemy);

        if (spawnedEnemies.Count == 0)
        {
            //Debug.Log($"[RoomZone] {roomName} 클리어!");
            ClearBool = true;
            //nextRoom?.ActivateRoom();
        }
    }

    public void CleanupRoom()
    {
        // 생성된 모든 몬스터 제거
        if (spawnedEnemies != null)
        {
            foreach (Enemy enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            spawnedEnemies.Clear();
        }

        // 방 내의 모든 아이템, 골드 등 정리
        // RoomZone의 자식 오브젝트 중 특정 태그를 가진 오브젝트들 정리
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Gold") || child.CompareTag("Soul"))
            {
                Destroy(child.gameObject);
            }
        }

        ClearBool = false;
    }
}