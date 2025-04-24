using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform targetSpawnPoint; //이동할위치
    public string requiredCondition; //조건 -> 게임매니저랑 연동

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //if 게임매니저 인스턴스 사용
        }
    }
}


