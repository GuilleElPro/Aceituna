using UnityEngine;

public class RalentizacionParticle : MonoBehaviour
{
    public ParticleSystem collisionParticles; // Asigna el ParticleSystem desde el Inspector
    public GameObject particlePrefab; // Asigna el prefab desde el Inspector.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si colisiona con un obstáculo (usando Tags o Layers)
        if (collision.gameObject.CompareTag("Liquido"))
        {
            GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liquido"))
        {
            GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
        }
    }
}
