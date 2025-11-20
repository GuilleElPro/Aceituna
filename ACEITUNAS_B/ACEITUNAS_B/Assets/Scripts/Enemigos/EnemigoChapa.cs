using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemigoChapa : MonoBehaviour
{
    [Header("Configuraci�n de Persecuci�n")]
    [SerializeField] private float radioDeteccion = 5f;
    [SerializeField] private float velocidadPersecucion = 3f;
    [SerializeField] private float velocidadMinimaParaMatar = 7f;
    [SerializeField] private int danoAlJugador = 1;
    [SerializeField] private float tiempoDescanso = 1f;
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private float intervaloParadas = 3f;
    [SerializeField] private float duracionParada = 1f;

    [SerializeField] private AudioClip sonidoMovimientoChapa;
    [SerializeField] private AudioClip sonidoMuerteChapa;

    private AudioSource audioSource;

    [Header("Efectos Visuales")]
    [SerializeField] private Color colorPersecucion = Color.red;
    [SerializeField] private Color colorParado = Color.yellow;
    [SerializeField] private Color colorDescanso = Color.gray;

    //public ParticleSystem collisionParticles; // Asigna el ParticleSystem desde el Inspector
    public GameObject particlePrefab; // Asigna el prefab desde el Inspector.


    private Transform jugador;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;
    private bool estaEnDescanso = false;
    private bool enParada = false;
    private float tiempoSiguienteParada;

    [SerializeField] private Animator animator;

    //Pablo
   // public LayerMask layerObjetos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorOriginal = spriteRenderer.color;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        tiempoSiguienteParada = Time.time + intervaloParadas;
    }

    private void Update()
    {
        if (estaEnDescanso) return;

        if (enParada)
        {
            if (!JugadorEnRango())
            {
                TerminarParada();
                DetenerPersecucion();
            }
            return;
        }

        if (JugadorEnRango())
        {
            if (Time.time >= tiempoSiguienteParada)
            {
                IniciarParada();
                return;
            }
            
            PerseguirJugador();
            
        }
        else
        {
            DetenerPersecucion();
        }
    }

    private void IniciarParada()
    {
        enParada = true;
        rb.linearVelocity = Vector2.zero;
       spriteRenderer.color = colorParado;
        Invoke("TerminarParada", duracionParada);
    }

    private void TerminarParada()
    {
        enParada = false;
        tiempoSiguienteParada = Time.time + intervaloParadas;
       spriteRenderer.color = JugadorEnRango() ? colorPersecucion : colorOriginal;
    }

    private bool JugadorEnRango()
    {
        
        Collider2D jugadorCollider = Physics2D.OverlapCircle(transform.position, radioDeteccion, capaJugador);
        if (jugadorCollider != null)
        {
            animator.SetBool("Anticipando", true);
            jugador = jugadorCollider.transform;
            return true;
        }
        animator.SetBool("IsAttacking", false);
        return false;
        
    }

    private void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        
        rb.linearVelocity = direccion * velocidadPersecucion;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        
        spriteRenderer.color = colorPersecucion;

        if (!audioSource.isPlaying && sonidoMovimientoChapa != null)
        {
            audioSource.clip = sonidoMovimientoChapa;
            audioSource.Play();
        }

        //audioSource.PlayOneShot(sonidoMovimientoChapa);

        animator.SetBool("Anticipando", false);
        animator.SetBool("IsAttacking", true);
    }

    private void DetenerPersecucion()
    {
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.color = colorOriginal;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            GameManager manager = collision.gameObject.GetComponent<GameManager>();

            // El jugador puede matar al enemigo cuando está parado (en pausa o descanso)
            if ((enParada || estaEnDescanso) &&
                rbJugador != null &&
                rbJugador.linearVelocity.magnitude >= velocidadMinimaParaMatar)
            {
                // Reproducir sonido de muerte antes de destruir el objeto
                if (sonidoMuerteChapa != null)
                {
                    // Usamos PlayClipAtPoint para que el sonido se reproduzca incluso si el objeto es destruido
                    AudioSource.PlayClipAtPoint(sonidoMuerteChapa, transform.position);
                }

                // Instancia las partículas en el punto de colisión
                GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                particles.GetComponent<ParticleSystem>().Play();

                Destroy(gameObject);
            }

            if (collision.gameObject.CompareTag("Player") && !enParada)
            {
                GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
            }

            // Entrar en modo descanso (si no estaba ya)
            if (!estaEnDescanso)
            {
                StartCoroutine(ModoDescanso());
            }
        }
    }

    private IEnumerator ModoDescanso()
    {
        estaEnDescanso = true;
        DetenerPersecucion();
        spriteRenderer.color = colorDescanso;


        yield return new WaitForSeconds(tiempoDescanso);

        estaEnDescanso = false;
        tiempoSiguienteParada = Time.time + intervaloParadas;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}