using UnityEngine;
using System.Collections;

public class EnemigoComensal : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float radioBusqueda = 5f;
    [SerializeField] private float attackDelay = 2f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGolpeComensal;
    private AudioSource audioSource;


    private bool playerInRange = false;
    private Coroutine attackCoroutine;
    private CircleCollider2D detectionCollider;

    [SerializeField] private Animator animator;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ConfigureCollider();
    }

    private void ConfigureCollider()
    {
        detectionCollider = GetComponent<CircleCollider2D>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
            Debug.LogWarning("CircleCollider2D añadido automáticamente al comensal.");
        }

        detectionCollider.radius = radioBusqueda;
        detectionCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsAttacking", true);
            playerInRange = true;
            attackCoroutine = StartCoroutine(PrepareAttack(other.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsAttacking", false);
            playerInRange = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
    }

    private IEnumerator PrepareAttack(GameObject player)
    {
        yield return new WaitForSeconds(attackDelay);

        if (playerInRange && player != null)
        {
            // Reproducir sonido de golpe mortal
            if (sonidoGolpeComensal != null)
            {
                audioSource.PlayOneShot(sonidoGolpeComensal);
            }

            // Matar al jugador
            GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                // Matar al jugador (quitar todas las vidas)
                while (gameManager.vidas > 0)
                {
                    gameManager.MatarJugador();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                Debug.LogError("GameManager no encontrado en el Player.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioBusqueda);
    }
}
