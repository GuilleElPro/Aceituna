using UnityEngine;
using System.Collections;

public class VidaUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 3f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
    }

    public void PerderVida()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Mueve aleatoriamente
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        rectTransform.localPosition = originalPosition; // Vuelve a la posición original
    }
}
