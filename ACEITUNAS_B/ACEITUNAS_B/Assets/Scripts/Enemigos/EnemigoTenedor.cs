using UnityEngine;
using System.Collections;

public class EnemigoTenedor : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Anticipación")]
    [SerializeField] private float anticipationDistance = 0.5f;
    [SerializeField] private float anticipationDuration = 0.5f;
    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private AnimationCurve anticipationCurve;
    [SerializeField] private Color anticipationColor = Color.red;
    [SerializeField] private float flashIntensity = 0.7f;
    [SerializeField] private float flashSpeed = 10f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoAtaqueTenedor;
    private AudioSource audioSource;


    [Header("Área de Detección")]
    [SerializeField] private Vector2 detectionSize = new Vector2(5f, 3f);
    [SerializeField] private Vector2 detectionOffset = Vector2.zero;
    [SerializeField] private float detectionAngle = 0f;
    [SerializeField] private Color zoneColor = new Color(1, 0, 0, 0.3f);

    [Header("Punto Final de Carga")]
    [SerializeField] private Transform puntoFinalCarga;

    [Header("Referencias")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SpriteRenderer enemySprite;

    [SerializeField] private Animator animator;
    private Transform player;
    private Rigidbody2D rb;
    private Vector3 originalPosition;
    private Color originalColor;
    private bool isCharging = false;
    private bool isReturning = false;
    private Vector3 chargeDirection;
    private Coroutine anticipationCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        originalColor = enemySprite.color;

        if (puntoFinalCarga == null)
        {
            Debug.LogError("Asigna un punto final de carga en el inspector");
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (!isCharging && !isReturning && anticipationCoroutine == null && IsPlayerInDetectionZone())
        {
            anticipationCoroutine = StartCoroutine(AnticipationRoutine());
        }

        HandleMovement();
    }

    private IEnumerator AnticipationRoutine()
    {
        // Preparación
        Vector3 startPosition = transform.position;
        chargeDirection = (puntoFinalCarga.position - transform.position).normalized;
        float timer = 0f;

        // Efecto visual: Cambio de color
        enemySprite.color = Color.Lerp(originalColor, anticipationColor, flashIntensity);

        // Fase 1: Retroceso
        while (timer < anticipationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / anticipationDuration;
            float curveValue = anticipationCurve.Evaluate(progress);

            Vector3 backwardPosition = startPosition - (Vector3)(chargeDirection * anticipationDistance);
            transform.position = Vector3.Lerp(startPosition, backwardPosition, curveValue);

            // Efecto de parpadeo
            float flash = Mathf.PingPong(Time.time * flashSpeed, 1f);
            enemySprite.color = Color.Lerp(originalColor, anticipationColor, flash * flashIntensity);

            yield return null;
        }

        // Fase 2: Pausa antes de atacar
        yield return new WaitForSeconds(attackDelay);

        // Restaurar color
        enemySprite.color = originalColor;

        // Comenzar ataque
        StartCharge();
        anticipationCoroutine = null;
    }

    private void HandleMovement()
    {
        if (isCharging)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, puntoFinalCarga.position, chargeSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, puntoFinalCarga.position) < 0.1f)
            {
                ReturnToStart();
            }
        }

        if (isReturning)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
            {
                isReturning = false;
            }
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        audioSource.PlayOneShot(sonidoAtaqueTenedor);
        isReturning = false;
        animator.SetBool("IsAttacking", true);

    }

    private void ReturnToStart()
    {
        isCharging = false;
        isReturning = true;
        animator.SetBool("IsAttacking", false);
    }

    private bool IsPlayerInDetectionZone()
    {
        if (player == null) return false;

        Collider2D hit = Physics2D.OverlapBox(
        (Vector2)transform.position + detectionOffset,
        detectionSize,
        detectionAngle,
        playerLayer
        );

        return hit != null && hit.CompareTag("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            GameObject.FindFirstObjectByType<GameManager>().RecibirDano();

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            ReturnToStart();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS((Vector2)transform.position + detectionOffset, Quaternion.Euler(0, 0, detectionAngle), Vector3.one);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = zoneColor;
        Gizmos.DrawCube(Vector3.zero, detectionSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, detectionSize);

        // Reset matrix
        Gizmos.matrix = Matrix4x4.identity;

        if (puntoFinalCarga != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, puntoFinalCarga.position);
            Gizmos.DrawSphere(puntoFinalCarga.position, 0.2f);
        }
    }
}