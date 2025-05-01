using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomZone : MonoBehaviour
{
    public string roomName;
    public List<Transform> spawnPoints;
    public RoomZone nextRoom;
    public RoomSpawnConfig spawnConfig;
    public float spawnInterval = 2f; //일반 몬스터 반복 소환 간격

    private List<Enemy> spawnedEnemies = new();

    public void ActivateRoom()
    {
        Debug.Log($"[RoomZone] {roomName} 시작됨");
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        // 일반 몬스터 순차 생성
        for (int i = 0; i < spawnConfig.normalCount; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnEnemy(spawnConfig.normalEnemyPrefab, point.position);
            yield return new WaitForSeconds(spawnInterval);
        }

        // 엘리트 몬스터
        if (spawnConfig.spawnElite && spawnConfig.eliteEnemyPrefab != null)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnEnemy(spawnConfig.eliteEnemyPrefab, point.position);
        }

        // 보스 몬스터
        if (spawnConfig.spawnBoss && spawnConfig.bossEnemyPrefab != null)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnEnemy(spawnConfig.bossEnemyPrefab, point.position);
        }

        if (spawnedEnemies.Count == 0 && nextRoom != null)
        {
            nextRoom.ActivateRoom();
        }
    }

    private void SpawnEnemy(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject enemyGO = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();

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
            Debug.Log($"[RoomZone] {roomName} 클리어!");
            nextRoom?.ActivateRoom();
        }
    }
}