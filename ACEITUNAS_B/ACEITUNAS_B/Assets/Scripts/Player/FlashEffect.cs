using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
   
    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }


    public void FlashOnDamage(int flashes = 3)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(MultipleFlashRoutine(flashes));
    }

    private IEnumerator MultipleFlashRoutine(int flashes)
    {
        for (int i = 0; i < flashes; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }


}
