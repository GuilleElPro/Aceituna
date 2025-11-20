using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuHasGanado : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject olivaPlayer;
    public GameObject menuHasGanado;
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text totalTimeTextFinal;

    [Header("Configuración de Sonido")]
    [SerializeField] private AudioClip sonidoBoton;
    [SerializeField][Range(0f, 1f)] private float volumenSonido = 0.7f;
    [SerializeField] private float delayCambioEscena = 0.3f;
    private AudioSource audioSource;

    public static bool juegoGanado = false;

    private void Awake()
    {
        // Configurar AudioSource
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volumenSonido;
    }

    public void ActivarMenuHasGanado()
    {
        menuHasGanado.SetActive(true);
        juegoGanado = true;
        olivaPlayer.gameObject.SetActive(false);
        Time.timeScale = 0;

        if (Timer.Instance != null)
        {
            Timer.Instance.StopTimer();

            if (SceneManager.GetActiveScene().buildIndex >= 20)
            {
                if (totalTimeTextFinal != null)
                {
                    totalTimeTextFinal.text = "Tiempo Total: " +
                                           Timer.Instance.GetTotalTimeFormatted();
                    totalTimeTextFinal.gameObject.SetActive(true);
                }
            }
        }
    }

    private void ReproducirSonidoBoton()
    {
        if (sonidoBoton != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoBoton);
        }
    }

    private IEnumerator CargarEscenaConDelay(string nombreEscena)
    {
        ReproducirSonidoBoton();
        yield return new WaitForSecondsRealtime(delayCambioEscena);
        SceneManager.LoadScene(nombreEscena);
    }

    private IEnumerator CargarSiguienteNivelConDelay()
    {
        ReproducirSonidoBoton();
        yield return new WaitForSecondsRealtime(delayCambioEscena);

        int nivelActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteNivel = nivelActual + 1;

        if (siguienteNivel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteNivel);
            PlayerPrefs.SetInt("NivelActual", siguienteNivel);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
            SceneManager.LoadScene("CreditosFinales");
        }
    }

    public void SiguienteNivel()
    {
        menuHasGanado.SetActive(false);
        juegoGanado = false;
        Time.timeScale = 1;
        olivaPlayer.gameObject.SetActive(true);

        if (Timer.Instance != null)
        {
            Timer.Instance.ResetTimer();
        }

        StartCoroutine(CargarSiguienteNivelConDelay());
    }

    public void Volver()
    {
        menuHasGanado.SetActive(false);
        juegoGanado = false;
        olivaPlayer.gameObject.SetActive(true);
        Time.timeScale = 1;
        StartCoroutine(CargarEscenaConDelay("MenuInicial"));
    }
}
