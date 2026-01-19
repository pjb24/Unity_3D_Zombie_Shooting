using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GameObject player;
    public Animator zombieAnimator;

    public float timer;
    public Collider zombieHand;

    private bool _isDead;
    public bool IsDead => _isDead;
    private float _interval;
    private bool _canAttack;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
        {
            navMeshAgent.isStopped = true;
            return;
        }

        navMeshAgent.SetDestination(player.transform.position);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (_canAttack)
            {
                zombieAnimator.SetBool("IsAttack", true);
                navMeshAgent.isStopped = true;
                zombieHand.enabled = true;

                _canAttack = false;
            }
        }

        else if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if (_canAttack)
            {
                zombieAnimator.SetBool("IsAttack", false);
                navMeshAgent.isStopped = false;
                zombieHand.enabled = false;
            }
        }

        AttackDealy();
    }

    void AttackDealy()
    {
        if (_canAttack)
        {
            return;
        }

        if (_interval <= 0f)
        {
            _canAttack = true;
            _interval = timer;
        }

        _interval -= Time.deltaTime;
    }

    public void Death()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            zombieAnimator.SetTrigger("Death1");
        }
        else
        {
            zombieAnimator.SetTrigger("Death2");
        }

        Destroy(gameObject, 3f);
    }
}
