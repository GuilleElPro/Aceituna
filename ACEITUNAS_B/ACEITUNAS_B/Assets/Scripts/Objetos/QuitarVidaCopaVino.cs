using UnityEngine;

public class QuitarVidaCopaVino : MonoBehaviour
{

    public float factorRalentizacion;
    /*
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            other.gameObject.GetComponent<GameManager>().RecibirDano();

            Debug.Log("Pierde Vida Vino");

        }
    } */

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            GameObject.FindFirstObjectByType<GameManager>().RecibirDano();
            other.gameObject.GetComponent<Ball>().Realentizacion();

            Debug.Log("Pierde Vida Vino");

        }
    }
}
