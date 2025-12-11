using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static CartoonFX.CFXR_Effect;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private SpriteRenderer art;

    [Header("Attributes")]
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float power = 2f;
    [SerializeField] private float interactMinSpeed = 0.2f;
    [SerializeField] private float dragDistance = 100f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMovimientoOliva;
    private AudioSource audioSource;
    private bool sonidoReproduciendose = false;

    public CinemachineImpulseSource impulseSource;

    [Header("Child")]
    //[SerializeField] private Transform hijo;

    private bool isDragging;
    private bool inHole;

    private Vector2 dragInputStartPos;

    public GameManager gameManager;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (art == null) art = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        PlayerInput();

        gameManager = GameObject.FindFirstObjectByType<GameManager>();

        // Control de animaciones basado en vidas
        if (gameManager.vidas == 3)
        {
            animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);
        }
        else if (gameManager.vidas == 2)
        {
            animator.SetBool("IsMoving2", rb.linearVelocity.magnitude > 0.1f);
        }
        else if (gameManager.vidas == 1)
        {
            animator.SetBool("IsMoving1", rb.linearVelocity.magnitude > 0.1f);
        }

        // Control general del sonido de movimiento
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            ReproducirSonidoMovimiento();
        }
        else
        {
            DetenerSonidoMovimiento();
        }
    }

    private void ReproducirSonidoMovimiento()
    {
        if (!sonidoReproduciendose && sonidoMovimientoOliva != null && audioSource != null)
        {
            audioSource.clip = sonidoMovimientoOliva;
            audioSource.loop = true;
            audioSource.Play();
            sonidoReproduciendose = true;
        }
    }

    private void DetenerSonidoMovimiento()
    {
        if (sonidoReproduciendose && audioSource != null)
        {
            audioSource.Stop();
            sonidoReproduciendose = false;
        }
    }

    private bool IsReady()
    {
        return rb.linearVelocity.magnitude <= interactMinSpeed || Mathf.Approximately(rb.linearVelocity.magnitude, 0);
    }

    private void PlayerInput()
    {
        if (!IsReady()) return;
        Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(dragInputStartPos, inputPos);

        if (Input.GetMouseButtonDown(0)) DragStart(inputPos);
        if (Input.GetMouseButton(0) && isDragging) DragChange(inputPos);
        if (Input.GetMouseButtonUp(0) && isDragging) DragRelease(inputPos);
    }

    private void DragStart(Vector2 pos)
    {
        Debug.Log("Click input pos: " + pos);
        isDragging = true;
        lr.positionCount = 2;
        dragInputStartPos = pos;
    }

    private void DragChange(Vector2 pos)
    {
        Vector2 dir = (Vector2)dragInputStartPos - pos;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude((dir * power) / 2, maxPower / 2));

        Vector2 faceDir = new Vector2(dir.y, -dir.x);
        var angle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        art.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void DragRelease(Vector2 pos)
    {
        float distance = Vector2.Distance(dragInputStartPos, pos);
        isDragging = false;
        lr.positionCount = 0;

        if (distance < 0.1f) return;

        Vector2 dir = (Vector2)dragInputStartPos - pos;
        float power = FindFirstObjectByType<ArduinoSerial>().powerArduino; // 0–10

        rb.linearVelocity = Vector2.ClampMagnitude(dir * power, maxPower);

        Vector2 faceDir = new Vector2(dir.y, -dir.x);
        var angle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        art.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Realentizacion()
    {
        rb.linearVelocity *= 0.2f;
    }

    public void Acelerar()
    {
        rb.linearVelocity *= 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemigo") || collision.gameObject.CompareTag("Obstaculo")) &&
            collision.relativeVelocity.magnitude > 2f)
        {
            float intensidad = Mathf.Clamp(collision.relativeVelocity.magnitude * 0.2f, 0.1f, 0.4f);
            impulseSource.GenerateImpulse(intensidad);
        }
    }
}