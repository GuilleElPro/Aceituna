using UnityEngine;
using System.Collections;

public class GelatinaEfecto : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool playerInRange = false;
    private Coroutine attackCoroutine;
    [SerializeField] private float attackDelay = 0.5f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("IsGelatina", true);
            attackCoroutine = StartCoroutine(PrepareAttack(other.gameObject));
        }
    }

    
    private IEnumerator PrepareAttack(GameObject player)
    {
        yield return new WaitForSeconds(attackDelay);
        animator.SetBool("IsGelatina", false);


    }
}
