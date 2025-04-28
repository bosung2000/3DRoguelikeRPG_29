using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkill : MonoBehaviour
{
    public Vector3 HalfMoon(Skill skill, Vector3 direction, Player player)
    {
        // 방향 벡터 정규화
        direction = direction.normalized;

        // 근접 스킬 로직
        Vector3 center = player.transform.position;

        // 스킬 범위 설정
        float attackRange = skill.attackRange;

        // 부채꼴 범위 공격을 위한 각도 계산
        float angle = 90f; // 90도 부채꼴
        Vector3 forward = direction;
        Debug.Log($"부채꼴 범위 설정: 각도={angle}도, 전방벡터={forward}");

        // 스킬 범위 시각화 (게임 내에서 실제로 보이는 효과)
        ShowMeleeSkillRange(center, forward, attackRange, angle);
        Debug.Log("스킬 범위 시각화 완료");

        // 박스 감지를 위한 설정
        // 플레이어 위치에서 스킬 범위의 절반 거리만큼 앞으로 이동한 위치를 박스 중심으로 설정
        Vector3 searchCenter = center + forward * (attackRange * 0.5f);

        // 박스 크기 설정 - 반드시 반쪽 크기로 지정
        Vector3 halfExtents = new Vector3(attackRange * 0.5f, 1f, attackRange * 0.5f);

        // 박스 회전 설정
        Quaternion orientation = Quaternion.LookRotation(direction);

        // 범위 내 적 탐색
        Collider[] hitColliders = Physics.OverlapBox(searchCenter,
                                                   halfExtents,
                                                   orientation,
                                                   LayerMask.GetMask("Enemy"));

        Debug.Log($"감지된 적의 수: {hitColliders.Length}");
        Debug.Log($"박스캐스트 위치: {searchCenter}, 크기: {halfExtents}");

        int enemiesHit = 0;
        foreach (var hitCollider in hitColliders)
        {
            // 각도 체크 (부채꼴 범위 내에 있는지)
            Vector3 dirToTarget = (hitCollider.transform.position - center).normalized;
            float angleToTarget = Vector3.Angle(forward, dirToTarget);

            if (angleToTarget <= angle * 0.5f) // 부채꼴 각도의 절반과 비교해야 함
            {
                // 적에게 데미지 적용
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log($"데미지 적용: {hitCollider.name}에게 {skill.value} 데미지");
                    enemy.TakeDamage(skill.value);
                    enemiesHit++;

                    // 히트 이펙트 생성
                    ShowHitEffect(enemy.transform.position);
                }
            }
        }

        // 이펙트 생성 (필요시)
        if (skill.effectPrefab != null)
        {
            // 이펙트 위치 플레이어 앞으로 조정 - 플레이어 바로 앞에서 시작하도록 수정
            Vector3 effectPosition = center + forward * (attackRange * 0.5f);
            effectPosition.y += 0.5f; // 약간 위로 올려서 바닥에 묻히지 않게

            GameObject effect = Instantiate(skill.effectPrefab,
                effectPosition,
                Quaternion.LookRotation(direction));

            // 이펙트 크기 증가 (더 잘 보이게)
            effect.transform.localScale = effect.transform.localScale * 1.5f;

            Destroy(effect, 2f);
        }

        // Gizmos 범위 표시 정보 업데이트
        lastMeleeCenter = center;
        lastMeleeDirection = forward;
        lastMeleeRange = attackRange;
        lastMeleeAngle = angle;
        showLastMeleeRange = true;

        // 5초 후 Gizmos 표시 숨김
        StartCoroutine(HideGizmosAfterDelay(5f));
        return direction;
    }

    // 근접 스킬 범위 시각화 (게임 내에서 보이는 효과)
    private void ShowMeleeSkillRange(Vector3 center, Vector3 direction, float range, float angle)
    {
        // 일시적인 바닥 범위 표시기 생성
        GameObject rangeIndicator = new GameObject("MeleeSkillRangeIndicator");

        // 플레이어 바로 앞에서 시작하여 범위의 중간 지점에 지시자 배치
        Vector3 indicatorPosition = center + direction * (range * 0.5f);
        indicatorPosition.y = 0.1f; // 바닥 위 약간 띄움
        rangeIndicator.transform.position = indicatorPosition;

        // 방향에 맞춰 회전
        rangeIndicator.transform.rotation = Quaternion.LookRotation(direction);

        // 메시 생성 시 원점을 조정하여 플레이어 위치에서 시작하도록 설정
        MeshFilter meshFilter = rangeIndicator.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = rangeIndicator.AddComponent<MeshRenderer>();

        // 반투명 재질 설정
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.3f, 0f, 0.5f); // 주황색 반투명
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        // 메시 생성 (플레이어 위치에서 시작하도록 조정된 부채꼴)
        Mesh mesh = CreateArcMesh(range, angle, true); // 플레이어 시작점 파라미터 추가
        meshFilter.mesh = mesh;

        // 잠시 후 제거
        Destroy(rangeIndicator, 0.7f);

        // 범위 외곽선 효과 추가 (플레이어 위치부터 시작하도록)
        StartCoroutine(CreateRangeOutline(center, direction, range, angle));
    }

    // 부채꼴 메시 생성
    private Mesh CreateArcMesh(float radius, float angle, bool startFromPlayer = false)
    {
        Mesh mesh = new Mesh();

        int segments = 30;
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 중앙 정점 - 플레이어 위치에서 시작하도록 조정
        vertices[0] = startFromPlayer ? new Vector3(0, 0, -radius * 0.5f) : Vector3.zero;

        // 원호 정점들 조정
        for (int i = 0; i <= segments; i++)
        {
            float segmentAngle = -halfAngle + ((float)i / segments) * 2 * halfAngle;
            float x = Mathf.Sin(segmentAngle) * radius;
            float z = Mathf.Cos(segmentAngle) * radius;

            if (startFromPlayer)
                z -= radius * 0.5f; // 플레이어 위치에서 시작하도록 z값 조정

            vertices[i + 1] = new Vector3(x, 0, z);
        }

        // 삼각형 인덱스는 동일
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // 범위 외곽선 효과
    private IEnumerator CreateRangeOutline(Vector3 center, Vector3 direction, float range, float angle)
    {
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
        int segments = 10;

        // 왼쪽 경계 방향
        Vector3 leftDir = Quaternion.Euler(0, -angle * 0.5f, 0) * direction;
        // 오른쪽 경계 방향
        Vector3 rightDir = Quaternion.Euler(0, angle * 0.5f, 0) * direction;

        // 외곽선 효과 생성
        for (int i = 0; i < 5; i++) // 깜빡임 효과
        {
            // 경계선 파티클 생성
            CreateLineEffect(center, center + leftDir * range, Color.red);
            CreateLineEffect(center, center + rightDir * range, Color.red);

            // 호 부분 파티클
            Vector3 prevPoint = center + leftDir * range;
            for (int j = 1; j <= segments; j++)
            {
                float segmentAngle = -halfAngle + ((float)j / segments) * 2 * halfAngle;
                Vector3 dir = Quaternion.Euler(0, segmentAngle * Mathf.Rad2Deg, 0) * direction;
                Vector3 point = center + dir * range;

                CreateLineEffect(prevPoint, point, Color.red);
                prevPoint = point;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // 디버그 용 Gizmos 그리기 (에디터에서만 보임)
    private Vector3 lastMeleeCenter;
    private Vector3 lastMeleeDirection;
    private float lastMeleeRange;
    private float lastMeleeAngle;
    private bool showLastMeleeRange = false;

    private void OnDrawGizmos()
    {
        if (showLastMeleeRange)
        {
            // 부채꼴 범위 그리기
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // 빨간색 반투명

            // 부채꼴의 중심 표시
            Gizmos.DrawSphere(lastMeleeCenter, 0.2f);

            // 부채꼴의 방향 표시
            Gizmos.DrawRay(lastMeleeCenter, lastMeleeDirection * lastMeleeRange);

            // 부채꼴 그리기
            float halfAngle = lastMeleeAngle * 0.5f * Mathf.Deg2Rad;
            int segments = 20;
            Vector3 prevPoint = lastMeleeCenter;

            // 왼쪽 경계선
            Vector3 leftDir = Quaternion.Euler(0, -lastMeleeAngle * 0.5f, 0) * lastMeleeDirection;
            Gizmos.DrawRay(lastMeleeCenter, leftDir * lastMeleeRange);

            // 오른쪽 경계선
            Vector3 rightDir = Quaternion.Euler(0, lastMeleeAngle * 0.5f, 0) * lastMeleeDirection;
            Gizmos.DrawRay(lastMeleeCenter, rightDir * lastMeleeRange);

            // 호 그리기
            for (int i = 0; i <= segments; i++)
            {
                float segmentAngle = -halfAngle + ((float)i / segments) * 2 * halfAngle;
                Vector3 dir = Quaternion.Euler(0, segmentAngle * Mathf.Rad2Deg, 0) * lastMeleeDirection;
                Vector3 point = lastMeleeCenter + dir * lastMeleeRange;

                if (i > 0)
                {
                    Gizmos.DrawLine(prevPoint, point);
                }
                prevPoint = point;
            }
        }
    }

    private IEnumerator HideGizmosAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        showLastMeleeRange = false;
    }

    // 라인 이펙트 생성
    private void CreateLineEffect(Vector3 start, Vector3 end, Color color)
    {
        int segments = 10;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y = 0.1f; // 바닥 위에 표시

            GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dot.transform.position = pos;
            dot.transform.localScale = Vector3.one * 0.15f;

            // 콜라이더 제거
            Destroy(dot.GetComponent<Collider>());

            // 파티클 효과 같은 재질
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 2f);
            dot.GetComponent<Renderer>().material = material;

            // 잠시 후 사라짐
            Destroy(dot, 0.2f);
        }
    }

    private void ShowHitEffect(Vector3 position)
    {
        // 간단한 히트 이펙트 (필요에 따라 구현)
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.5f; // 더 크게 설정

        // 콜라이더 제거 (충돌 방지)
        Destroy(hitEffect.GetComponent<Collider>());

        // 재질 설정 - 더 눈에 띄는 색상으로
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.1f, 0.1f, 0.8f); // 진한 빨간색
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.red * 3f); // 더 강한 발광
        hitEffect.GetComponent<Renderer>().material = material;

        // 크기 변화 효과 추가
        StartCoroutine(PulseEffect(hitEffect));

        // 짧은 시간 후 제거
        Destroy(hitEffect, 0.4f); // 좀 더 오래 지속
    }

    // 펄스 효과 (크기가 커졌다 작아졌다 하는 효과)
    private IEnumerator PulseEffect(GameObject obj)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        while (elapsed < duration && obj != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 사인 파형으로 크기 변화 (0->1->0)
            float scale = 1f + 0.5f * Mathf.Sin(t * Mathf.PI);
            obj.transform.localScale = originalScale * scale;

            yield return null;
        }
    }
}
