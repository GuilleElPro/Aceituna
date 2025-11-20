using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PausarJuego : MonoBehaviour
{
    [Header("Configuración de Menú Pausa")]
    public GameObject menuPausa;
    public bool juegoPausado = false;

    [Header("Configuración de Sonidos")]
    [SerializeField] private AudioClip sonidoBoton;
    [SerializeField][Range(0f, 1f)] private float volumenSonido = 0.7f;
    [SerializeField] private float delayCambioEscena = 0.3f;
    private AudioSource audioSource;
    private List<AudioSource> audioSourcesPausados = new List<AudioSource>();
    private List<float> tiemposAudioPausado = new List<float>();

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volumenSonido;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !MenuHasPerdido.juegoPerdido && !MenuHasGanado.juegoGanado)
        {
            if (juegoPausado)
            {
                Reanudar(false);
            }
            else
            {
                Pausar(false);
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
        ReanudarSonidos();
        SceneManager.LoadScene(nombreEscena);
    }

    public void VolverAlMenu()
    {
        StartCoroutine(CargarEscenaConDelay("MenuInicial"));
    }

    public void Reanudar()
    {
        Reanudar(true);
    }

    public void Pausar()
    {
        Pausar(true);
    }

    private void Reanudar(bool conSonido)
    {
        if (conSonido)
        {
            ReproducirSonidoBoton();
        }

        menuPausa.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;
        ReanudarSonidos();
    }

    private void Pausar(bool conSonido)
    {
        if (conSonido)
        {
            ReproducirSonidoBoton();
        }

        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
        PausarSonidos();
    }

    private void PausarSonidos()
    {
        audioSourcesPausados.Clear();
        tiemposAudioPausado.Clear();

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allAudioSources)
        {
            // Excluir el AudioSource de UI y los que no están reproduciendo sonido
            if (source != this.audioSource && source.isPlaying)
            {
                audioSourcesPausados.Add(source);
                tiemposAudioPausado.Add(source.time); // Guardar el tiempo actual
                source.Pause();
            }
        }
    }

    private void ReanudarSonidos()
    {
        for (int i = 0; i < audioSourcesPausados.Count; i++)
        {
            AudioSource source = audioSourcesPausados[i];
            if (source != null)
            {
                source.UnPause();
                source.time = tiemposAudioPausado[i]; // Restaurar el tiempo exacto
            }
        }

        audioSourcesPausados.Clear();
        tiemposAudioPausado.Clear();
    }
}