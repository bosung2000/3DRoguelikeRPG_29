using System.Collections.Generic;
using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    [SerializeField] private string portalIDToUnlock;
    private List<Enemy> enemies;

    private void Start()
    {
        enemies = new List<Enemy>();
        // 자식 오브젝트의 Enemy 컴포넌트 자동 등록
        enemies.AddRange(GetComponentsInChildren<Enemy>());

        foreach (var enemy in enemies)
        {
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        
        enemies.Remove(deadEnemy);

        if (enemies.Count == 0)
        {           
            GameManager.Instance.PortalManager.UnlockPortal(portalIDToUnlock);
        }
    }
}
