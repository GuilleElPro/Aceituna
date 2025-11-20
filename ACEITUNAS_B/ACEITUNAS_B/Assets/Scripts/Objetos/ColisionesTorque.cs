using UnityEngine;

public class ColisionesTorque : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGolpeOliva;
    private AudioSource audioSource;

    public float torqueForce = 200f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sonidoGolpeOliva);
            other.rigidbody.AddForce((other.transform.position - transform.position).normalized * 100f);
            

            //Fuerza con la que sale disparada la oliva al colisionar
            other.rigidbody.AddTorque(torqueForce);
            Debug.Log("Pared colision");
        }


    }
}
