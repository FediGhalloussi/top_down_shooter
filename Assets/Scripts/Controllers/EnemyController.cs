using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    public Transform target;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            rb.velocity = direction * moveSpeed;

            RotateTowards(target.position);
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            //GameManager.Instance.PlayerAvatar.TakeDamage(enemyAvatar.Damage);
            EnemyFactory.ReleaseEnemy(this.GetComponent<EnemyAvatar>());
            GameManager.Instance.PlayerAvatar.TakeDamage(1);
        }
    }
}