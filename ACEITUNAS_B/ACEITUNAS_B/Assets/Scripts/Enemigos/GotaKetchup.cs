using UnityEngine;
using UnityEngine.Audio;

public class GotaKetchup : MonoBehaviour
{
    [SerializeField] private float velocidad = 4f;
    [SerializeField] private float tiempoVida = 3f;
    [SerializeField] private int dano = 1;

    //[SerializeField] private AudioClip sonidoExpulsaGotas;
    //private AudioSource audioSource;

    private Rigidbody2D rb;
    private Vector2 direccion;
    private float fuerza;
    private GameObject enemigoPadre;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    public void Inicializar(Vector2 dir, float force = 1f, GameObject padre = null)
    {
        direccion = dir;
        fuerza = force;
        enemigoPadre = padre;
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, tiempoVida);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direccion * velocidad * fuerza;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisión con el enemigo padre y otros enemigos
        if (enemigoPadre != null && (other.gameObject == enemigoPadre || other.CompareTag("Enemigo")))
            return;

        if (other.CompareTag("Player"))
        {
            //audioSource.PlayOneShot(sonidoExpulsaGotas);
            var gameManager = GameObject.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                
                GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
                other.gameObject.GetComponent<Ball>().Realentizacion();
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstaculo"))
        {
            Destroy(gameObject);
        }
    }
}
