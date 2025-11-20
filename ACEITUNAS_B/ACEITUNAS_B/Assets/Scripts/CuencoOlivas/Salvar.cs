using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Salvar : MonoBehaviour
{
    [Header("Configuración del Cuenco")]
    public int vidasIniciales = 3; // Número de vidas iniciales
    public SpriteRenderer cuboColor; // Referencia al SpriteRenderer para cambiar el color
    public float velocidadMinimaParaRomper = 7f; // Velocidad mínima para romper el cuenco
    public float torqueForceCuenco = 10f; //Torque del cuenco

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGolpeCuenco;
    [SerializeField] private AudioClip sonidoGolpeCuencoDestruir;
    [SerializeField] private AudioClip sonidoHasGanado;


    private AudioSource audioSource;

    [SerializeField] private Animator animator;

    private bool playerInRange = false;
    private Coroutine attackCoroutine;
    [SerializeField] private float attackDelay = 1.5f;

    private int vidasActuales; // Vidas actuales del cuenco

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vidasActuales = vidasIniciales; // Inicializar las vidas
        ActualizarColor(); // Actualizar el color inicial
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Obtener la velocidad del jugador
            Rigidbody2D rbJugador = other.gameObject.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                float velocidadJugador = rbJugador.linearVelocity.magnitude;

                // Verificar si la velocidad es suficiente para romper el cuenco
                if (velocidadJugador >= velocidadMinimaParaRomper)
                {
                    //animator.SetBool("IsBol", true);
                    //animator.SetBool("IsAceituna", true);
                    attackCoroutine = StartCoroutine(PrepareAttack(other.gameObject));

                    // Aplicar torque al jugador
                    //rbJugador.AddTorque(25f);
                    //rbJugador.AddTorque(torqueForceCuenco);

                    // Reducir una vida
                    vidasActuales--;
                    Debug.Log($"Impacto al Cuenco de olivas. Vidas restantes: {vidasActuales}");

                    // Actualizar el color y verificar si el cuenco debe ser destruido
                    ActualizarColor();

                    if (vidasActuales <= 0)
                    {
                        DestruirCuenco();
                    }
                }
                else
                {
                    Debug.Log("Velocidad insuficiente para romper el cuenco.");
                }
            }
        }
    }

    private IEnumerator PrepareAttack(GameObject player)
    {
        yield return new WaitForSeconds(attackDelay);
        animator.SetBool("IsBol", false);
        animator.SetBool("IsBol2", false);
        //animator.SetBool("IsAceituna",false);

    }

    // Método para actualizar el color del cuenco //TEMPORAL, HACER POR ANIMACION GRIETAS
    
    private void ActualizarColor()
    {
        switch (vidasActuales)
        {
            case 2:
                animator.SetBool("IsBol", true);
                audioSource.PlayOneShot(sonidoGolpeCuenco);
                //cuboColor.color = Color.green;
                Debug.Log("2 vidas restantes...");
                break;
            case 1:
                animator.SetBool("IsBol", false);
                animator.SetBool("IsBol2", true);
                audioSource.PlayOneShot(sonidoGolpeCuenco);
                //cuboColor.color = Color.red;
                Debug.Log("1 vida restante...");
                break;
            default:
                break;
        }
    }
    

    // Método para destruir el cuenco y cargar el siguiente nivel
    private void DestruirCuenco()
    {
        audioSource.PlayOneShot(sonidoGolpeCuencoDestruir);
        Debug.Log("Cuenco Destruido - 0 vidas restantes");
        cuboColor.enabled = false;
        //Destroy(gameObject); // Destruir el cuenco


        GameObject.FindFirstObjectByType<MenuHasGanado>().ActivarMenuHasGanado();
        audioSource.PlayOneShot(sonidoHasGanado);

        //CODIGO MOVIDO A  SCRIPT MenuHasGanado -->

        /*
        // Obtener el índice de la escena actual
        int nivelActual = SceneManager.GetActiveScene().buildIndex;

        // Cargar el siguiente nivel
        int siguienteNivel = nivelActual + 1;

        if (siguienteNivel < SceneManager.sceneCountInBuildSettings) // Verificar si existe el siguiente nivel
        {
            SceneManager.LoadScene(siguienteNivel); // Cargar el siguiente nivel
            PlayerPrefs.SetInt("NivelActual", siguienteNivel); // Guardar el siguiente nivel
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
            SceneManager.LoadScene("MenuNiveles"); // Volver al menú de niveles si no hay más niveles
        }

        */
    }
}