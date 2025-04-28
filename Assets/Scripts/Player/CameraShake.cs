using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeRoutine;
    [SerializeField] Image _takeDamageImage;


    private void Awake()
    {
        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera(float intensity, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        
        shakeRoutine = StartCoroutine(DoShake(intensity, duration));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = 2f;

        if (_takeDamageImage != null)
        {
            _takeDamageImage.color = new Color(1f, 0f, 0f, 0.3f);
        }

        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0f;

        if(_takeDamageImage != null)
        {
            float fade = 0.3f;
            while (fade > 0f)
            {
                fade -= Time.deltaTime*2f;
                _takeDamageImage.color = new Color(1f, 0f, 0f, Mathf.Clamp01(fade));
                yield return null;
            }
            _takeDamageImage.color = new Color(1f, 0f, 0f, 0f);
        }
    }
}
