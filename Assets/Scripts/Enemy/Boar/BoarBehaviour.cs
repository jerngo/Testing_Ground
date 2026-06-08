using System.Collections;
using UnityEngine;

public class BoarBehaviour : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float moveSpeed = 2f;
    public float waitTime = 1f;

    private Transform targetPoint;
    private Animator anim;
    private SpriteRenderer sr;

    public Transform enemyHit;

    float hitOffsetX = 0.1170797f;
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        targetPoint = pointA;

        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            
            anim.SetBool("IsRunning", true);

            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                MoveToTarget();
                yield return null;
            }

            
            anim.SetBool("IsRunning", false);

            
            yield return new WaitForSeconds(waitTime);

            
            Flip();

            
            yield return new WaitForSeconds(waitTime);

            
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void MoveToTarget()
    {
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void Flip()
    {
        if (sr == null) return;

        sr.flipX = !sr.flipX;

        UpdateHitbox();
    }

    void UpdateHitbox()
    {
        Vector3 pos = enemyHit.localPosition;

        if (sr.flipX)
            pos.x = hitOffsetX;
        else
            pos.x = -hitOffsetX;

        enemyHit.localPosition = pos;
    }
}
