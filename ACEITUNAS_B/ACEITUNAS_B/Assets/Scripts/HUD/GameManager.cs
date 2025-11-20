using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public HUD hud;
    public int vidas = 3;

    [Header("Sprites por vida")]
    public Sprite sprite3Lives;  // Sprite para 3 vidas
    public Sprite sprite2Lives;  // Sprite para 2 vidas
    public Sprite sprite1Life;   // Sprite para 1 vida
    public SpriteRenderer playerSpriteRenderer;
    public GameObject olivaPlayer;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoDañoOliva;
    private AudioSource audioSource;


    [Header("Efecto Flash")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;
    public int numberOfFlashes = 3;

    [Header("Efecto Shake UI Vidas")]
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 3f;

    private bool esInmune = false;
    public float tiempoInmunidad = 2f;
    private Color originalColor;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playerSpriteRenderer != null)
        {
            originalColor = playerSpriteRenderer.color;
        }
    }

    public void RecibirDano()
    {
        if (!esInmune && vidas > 0)
        {
            audioSource.PlayOneShot(sonidoDañoOliva);
            vidas--;
            hud.DesactivarVida(vidas);

            // Efecto de shake en las vidas UI
            if (vidas > 0)
            {
                StartCoroutine(ShakeVidasUI());
            }

            Debug.Log("Daño recibido. Vidas restantes: " + vidas);

            // Actualizar sprite del jugador
            UpdatePlayerSprite();

            // Efectos visuales
            StartCoroutine(FlashEffect());
            StartCoroutine(ActivarInmunidad());

            // Verificar si el jugador murió
            if (vidas <= 0)
            {
                MatarJugador();
            }
        }
    }

    // Método para actualizar el sprite según las vidas
    private void UpdatePlayerSprite()
    {
        if (playerSpriteRenderer == null) return;

        switch (vidas)
        {
            case 2:
                playerSpriteRenderer.sprite = sprite2Lives;
                Debug.Log("Player 2 Vidas");
                break;
            case 1:
                playerSpriteRenderer.sprite = sprite1Life;
                Debug.Log("Player 1 Vida");
                break;
            case 0:
                playerSpriteRenderer.enabled = false;
                Debug.Log("Player Muerto");
                break;
        }
    }

    // Efecto de shake para las vidas en el HUD
    IEnumerator ShakeVidasUI()
    {
        GameObject[] vidaUIs = hud.vidas;
        if (vidaUIs == null || vidaUIs.Length == 0) yield break;

        Vector3[] originalPositions = new Vector3[vidaUIs.Length];
        for (int i = 0; i < vidaUIs.Length; i++)
        {
            if (vidaUIs[i] != null)
            {
                originalPositions[i] = vidaUIs[i].transform.localPosition;
            }
        }

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < vidaUIs.Length; i++)
            {
                if (vidaUIs[i] != null && i < vidas)
                {
                    float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
                    float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
                    vidaUIs[i].transform.localPosition = originalPositions[i] + new Vector3(offsetX, offsetY, 0);
                }
            }
            yield return null;
        }

        // Restaurar posiciones originales
        for (int i = 0; i < vidaUIs.Length; i++)
        {
            if (vidaUIs[i] != null)
            {
                vidaUIs[i].transform.localPosition = originalPositions[i];
            }
        }
    }

    // Efecto de flash al recibir daño
    IEnumerator FlashEffect()
    {
        if (playerSpriteRenderer == null) yield break;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            playerSpriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerSpriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    // Inmunidad temporal después del daño
    IEnumerator ActivarInmunidad()
    {
        esInmune = true;
        Debug.Log("Inmunidad Activada");
        yield return new WaitForSeconds(tiempoInmunidad);
        esInmune = false;
        Debug.Log("Inmunidad Desactivada");
    }

    // Recuperar vida (usado por power-ups)
    public bool RecuperarVida()
    {
        if (vidas >= 3) return false;

        vidas++;
        hud.ActivarVida(vidas - 1); // Ajuste para índice 0-based
        return true;
    }

    // Muerte del jugador
    public void MatarJugador()
    {
        olivaPlayer.gameObject.SetActive(false);
        //playerSpriteRenderer.enabled = false;
        GameObject.FindFirstObjectByType<MenuHasPerdido>().ActivarMenuHasPerdido();
    }

    //Metodo muerte Enemigo al salirse de la mesa

    /*
    public void EnemigoCaida()
    {
        Destroy(gameObject);
    }*/
    
}