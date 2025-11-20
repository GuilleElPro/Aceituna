using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SaltoDeMesa : MonoBehaviour
{
    [Header("Configuración del Impulso")]
    [SerializeField] private Transform puntoDestino;

    [SerializeField] private float fuerza = 15f;

    [SerializeField] private ForceMode2D modoFuerza = ForceMode2D.Impulse;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoChampagne;

    private AudioSource audioSource;

    [Header("Opciones")]
    [SerializeField] private bool normalizarDireccion = true;

    [SerializeField] private bool considerarMasa = true;

    [SerializeField] private Animator animator;

    [Header("Configuración")]
    [SerializeField] private float radioBusqueda = 2f;
    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private float launchDelay = 2f;


    
    public bool isImpulse = false;
    public GameObject colliderMesa;
    public SpriteRenderer playerSprite;
    //public TrailRenderer playertrailRenderer;
    //public GameObject player;

    private bool playerInRange = false;
    private Coroutine attackCoroutine;

    private Coroutine launchCoroutine;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //inside = true;
            //player.SetActive(false);
            
            playerSprite.enabled = false;
            
            //playertrailRenderer.enabled = false;
            animator.SetBool("IsChampagne", true);

            playerInRange = true;
            attackCoroutine = StartCoroutine(PrepareAttack(other.gameObject));
            



        }
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //player.SetActive(true);
            playerSprite.enabled = true;
           // playertrailRenderer.enabled = true;
            animator.SetBool("IsChampagne", false);
            playerInRange = false;
            if (attackCoroutine != null)
            {
                //StopCoroutine(launchCoroutine);
                StopCoroutine(attackCoroutine);
            }
        }
    }
    
    
    
    private IEnumerator PrepareLaunch()
    {
        if (isImpulse == true)
        {
            colliderMesa.SetActive(false);
            yield return new WaitForSeconds(launchDelay);
            isImpulse = false;

        }

        if (isImpulse == false)
        {
            colliderMesa.SetActive(true);
        }
    }
    

    private IEnumerator PrepareAttack(GameObject player)
    {
        Debug.Log("Prepare Attack");
        yield return new WaitForSeconds(attackDelay);
        player.SetActive(true);
        if (playerInRange && player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direccion = puntoDestino.position - player.transform.position;

                if (normalizarDireccion)
                {
                    direccion = direccion.normalized;
                }

                float fuerzaFinal = considerarMasa ? fuerza * rb.mass : fuerza;

                audioSource.PlayOneShot(sonidoChampagne);
                rb.AddForce(direccion * fuerzaFinal, modoFuerza);
                
                isImpulse = true;
                launchCoroutine = StartCoroutine(PrepareLaunch());
            }

        }
        
    }
    private void OnDrawGizmos()
    {
        if (puntoDestino != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, puntoDestino.position);
            Gizmos.DrawWireSphere(puntoDestino.position, 0.3f);
        }
    }
}
