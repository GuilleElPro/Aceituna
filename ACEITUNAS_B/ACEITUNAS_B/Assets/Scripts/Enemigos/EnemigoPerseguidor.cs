using UnityEngine;
using System.Collections;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public float radioBusqueda;
    public LayerMask capaJugador;
    public float tiempoDeteccion = 2f;
    public float tiempoPersecucion = 5f;

    [Header("Movimiento")]
    public float velocidadMovimiento;
    public float velocidadRotacion = 5f;
    public float distanciaMaxima;
    public Vector3 puntoInicial;

    [Header("Ataque")]
    public float rangoAtaque = 1.5f;
    public float distanciaAnticipacion = 0.5f;
    public float tiempoAnticipacion = 0.5f;
    public float tiempoAtaque = 0.3f;
    public float fuerzaAtaque = 5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoDañoSacacorchos;
    [SerializeField] private AudioClip sonidoMovimientoSacacorchos;
    [SerializeField] private AudioClip sonidoMuerteSacacorchos;


    private AudioSource audioSource;

    [Header("Efectos Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorHerido1 = new Color(1f, 0.7f, 0.7f);
    
    [Header("Interacción con Jugador")]
    public float velocidadMinimaParaMatar = 7f;
    public float tiempoStop = 3f;
    public int danoAlJugador = 1;

    [SerializeField] private Animator animator;

    public GameObject particlePrefab;

    private Transform transformJugador;
    private EstadosMovimiento estadoActual;
    private bool jugadorEnRango;
    private float tiempoPersiguiendo;
    private Vector2 direccionMirar;
    private bool esStop = false;
    private Rigidbody2D rb;

    public enum EstadosMovimiento
    {
        Esperando,
        Detectando,
        Siguiendo,
        PreparandoAtaque,
        Anticipando,
        Atacando,
        Volviendo,
        Detenido
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        puntoInicial = transform.position;
        estadoActual = EstadosMovimiento.Esperando;
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Método principal de actualización por frame.
    /// Gestiona el comportamiento del enemigo según su estado actual.
    /// Ejecuta la lógica correspondiente (espera, detección, persecución, ataque, retorno, etc.)
    /// y controla el sonido de movimiento. Si el enemigo está detenido, detiene el sonido.
    /// </summary>
    private void Update()
    {
        if (estadoActual != EstadosMovimiento.Detenido)
        {
            switch (estadoActual)
            {
                case EstadosMovimiento.Esperando:
                    EstadoEsperando();
                    break;
                case EstadosMovimiento.Detectando:
                    EstadoDetectando();
                    break;
                case EstadosMovimiento.Siguiendo:
                    EstadoSiguiendo();
                    break;
                case EstadosMovimiento.PreparandoAtaque:
                    EstadoPreparandoAtaque();
                    break;
                case EstadosMovimiento.Anticipando:
                    EstadoAnticipando();
                    break;
                case EstadosMovimiento.Atacando:
                    EstadoAtacando();
                    break;
                case EstadosMovimiento.Volviendo:
                    EstadoVolviendo();
                    break;
            }

            ControlarSonidoMovimiento();
        }
        else
        {
            // Si está detenido, detener el sonido
            if (audioSource.isPlaying && audioSource.clip == sonidoMovimientoSacacorchos)
            {
                audioSource.Stop();
            }
        }
    }



    private void EstadoEsperando()
    {
        Collider2D jugadorCollider = Physics2D.OverlapCircle(transform.position, radioBusqueda, capaJugador);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsSiguiendo", false);

        if (jugadorCollider)
        {
            transformJugador = jugadorCollider.transform;
            estadoActual = EstadosMovimiento.Detectando;
            StartCoroutine(MirarJugador());
        }
    }

    private IEnumerator MirarJugador()
    {
        float tiempo = 0f;

        while (tiempo < tiempoDeteccion && estadoActual == EstadosMovimiento.Detectando)
        {
            if (transformJugador != null)
            {
                Vector2 direccion = (transformJugador.position - transform.position).normalized;
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg + 90f;
                Quaternion rotacion = Quaternion.AngleAxis(angulo, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, velocidadRotacion * Time.deltaTime);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (estadoActual == EstadosMovimiento.Detectando)
        {
            estadoActual = EstadosMovimiento.Siguiendo;
            tiempoPersiguiendo = 0f;
        }
    }

    private void EstadoDetectando()
    {
        // La lógica principal está en la corrutina MirarJugador
    }

    private void EstadoSiguiendo()
    {
        animator.SetBool("IsSiguiendo", true);
        animator.SetBool("IsAttacking", false);

        if (transformJugador == null)
        {
            estadoActual = EstadosMovimiento.Volviendo;
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, transformJugador.position);

        if (distanciaAlJugador <= rangoAtaque)
        {
            estadoActual = EstadosMovimiento.PreparandoAtaque;
            return;
        }

        Vector2 direccion = (transformJugador.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direccion, velocidadMovimiento * Time.deltaTime);

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);

        tiempoPersiguiendo += Time.deltaTime;
        if (tiempoPersiguiendo >= tiempoPersecucion ||
            Vector2.Distance(transform.position, puntoInicial) > distanciaMaxima)
        {
            estadoActual = EstadosMovimiento.Volviendo;
            transformJugador = null;
        }
    }

    private void EstadoPreparandoAtaque()
    {
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsSiguiendo", false);
        StartCoroutine(PrepararAtaque());
    }

    private IEnumerator PrepararAtaque()
    {
        yield return new WaitForSeconds(0.5f);

        if (estadoActual == EstadosMovimiento.PreparandoAtaque)
        {
            estadoActual = EstadosMovimiento.Anticipando;
            StartCoroutine(AnticiparAtaque());
        }
    }

    private IEnumerator AnticiparAtaque()
    {
        Vector2 posicionInicial = transform.position;
        Vector2 direccionRetroceso = -((Vector2)transformJugador.position - (Vector2)transform.position).normalized;
        Vector2 posicionFinal = posicionInicial + direccionRetroceso * distanciaAnticipacion;

        float tiempo = 0f;
        while (tiempo < tiempoAnticipacion && estadoActual == EstadosMovimiento.Anticipando)
        {
            transform.position = Vector2.Lerp(posicionInicial, posicionFinal, tiempo / tiempoAnticipacion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        if (estadoActual == EstadosMovimiento.Anticipando)
        {
            estadoActual = EstadosMovimiento.Atacando;
            StartCoroutine(EjecutarAtaque());
        }
    }

    private void EstadoAnticipando()
    {
        // La lógica principal está en la corrutina AnticiparAtaque
    }

    private IEnumerator EjecutarAtaque()
    {
        audioSource.PlayOneShot(sonidoDañoSacacorchos);
        Vector2 posicionInicial = transform.position;
        Vector2 direccionAtaque = -transform.up; // Apunta con la punta que está abajo
        Vector2 posicionFinal = posicionInicial + direccionAtaque * 1.5f;

        float tiempo = 0f;
        while (tiempo < tiempoAtaque && estadoActual == EstadosMovimiento.Atacando)
        {
            transform.position = Vector2.Lerp(posicionInicial, posicionFinal, tiempo / tiempoAtaque);
            tiempo += Time.deltaTime;
            yield return null;
        }

        estadoActual = EstadosMovimiento.Siguiendo;
        tiempoPersiguiendo = 0f;
    }

    private void EstadoAtacando()
    {
        // La lógica principal está en la corrutina EjecutarAtaque
    }

    private void EstadoVolviendo()
    {
        transform.position = Vector2.MoveTowards(transform.position, puntoInicial, velocidadMovimiento * Time.deltaTime);

        Vector2 direccionMirar = (puntoInicial - transform.position).normalized;
        float angulo = Mathf.Atan2(direccionMirar.y, direccionMirar.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);

        if (Vector2.Distance(transform.position, puntoInicial) < 0.1f)
        {
            estadoActual = EstadosMovimiento.Esperando;
            transform.rotation = Quaternion.identity;
        }
    }

    private void ControlarSonidoMovimiento()
    {
        // Estados en los que el enemigo se mueve
        bool estaMoviendose = estadoActual == EstadosMovimiento.Siguiendo ||
                         estadoActual == EstadosMovimiento.Volviendo ||
                         estadoActual == EstadosMovimiento.Anticipando ||
                         estadoActual == EstadosMovimiento.Atacando;

        if (estaMoviendose)
        {
            // Si no está reproduciendo el sonido de movimiento y tiene clip asignado
            if (!audioSource.isPlaying && sonidoMovimientoSacacorchos != null)
            {
                audioSource.clip = sonidoMovimientoSacacorchos;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Si no se está moviendo, detener el sonido
            if (audioSource.isPlaying && audioSource.clip == sonidoMovimientoSacacorchos)
            {
                audioSource.Stop();
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = other.gameObject.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                float velocidadJugador = rbJugador.linearVelocity.magnitude;

                if (velocidadJugador >= velocidadMinimaParaMatar)
                {
                    MatarEnemigo();
                }
                else
                {
                    GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
                    rbJugador.AddTorque(60f);

                    if (!esStop)
                    {
                        StartCoroutine(Stop());
                    }
                }
            }
        }
    }

    public float vidasSaca = 2f;

    private void MatarEnemigo()
    {
        vidasSaca--;
        if (vidasSaca == 1)
        {
            spriteRenderer.color = colorHerido1;
        }

        if (vidasSaca == 0)
        {
            // Reproducir sonido de muerte antes de destruir el objeto
            if (sonidoMuerteSacacorchos != null)
            {
                AudioSource.PlayClipAtPoint(sonidoMuerteSacacorchos, transform.position);
            }

            Debug.Log("Enemigo eliminado por el jugador");

            // Instanciar partículas de muerte
            GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();

            Destroy(gameObject);
        }
    }

    private IEnumerator Stop()
    {
        esStop = true;
        EstadosMovimiento estadoPrevio = estadoActual;
        estadoActual = EstadosMovimiento.Detenido;

        // Detener sonido de movimiento
        if (audioSource.isPlaying && audioSource.clip == sonidoMovimientoSacacorchos)
        {
            audioSource.Stop();
        }

        Debug.Log("Stop Activado");

        yield return new WaitForSeconds(tiempoStop);

        esStop = false;
        Debug.Log("Stop Desactivado");

        Collider2D jugadorCollider = Physics2D.OverlapCircle(transform.position, radioBusqueda, capaJugador);
        if (jugadorCollider != null)
        {
            transformJugador = jugadorCollider.transform;
            estadoActual = EstadosMovimiento.Siguiendo;
            tiempoPersiguiendo = 0f;
        }
        else
        {
            estadoActual = EstadosMovimiento.Volviendo;
            transformJugador = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioBusqueda);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        Gizmos.color = Color.blue;
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(puntoInicial, distanciaMaxima);
    }
}
