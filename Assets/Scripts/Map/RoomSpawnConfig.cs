using UnityEngine;

[System.Serializable]
public class RoomSpawnConfig
{
    public string configName;

    [Header("일반 몬스터 설정")]
    public GameObject normalEnemyPrefab;
    public int normalCount;

    [Header("엘리트 몬스터 설정")]
    public GameObject eliteEnemyPrefab;
    public bool spawnElite;

    [Header("보스 몬스터 설정")]
    public GameObject bossEnemyPrefab;
    public bool spawnBoss;
}
