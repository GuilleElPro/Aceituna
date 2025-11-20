using UnityEngine;

public class EnemyParticle : MonoBehaviour
{
    public ParticleSystem collisionParticles; // Asigna el ParticleSystem desde el Inspector
    public GameObject particlePrefab; // Asigna el prefab desde el Inspector.

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si colisiona con un obstáculo (usando Tags o Layers)
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            
            // Activa las partículas en la posición de colisión
            if (collisionParticles != null)
            {
                Debug.Log("Particle System");
                //collisionParticles.transform.position = collision.contacts[0].point;
                //collisionParticles.Play();

                // Instancia las partículas en el punto de colisión
                GameObject particles = Instantiate(particlePrefab, collision.contacts[0].point, Quaternion.identity);
                particles.GetComponent<ParticleSystem>().Play();

                // Destruye el objeto después de que terminen las partículas
                Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }
}
