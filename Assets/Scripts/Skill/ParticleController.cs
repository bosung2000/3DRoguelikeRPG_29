using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 크리티컬 파티클 효과를 독립적으로 관리하는 컨트롤러
/// </summary>
public class ParticleController : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 direction;
    private float speed;
    private float lifetime;
    
    public void Init(Vector3 startPos, Vector3 direction, float speed, float lifetime)
    {
        this.startPos = startPos;
        this.direction = direction;
        this.speed = speed;
        this.lifetime = lifetime;
        
        // 애니메이션 시작
        StartCoroutine(AnimateParticle());
    }
    
    private IEnumerator AnimateParticle()
    {
        float elapsed = 0f;
        
        while (elapsed < lifetime)
        {
            transform.position += direction * speed * Time.deltaTime;
            float scale = 0.25f * (1f - elapsed / lifetime);
            transform.localScale = Vector3.one * scale;
            
            // 시간이 지날수록 투명해지는 효과
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Color color = renderer.material.color;
                renderer.material.color = new Color(color.r, color.g, color.b, 1f - (elapsed / lifetime));
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
} 