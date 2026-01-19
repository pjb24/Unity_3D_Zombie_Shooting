using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float power;
    public GameObject killScoreText;
    public Player playerScript;
    public ParticleSystem bloodParticle;

    public ParticleSystem explodeParticle;

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(transform.forward * power);
        killScoreText = GameObject.FindGameObjectWithTag("KillScore");
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") || collision.collider.CompareTag("Player"))
        {
            return;
        }

        Instantiate(explodeParticle, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

        if (collision.collider.CompareTag("Zombie"))
        {
            if (!collision.gameObject.GetComponent<Zombie>().IsDead)
            {
                playerScript.AddScore();
                killScoreText.GetComponent<TextMeshProUGUI>().text = $"killScore : {playerScript.score}";
                collision.gameObject.GetComponent<Zombie>().Death();
            }

            Instantiate(bloodParticle, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }

        Destroy(gameObject);
    }
}
