using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditosController : MonoBehaviour
{
    public float duracionCreditos = 30f; // Ajusta según la duración de tu animación

    void Start()
    {
        StartCoroutine(FinCreditos());
    }

    /*
    public void CreditosFinal()
    {
        SceneManager.LoadScene("MenuInicial");
    }
    */

    IEnumerator FinCreditos()
    {
        yield return new WaitForSeconds(duracionCreditos);
        SceneManager.LoadScene("MenuInicial");
    }
}
