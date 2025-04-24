using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask levelCollisionLayer; //읽어온 뒤 데미지를 적용할 대상의 레이어
    private float currentDuration; //지나갈 시간
    private Vector3 direction; //방향
    private SpriteRenderer spriteRenderer; //
    [SerializeField] private Rigidbody _rigidbody; //리지드바디
    public float Duration; //투사체 지속시간
    public float ProjectileSpeed; //투사체 속도
    public LayerMask target; //충돌했을때 읽어올 레이어
    public bool fxOnDestroy; //없어질 때 이펙트를 호출하는 투사체인지

    private void Update()
    {
        //투사체의 시간을 흐르게 한 뒤
        currentDuration += Time.deltaTime;

        //투사체의 지속시간보다 더 오래 지속되었을 경우, 오브젝트 삭제
        if (currentDuration > Duration)
        {
            DestroyProjectile(transform.position, false);
        }

        transform.position += direction * ProjectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //레이어를 충돌한 오브젝트의 레이어만큼 계속 밀어서 나온 이진값이 투사체의 레이어와 같을 경우
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            //투사체 파괴, 이 경우 벽에 부딪힌거라 충돌지점에서 살짝 뒤로 빠진 위치에서 파티클 생성
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * 0.2f, fxOnDestroy);
        }
        //레이어를 충돌한 오브젝트의 레이어만큼 계속 밀어서 나온 이진값이 rangeWeaponHandler의 탐색 레이어와 같을 경우
        else if (target.value == (target.value | (1 << collision.gameObject.layer)))
        {
            //해당 개체한테 데미지 적용

            //투사체 파괴, 단. 이 경우 적을 맞춘거니 그 자리에 파티클 생성
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
        }
    }

    public void Init(Vector3 direction)
    {
        this.direction = direction;
        currentDuration = 0;

        //오른쪽 축을 이 스크립트의 방향으로 설정하기
        transform.right = this.direction;
    }

    public void ShootBullet(Vector3 startPosition, Vector3 direction)
    {

        GameObject obj = Instantiate(this.gameObject);
        Collider col = obj.GetComponent<Collider>();


        float offsetDistance = 0.5f; // 기본값
        if (col != null)
        {
            offsetDistance = col.bounds.extents.magnitude*1.5f; // 투사체의 반지름 거리 정도
        }

        Vector3 spawnPosition = startPosition + direction.normalized * offsetDistance;
        obj.transform.position = spawnPosition;
        obj.transform.rotation = Quaternion.identity;

        //그리고 투사체 안에 있는 투사체 제어자를 변수로 지정한 뒤 사격 지시
        SkillProjectile projectile = obj.GetComponent<SkillProjectile>();
        projectile.Init(direction);
    }



    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            //projectileManager.CreateImpactParticlesAtPosition(position, rangeWeaponHandler);
        }
        Destroy(this.gameObject);
    }
}