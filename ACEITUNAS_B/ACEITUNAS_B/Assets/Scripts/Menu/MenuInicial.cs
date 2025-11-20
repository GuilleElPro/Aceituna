using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    //public Animator animator;

    [Header("Configuración de Sonido")]
    [SerializeField] private AudioClip sonidoBoton;
    [SerializeField][Range(0f, 1f)] private float volumenSonido = 0.5f;
    [SerializeField] private float delayCambioEscena = 0.3f;
    private AudioSource audioSource;


    private void Awake()
    {
        // Configurar el AudioSource
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.volume = volumenSonido;
    }

    private void Start()
    {
        /*
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        */

        // Configurar AudioSource
       // audioSource.playOnAwake = false;

        //animator = GetComponent<Animator>();
        // Cargar el nivel actual guardado (o el nivel 1 si no hay datos guardados)
        int nivelActual = PlayerPrefs.GetInt("NivelActual", 1);
        Debug.Log($"Nivel actual guardado: {nivelActual}");
    }

    private IEnumerator CargarEscenaConDelay(string nombreEscena)
    {
        // Reproducir el sonido primero
        ReproducirSonidoBoton();

        // Esperar el tiempo especificado
        yield return new WaitForSecondsRealtime(delayCambioEscena);

        // Cargar la escena
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"La escena '{nombreEscena}' no existe");
        }
    }

    private void ReproducirSonidoBoton()
    {
        if (sonidoBoton != null)
        {
            audioSource.PlayOneShot(sonidoBoton);
        }
        else
        {
            Debug.LogWarning("No hay clip de audio asignado para el sonido del botón");
        }
    }

    // Método para cargar una escena por nombre
    /*
    private void CargarEscena(string nombreEscena)
    {
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"La escena '{nombreEscena}' no existe");
        }
    }
    */

    // Métodos para los botones del menú
    public void Jugar()
    {
        ReproducirSonidoBoton();
        // Cargar el nivel actual guardado
        //int nivelActual = PlayerPrefs.GetInt("NivelActual", 1);
        //CargarEscena($"Level{nivelActual}");
        Time.timeScale = 1;
        StartCoroutine(CargarEscenaConDelay("MenuNiveles"));
    }

    public void Salir()
    {
        ReproducirSonidoBoton();
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void Siguiente()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("MenuNiveles2"));
    }

    public void Atras()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("MenuNiveles"));
    }

    //Aqui es donde se tienen que poner el nombre de las escenas

    public void Nivel1()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level1"));
        //animator.Play("FundidoNegro");
    }

    public void Nivel2()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level2"));

    }

    public void Nivel3()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level3"));
        
    }

    public void Nivel4()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level4"));
    }

    public void Nivel5()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level5"));
    }

    public void Nivel6()

    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level6"));
    }

    public void Nivel7()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level7"));
    }

    public void Nivel8()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level8"));
    }

    public void Nivel9()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level9"));
    }

    public void Nivel10()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level10"));
    }

    public void Nivel11()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level11"));
    }

    public void Nivel12()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level12"));
    }

    public void Nivel13()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level13"));
    }

    public void Nivel14()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level14"));
    }

    public void Nivel15()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level15"));
    }

    public void Nivel16()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level16"));
    }

    public void Nivel17()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level17"));
    }

    public void Nivel18()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level18"));
    }

    public void Nivel19()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level19"));
    }

    public void Nivel20()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("Level20"));
    }


    //Crear lo mismo para los 20 niveles . . .

    public void Menu()
    {
        ReproducirSonidoBoton();
        StartCoroutine(CargarEscenaConDelay("MenuInicial"));
    }
}
