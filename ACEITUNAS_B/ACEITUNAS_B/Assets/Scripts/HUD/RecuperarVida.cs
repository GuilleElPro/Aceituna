using UnityEngine;
using System.Collections;

public class RecuperarVida : MonoBehaviour
{
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bool vidaRecuperada = GameObject.FindFirstObjectByType<GameManager>().RecuperarVida();

            if (vidaRecuperada)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
