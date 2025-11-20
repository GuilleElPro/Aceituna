using UnityEngine;

public class Enemigo : MonoBehaviour
{
    //Script enemigo

    [Header("Variables")]
    [SerializeField] private GameObject objetomover;
    [SerializeField] private Transform punto1, punto2;
    [SerializeField] private float velocidad;
    [SerializeField] private Vector3 moverHacia;

    [SerializeField] private Animator animator;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMovimientoSalero;
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
            audioSource.PlayOneShot(sonidoMovimientoSalero);
            moverHacia = punto1.position;
            animator.SetBool("RightSal", false);
            animator.SetBool("LeftSal", true);

        }

        if (objetomover.transform.position == punto1.position)
        {
            audioSource.PlayOneShot(sonidoMovimientoSalero);
            moverHacia = punto2.position;
            animator.SetBool("LeftSal", false);
            animator.SetBool("RightSal", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            GameObject.FindFirstObjectByType<GameManager>().RecibirDano();

            //Fuerza con la que sale disparada la oliva al colisionar
            other.rigidbody.AddTorque(60f);

            //Debug.Log("Pierde Vida");

        }
    }
}
