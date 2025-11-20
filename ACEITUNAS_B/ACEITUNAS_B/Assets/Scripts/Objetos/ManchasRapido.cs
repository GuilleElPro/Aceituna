using UnityEngine;

public class ManchasRapido : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMovimientoLiquidoAgua;
    private AudioSource audioSource;

    public float factorRalentizacion;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = sonidoMovimientoLiquidoAgua;
                audioSource.Play();
            }

            //GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
            other.gameObject.GetComponent<Ball>().Acelerar();
            Debug.Log("Va Rapido. . .");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

    }
}
