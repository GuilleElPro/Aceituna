using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuHasPerdido : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuHasPerdido;
    [SerializeField] private Timer gameTimer;

    [Header("Configuración de Sonido")]
    [SerializeField] private AudioClip sonidoBoton;
    [SerializeField] private AudioClip sonidoFallar;
    [SerializeField][Range(0f, 1f)] private float volumenSonido = 0.7f;
    [SerializeField] private float delayCambioEscena = 0.3f;
    private AudioSource audioSource;

    public static bool juegoPerdido = false;

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

    public void ActivarMenuHasPerdido()
    {
        menuHasPerdido.SetActive(true);
        audioSource.PlayOneShot(sonidoFallar);
        juegoPerdido = true;

        // Ocultar el timer al perder
        if (gameTimer != null)
        {
            gameTimer.HideTimer();
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

    private IEnumerator ReiniciarEscenaConDelay()
    {
        ReproducirSonidoBoton();
        yield return new WaitForSecondsRealtime(delayCambioEscena);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Reintentar()
    {
        menuHasPerdido.SetActive(false);
        juegoPerdido = false;

        // Reiniciar el timer al reintentar
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        StartCoroutine(ReiniciarEscenaConDelay());
    }

    public void Volver()
    {
        menuHasPerdido.SetActive(false);
        juegoPerdido = false;
        Time.timeScale = 1;
        StartCoroutine(CargarEscenaConDelay("MenuInicial"));
    }
}