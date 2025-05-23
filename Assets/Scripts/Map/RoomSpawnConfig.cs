using UnityEngine;

[System.Serializable]
public class RoomSpawnConfig
{
    //public string configName;

    [Header("일반 근접 몬스터 설정")]
    public GameObject normalMeleeEnemyPrefab;
    public bool spawnnormalMelee;
    public int normalMeleeCount;

    [Header("일반 원거리 몬스터 설정")]
    public GameObject normalRangedEnemyPrefab;
    public bool spawnnormalRanged;
    public int normalRangedCount;


    [Header("엘리트 원거리 몬스터 설정")]
    public GameObject eliteRangeEnemyPrefab;
    public bool spawnEliteRange;
    public int EliteCountRange;

    [Header("엘리트 근거리 몬스터 설정")]
    public GameObject eliteMeleeEnemyPrefab;
    public bool spawnEliteMelee;
    public int EliteCountMelee;

    [Header("보스 몬스터 설정")]
    public GameObject bossEnemyPrefab;
    public int BossCount;
    public bool spawnBoss;
}
