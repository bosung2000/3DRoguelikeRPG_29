using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabSkill : MeleeSkillBase
{
    [SerializeField] private float stabRange = 3f; // 찌르기 사거리
    [SerializeField] private float stabWidth = 0.5f; // 찌르기 폭
    [SerializeField] private float stabSpeed = 15f; // 찌르기 속도



    protected override IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;

        // 방향 정규화
        direction = direction.normalized;

        // 찌르기 시작 위치 (플레이어 앞, 약간 위로)
        Vector3 startPos = player.transform.position + Vector3.up * 0.5f;
        Vector3 endPos = startPos + direction * stabRange;

        // 캡슐 캐스트로 적 탐색
        RaycastHit[] hits = Physics.CapsuleCastAll(
            startPos,
            endPos,
            stabWidth * 0.5f,
            direction,
            stabRange,
            targetLayer
        );

        foreach (var hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(skillData.value);
                ShowStabHitEffect(hit.point);
            }
        }

        // 찌르기 이펙트 표시
        ShowStabEffect(startPos, direction);

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    private void ShowStabEffect(Vector3 startPos, Vector3 direction)
    {
        // 찌르기 궤적 이펙트
        GameObject stabEffect = new GameObject("StabEffect");
        stabEffect.transform.position = startPos;
        stabEffect.transform.rotation = Quaternion.LookRotation(direction);

        LineRenderer line = stabEffect.AddComponent<LineRenderer>();
        line.startWidth = stabWidth;
        line.endWidth = stabWidth;
        line.material = new Material(Shader.Find("Standard"));
        line.material.color = new Color(1f, 0.2f, 0.2f, 0.8f);
        line.material.EnableKeyword("_EMISSION");
        line.material.SetColor("_EmissionColor", Color.red * 2f);

        Vector3[] positions = new Vector3[2];
        positions[0] = startPos;
        positions[1] = startPos + direction * stabRange;
        line.positionCount = positions.Length;
        line.SetPositions(positions);

        // 찌르기 잔상 이펙트
        StartCoroutine(CreateStabAfterimage(startPos, direction));

        Destroy(stabEffect, 0.3f);
    }

    private IEnumerator CreateStabAfterimage(Vector3 startPos, Vector3 direction)
    {
        int afterimageCount = 5;
        float interval = 0.05f;

        for (int i = 0; i < afterimageCount; i++)
        {
            GameObject afterimage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            afterimage.transform.position = startPos + direction * (stabRange * (i + 1) / afterimageCount);
            afterimage.transform.rotation = Quaternion.LookRotation(direction);
            afterimage.transform.localScale = new Vector3(stabWidth, stabWidth, 0.2f);

            Destroy(afterimage.GetComponent<Collider>());

            Material material = new Material(Shader.Find("Standard"));
            float alpha = 0.7f - (i * 0.1f);
            material.color = new Color(1f, 0.3f, 0.3f, alpha);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.red * (1.0f - i * 0.15f));
            afterimage.GetComponent<Renderer>().material = material;

            Destroy(afterimage, 0.2f);
            yield return new WaitForSeconds(interval);
        }
    }

    private void ShowStabHitEffect(Vector3 position)
    {
        // 찌르기 특유의 관통 이펙트
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.4f;

        Destroy(hitEffect.GetComponent<Collider>());

        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.2f, 0.2f, 0.9f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.red * 4f);
        hitEffect.GetComponent<Renderer>().material = material;

        // 관통 파티클 효과
        StartCoroutine(StabHitParticles(position));

        Destroy(hitEffect, 0.3f);
    }

    private IEnumerator StabHitParticles(Vector3 position)
    {
        int particleCount = 8;
        float speed = 5f;
        float lifetime = 0.3f;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.1f, 0.5f),
                Random.Range(-1f, 1f)
            ).normalized;

            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * 0.15f;

            Destroy(particle.GetComponent<Collider>());

            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(1f, 0.1f, 0.1f, 0.8f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.red * 2f);
            particle.GetComponent<Renderer>().material = material;

            StartCoroutine(MoveParticle(particle, position, randomDir, speed, lifetime));
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator MoveParticle(GameObject particle, Vector3 startPos, Vector3 direction, float speed, float lifetime)
    {
        float elapsed = 0f;

        while (elapsed < lifetime && particle != null)
        {
            particle.transform.position += direction * speed * Time.deltaTime;
            float scale = 0.15f * (1f - elapsed / lifetime);
            particle.transform.localScale = Vector3.one * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (particle != null)
        {
            Destroy(particle);
        }
    }
}
