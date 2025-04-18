using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private void Start()
    {
        Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        enemyPrefab.SetActive(true);
    }
}
