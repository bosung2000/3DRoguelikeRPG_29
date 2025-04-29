using System.Collections.Generic;
using UnityEngine;

public class RoomZone : MonoBehaviour
{
    [Header("Room 설정")]
    public string roomName;
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints;
    public RoomZone nextRoom;

    private List<Enemy> spawnedEnemies = new();

    public void ActivateRoom()
    {
        Debug.Log($"[RoomZone] {roomName} 시작됨");
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (var point in spawnPoints)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, point.position, Quaternion.identity);
            Enemy enemy = enemyGO.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.OnDeath += OnEnemyDeath;
                spawnedEnemies.Add(enemy);
            }
            else
            {
                Debug.LogWarning("Enemy 프리팹에 Enemy 스크립트가 없습니다.");
            }
        }

        if (spawnedEnemies.Count == 0 && nextRoom != null)
        {
            nextRoom.ActivateRoom(); //적이 없을 경우 바로 다음 방으로
        }
    }

    private void OnEnemyDeath(Enemy dead)
    {
        spawnedEnemies.Remove(dead);

        if (spawnedEnemies.Count == 0)
        {
            Debug.Log($"[RoomZone] {roomName} 클리어!");
            nextRoom?.ActivateRoom();
        }
    }
}
