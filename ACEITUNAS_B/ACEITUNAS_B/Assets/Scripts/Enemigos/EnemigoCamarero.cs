using UnityEngine;

public class EnemigoCamarero : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private GameObject objetomover;
    [SerializeField] private Transform punto1, punto2;
    [SerializeField] private float velocidad;
    [SerializeField] private Vector3 moverHacia;
    

    [SerializeField] private Animator animator;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoPasosCamarero;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        moverHacia = punto2.position;
    }

    private void Update()
    {
        MovimientoEnemigo();

    }

    public void MovimientoEnemigo()
    {
        objetomover.transform.position = Vector3.MoveTowards(objetomover.transform.position, moverHacia, velocidad * Time.deltaTime);
        

        if (objetomover.transform.position == punto2.position)
        {
            audioSource.PlayOneShot(sonidoPasosCamarero);
            moverHacia = punto1.position;
            objetomover.transform.Rotate(0f, 0f, 180f);
            //animator.SetBool("RightSal", false);
            //animator.SetBool("LeftSal", true);

        }

        if (objetomover.transform.position == punto1.position)
        {
            audioSource.PlayOneShot(sonidoPasosCamarero);
            moverHacia = punto2.position;
            objetomover.transform.Rotate(0f, 0f, 180f);
            //animator.SetBool("LeftSal", false);
            //animator.SetBool("RightSal", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.FindFirstObjectByType<GameManager>().MatarJugador();
        }
    }
}
