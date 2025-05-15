using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private Queue<(float intensity, float duration)> shakeQueue = new Queue<(float, float)>();
    private bool isShaking = false;

    [SerializeField] Image _takeDamageImage;

    private Coroutine damageRoutine;

    private Coroutine zoomRoutine;
    private float originalFOV;

    private void Awake()
    {
        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        originalFOV = virtualCam.m_Lens.FieldOfView;
    }
    public void ShakeCamera(float intensity, float duration)
    {
        shakeQueue.Enqueue((intensity, duration));
        if (!isShaking)
        {
            StartCoroutine(DoShake());
        }
    }

    private IEnumerator DoShake()
    {
        isShaking = true;

        while (shakeQueue.Count > 0)
        {
            var (intensity, duration) = shakeQueue.Dequeue();

            noise.m_AmplitudeGain = intensity;
            noise.m_FrequencyGain = 2f;

            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0f;
        }

        isShaking = false;
    }
    public void Zoom(float zoomedFOV, float duration)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(DoZoom(zoomedFOV, duration));
    }
    private IEnumerator DoZoom(float zoomedFOV, float duration)
    {
        float elapsed = 0f;
        float startFOV = virtualCam.m_Lens.FieldOfView;

        // 줌 인
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, zoomedFOV, t);
            yield return null;
        }

        elapsed = 0f;

        // 줌 아웃
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(zoomedFOV, originalFOV, t);
            yield return null;
        }

        virtualCam.m_Lens.FieldOfView = originalFOV;
        zoomRoutine = null;
    }
    public void DamageEffect(float duration = 0.5f)
    {
        if (_takeDamageImage == null) return;

        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damageRoutine = StartCoroutine(DoDamageFlash(duration));
    }
    private IEnumerator DoDamageFlash(float duration)
    {
        _takeDamageImage.color = new Color(1f, 0f, 0f, 0.3f);

        float fade = 0.3f;
        while (fade > 0f)
        {
            fade -= Time.deltaTime * (0.3f / duration);
            _takeDamageImage.color = new Color(1f, 0f, 0f, Mathf.Clamp01(fade));
            yield return null;
        }

        _takeDamageImage.color = new Color(1f, 0f, 0f, 0f);
    }
    public void ShakeAndDamage(float shakeIntensity, float shakeDuration, float damageFlashDuration)
    {
        ShakeCamera(shakeIntensity, shakeDuration);
        DamageEffect(damageFlashDuration);
    }
}
