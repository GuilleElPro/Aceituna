using UnityEngine;

public class ColisionesTorqueEnemigos : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGolpeEnemigos;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sonidoGolpeEnemigos);
           
        }


    }

}
