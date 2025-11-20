using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text finalTimeText;

    private float startTime;
    private bool isTimerRunning;
    private float currentTime;

    private float totalGameTime = 0f;
    private const string TotalTimeKey = "TotalGameTime";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    private void Start()
    {
        // Cargar tiempo acumulado al iniciar
        totalGameTime = PlayerPrefs.GetFloat(TotalTimeKey, 0f);
        StartTimer();
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        currentTime = Time.time - startTime;

        // Sumar al tiempo total
        totalGameTime += currentTime;
        PlayerPrefs.SetFloat(TotalTimeKey, totalGameTime);
        PlayerPrefs.Save();

        DisplayFinalTime();
    }

    public string GetTotalTimeFormatted()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalGameTime);
        // Solo muestra minutos:segundos (incluso si hay horas)
        return string.Format("{0:00}:{1:00}",
               (int)timeSpan.TotalMinutes,
               timeSpan.Seconds);
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
        timerText.gameObject.SetActive(true);
    }

    public void ResetTotalTime()
    {
        totalGameTime = 0f;
        PlayerPrefs.SetFloat(TotalTimeKey, 0f);
        PlayerPrefs.Save();

        // Opcional: Mostrar confirmación en consola
        Debug.Log("Tiempo total reiniciado");
    }



    private void Update()
    {
        if (isTimerRunning)
        {
            float elapsedTime = Time.time - startTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    private void UpdateTimerDisplay(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        // Solo muestra minutos:segundos sin texto adicional
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /*
    private void DisplayFinalTime()
    {
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(true);

        int minutes = (int)currentTime / 60;
        int seconds = (int)currentTime % 60;

        finalTimeText.text = $"Tiempo: {minutes:00}:{seconds:00}";



    }
    */

    private void DisplayFinalTime()
    {
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(true);

        int minutes = (int)currentTime / 60;
        int seconds = (int)currentTime % 60;

        // Formato mejorado con tamaño, estilo y espaciado
        finalTimeText.text = $"{minutes:00}:{seconds:00}";



        // Asegurar que el texto tiene suficiente espacio
        finalTimeText.rectTransform.sizeDelta = new Vector2(400, 150);
        finalTimeText.alignment = TextAlignmentOptions.Center;
    }

    public void ResetTimer()
    {
        finalTimeText.gameObject.SetActive(false);
        StartTimer();
    }

    public void HideTimer()
    {
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (finalTimeText != null) finalTimeText.gameObject.SetActive(false);
    }
}
