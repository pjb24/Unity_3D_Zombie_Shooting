using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public Zombie zombieSC;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieSC.timer = 2.5f;
            other.GetComponent<Player>().Damage();
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
