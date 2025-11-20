using UnityEngine;

public class ManchasLento : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMovimientoLiquido;
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
                audioSource.clip = sonidoMovimientoLiquido;
                audioSource.Play();
            }

            //GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
            other.gameObject.GetComponent<Ball>().Realentizacion();

            Debug.Log("Va lento. . .");

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
