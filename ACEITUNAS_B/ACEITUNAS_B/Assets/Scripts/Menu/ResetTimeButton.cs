using UnityEngine;
using UnityEngine.UI;

public class ResetTimerButton : MonoBehaviour
{
    private Button resetButton;

    private void Start()
    {
        resetButton = GetComponent<Button>();
        resetButton.onClick.AddListener(ResetTotalTime);
    }

    public void ResetTotalTime()
    {
        // Buscar el Timer incluyendo objetos inactivos (alternativa para Unity 6)
        Timer timer = FindFirstObjectByType<Timer>(FindObjectsInactive.Include);

        if (timer != null)
        {
            timer.ResetTotalTime();
            Debug.Log("Tiempo total reiniciado con éxito");

            // Buscar el display incluyendo objetos inactivos
            TotalTimeDisplay display = FindFirstObjectByType<TotalTimeDisplay>(FindObjectsInactive.Include);
            if (display != null)
            {
                display.UpdateTotalTimeDisplay();
                Debug.Log("Display actualizado");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el Timer en la escena");
        }
    }
}
