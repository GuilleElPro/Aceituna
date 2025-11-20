using UnityEngine;
using TMPro;

public class TotalTimeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text totalTimeText;

    private void OnEnable()
    {
        UpdateTotalTimeDisplay();
    }

    public void UpdateTotalTimeDisplay()
    {
        if (Timer.Instance != null && totalTimeText != null)
        {
            totalTimeText.text = "Tiempo Total: " + Timer.Instance.GetTotalTimeFormatted();
        }
    }
}
