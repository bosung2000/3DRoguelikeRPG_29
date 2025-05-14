using System.Collections;
using UnityEngine;

/// <summary>
/// 크리티컬 히트 이펙트를 독립적으로 관리하는 컨트롤러
/// </summary>
public class CriticalEffectController : MonoBehaviour
{
    private float duration;
    private float startScale;
    private float endScale;
    private Vector3 position;
    
    public void Init(Vector3 position, float duration)
    {
        this.position = position;
        this.duration = duration;
        this.startScale = 0.8f;
        this.endScale = 1.5f;
        
        // 파티클 생성
        StartCoroutine(CreateCriticalParticles());
        // 이펙트 애니메이션 시작
        StartCoroutine(AnimateEffect());
    }
    
    private IEnumerator AnimateEffect()
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Mathf.Lerp(startScale, endScale, t);
            
            transform.localScale = Vector3.one * scale;
            
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = new Color(1f, 0f, 0f, Mathf.Lerp(1f, 0f, t));
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    private IEnumerator CreateCriticalParticles()
    {
        int particleCount = 15;
        float speed = 8f;
        float lifetime = 0.4f;
        
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (particle != null) 
            {
                particle.transform.position = position;
                particle.transform.localScale = Vector3.one * 0.25f;
                
                Destroy(particle.GetComponent<Collider>());
                
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(1f, 0f, 0f, 1f);
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", new Color(1f, 0f, 0f) * 5f);
                
                Renderer renderer = particle.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = material;
                }
                
                // 파티클에 자체 스크립트 추가
                ParticleController particleController = particle.AddComponent<ParticleController>();
                particleController.Init(position, randomDir, speed, lifetime);
            }
            
            yield return new WaitForSeconds(0.01f);
        }
    }
} 