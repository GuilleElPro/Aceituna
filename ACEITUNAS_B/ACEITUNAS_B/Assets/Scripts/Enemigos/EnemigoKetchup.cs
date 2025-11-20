using UnityEngine;
using System.Collections;

public class EnemigoKetchup : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidasMaximas = 2;
    [SerializeField] private float tiempoInvulnerabilidad = 1f;

    [Header("Efectos Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorHerido = new Color(1f, 0.7f, 0.7f);
    [SerializeField] private Color colorMuyHerido = new Color(1f, 0.4f, 0.4f);

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMovimientoKetchup;
    [SerializeField] private AudioClip sonidoExpulsaGotas;
    [SerializeField] private AudioClip sonidoMuerteKetchup;
    private AudioSource audioSource;

    [Header("Configuración de Movimiento")]
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float tiempoEntreGotas = 1.5f;

    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject gotaPequeñaPrefab;
    [SerializeField] private GameObject gotaGrandePrefab;
    [SerializeField] private float fuerzaGotaGrande = 8f;
    [SerializeField] private float rangoGotaGrande = 3f;

    [Header("Configuración de Golpe por Velocidad")]
    [SerializeField] private float velocidadMinimaParaDaño = 7f;
    [SerializeField] private float fuerzaEmpujeExtra = 15f;

    [Header("Ataque Especial al Recibir Daño")]
    [SerializeField] private float tiempoDetenidoAlRecibirDaño = 1.5f;
    [SerializeField] private float tiempoAntesDeDispararGotaGrande = 0.5f;

    [Header("Ataque Final al Morir")]
    [SerializeField] private float tiempoEsperaAtaqueFinal = 0.3f;
    [SerializeField] private int cantidadGotasFinales = 4;

    [SerializeField] private Animator animator;

    public GameObject particlePrefab;

    private int vidasActuales;
    private bool puedeRecibirDaño = true;
    private int indicePuntoActual = 0;
    private float tiempoUltimaGota;
    private Color colorOriginal;
    private Transform jugador;
    private bool estaRealizandoAtaqueEspecial = false;
    private float velocidadOriginal;
    private bool estaMuriendo = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vidasActuales = vidasMaximas;
        spriteRenderer = GetComponent<SpriteRenderer>();

        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        colorOriginal = spriteRenderer.color;
        tiempoUltimaGota = Time.time;
        velocidadOriginal = velocidad;

        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            Debug.LogError("No hay puntos de patrulla asignados!");
            enabled = false;
        }

        gameObject.layer = LayerMask.NameToLayer("Enemigos");
    }

    private void Update()
    {
        if (!estaRealizandoAtaqueEspecial && !estaMuriendo)
        {
            Movimiento();
            DispararGotasPequeñas();
        }
    }

    private void Movimiento()
    {
        animator.SetBool("IsAttacking", false);
        if (puntosPatrulla.Length == 0 || estaRealizandoAtaqueEspecial || estaMuriendo) return;

        transform.position = Vector3.MoveTowards(transform.position,
            puntosPatrulla[indicePuntoActual].position,
            velocidad * Time.deltaTime);
           

        if (Vector3.Distance(transform.position, puntosPatrulla[indicePuntoActual].position) < 0.1f)
        {
            audioSource.PlayOneShot(sonidoMovimientoKetchup);
            indicePuntoActual = (indicePuntoActual + 1) % puntosPatrulla.Length;
        }
    }

    private void DispararGotasPequeñas()
    {
        //audioSource.PlayOneShot(sonidoExpulsaGotas);

        if (Time.time - tiempoUltimaGota >= tiempoEntreGotas && !estaRealizandoAtaqueEspecial && !estaMuriendo)
        {
            tiempoUltimaGota = Time.time;

            for (int i = 0; i < 8; i++)
            {
                float angulo = i * 45f;
                Vector2 direccion = new Vector2(
                    Mathf.Cos(angulo * Mathf.Deg2Rad),
                    Mathf.Sin(angulo * Mathf.Deg2Rad)
                );

                audioSource.PlayOneShot(sonidoExpulsaGotas);
                GameObject gota = Instantiate(gotaPequeñaPrefab, transform.position, Quaternion.identity);
                GotaKetchup gotaScript = gota.GetComponent<GotaKetchup>();
                gotaScript.Inicializar(direccion, 1f, gameObject);

                gota.layer = LayerMask.NameToLayer("GotasEnemigo");
            }
        }
    }

    public void RecibirGolpe(int cantidadDaño = 1)
    {
        if (!puedeRecibirDaño || estaRealizandoAtaqueEspecial || estaMuriendo) return;

        vidasActuales -= cantidadDaño;
        Debug.Log($"Vidas restantes: {vidasActuales}");

        if (vidasActuales <= 0)
        {
            StartCoroutine(Morir());
        }
        else
        {
            StartCoroutine(AtaqueEspecialPorDaño());
            StartCoroutine(ModoInvulnerable());
        }
    }

    private IEnumerator AtaqueEspecialPorDaño()
    {
        estaRealizandoAtaqueEspecial = true;
        velocidad = 0f;
        spriteRenderer.color = colorMuyHerido;

        yield return new WaitForSeconds(tiempoAntesDeDispararGotaGrande);

        if (jugador != null)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;
            DispararGotaGrande(direccion);
        }

        yield return new WaitForSeconds(tiempoDetenidoAlRecibirDaño - tiempoAntesDeDispararGotaGrande);

        spriteRenderer.color = colorOriginal;
        velocidad = velocidadOriginal;
        estaRealizandoAtaqueEspecial = false;
    }

    private IEnumerator Morir()
    {
        estaMuriendo = true;
        puedeRecibirDaño = false;
        velocidad = 0f;

        

        // Cambiar color a rojo intenso
        spriteRenderer.color = Color.white;

        // Esperar un momento antes del ataque final
        yield return new WaitForSeconds(tiempoEsperaAtaqueFinal);

        // Ataque final en múltiples direcciones
        animator.SetBool("IsAttacking", true);
        if (jugador != null)
        {
            Vector2 direccionBase = (jugador.position - transform.position).normalized;

            for (int i = 0; i < cantidadGotasFinales; i++)
            {
                float angulo = i * (360f / cantidadGotasFinales);
                Vector2 direccion = Quaternion.Euler(0, 0, angulo) * direccionBase;
                DispararGotaGrande(direccion);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Desactivar componentes y destruir
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.enabled = false;

        GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        particles.GetComponent<ParticleSystem>().Play();

        // Esperar un momento para que las gotas salgan antes de destruirse
        yield return new WaitForSeconds(0.5f);
        // Reproducir sonido de muerte
        if (sonidoMuerteKetchup != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerteKetchup, transform.position);
        }
        Destroy(gameObject);
    }

    private void DispararGotaGrande(Vector2 direccion)
    {
        animator.SetBool("IsAttacking", true);
        GameObject gota = Instantiate(gotaGrandePrefab, transform.position, Quaternion.identity);
        GotaKetchup gotaScript = gota.GetComponent<GotaKetchup>();
        gotaScript.Inicializar(direccion, fuerzaGotaGrande * 1.5f, gameObject); // Aumentar fuerza en ataque final
        gota.layer = LayerMask.NameToLayer("GotasEnemigo");
    }

    private IEnumerator ModoInvulnerable()
    {
        puedeRecibirDaño = false;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoInvulnerabilidad)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            tiempoTranscurrido += 0.1f;
        }

        spriteRenderer.enabled = true;
        puedeRecibirDaño = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && puedeRecibirDaño && !estaMuriendo)
        {
            var playerRb = other.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null && playerRb.linearVelocity.magnitude >= velocidadMinimaParaDaño)
            {
                Vector2 direccionEmpuje = (playerRb.transform.position - transform.position).normalized;
                playerRb.AddForce(direccionEmpuje * fuerzaEmpujeExtra, ForceMode2D.Impulse);
                RecibirGolpe(1);
            }
            else
            {
                var gameManager = other.gameObject.GetComponent<GameManager>();
                if (gameManager != null)
                {
                    gameManager.RecibirDano();
                }
            }
        }
    }
}