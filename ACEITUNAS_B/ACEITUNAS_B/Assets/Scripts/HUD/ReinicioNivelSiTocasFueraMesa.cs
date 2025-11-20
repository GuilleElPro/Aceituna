using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReinicioNivelSiTocasFueraMesa : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoCaidaOliva;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Al entrar el player muere
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sonidoCaidaOliva);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.FindFirstObjectByType<GameManager>().MatarJugador();

        }
        

        //Al entrar un enemigo se destruye
        if (other.gameObject.CompareTag("Enemigo"))
        {
            audioSource.PlayOneShot(sonidoCaidaOliva);
            Destroy(other.gameObject);
           //GameObject.FindFirstObjectByType<GameManager>().EnemigoCaida();
            

        }
        
    }
}
