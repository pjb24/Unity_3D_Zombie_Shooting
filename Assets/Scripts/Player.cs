using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody rb;
    public Animator playerAnimator;

    public GameObject bulletPrefab;
    public Transform spawnPos;

    public GameObject particlePrefab;
    public ParticleSystem muzzleEffect;

    public int score;

    public int life = 10;
    public TextMeshProUGUI lifeText;
    public GameObject panel;

    public bool alive;

    [Header("Aim (Mouse -> World)")]
    [SerializeField] float aimPlaneY = 0f;         // 가상 평면 높이(지면 콜라이더 없을 때 사용)
    [SerializeField] LayerMask aimMask = ~0;       // 지면 레이어(있으면 우선 사용)
    [SerializeField] bool drawAimDebug = false;

    private Camera _cam;

    public float fireInterval = 0.05f;
    private float _fireDelay;
    private bool _isFireReady;

    private bool _isInputFire;
    private bool _isInputUp;
    private bool _isInputDown;
    private bool _isInputRight;
    private bool _isInputLeft;

    private void Start()
    {
        _cam = GetComponent<Camera>();

        life = 10;
        alive = true;
    }

    public void AddScore()
    {
        score = score + 1;
    }

    public void Damage()
    {
        Debug.Log(1);
        if (life > 0)
        {
            life--;
            lifeText.text = $"life : {life}";
        }

        else if (life <= 0)
        {
            lifeText.text = $"life : 0";
            playerAnimator.SetTrigger("IsDead");
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            Restart();
            panel.SetActive(true);
            alive = false;
        }
    }

    public void Restart()
    {
        StartCoroutine("RestartScene");
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FixedUpdate()// 게임 플레이 버튼을 누르고 게임이 끝날 때 까지 실행
    {
        if (alive)
        {
            float x = 0f;
            float y = 0f;

            // 위쪽 키를 눌렀을 때
            if (_isInputUp) // 인풋시스템의 (위쪽방향키 키값) 키를 받오는 기능일때 
            {
                // this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                y = 1f;
            }
            else if (_isInputDown) // 아래 방향키를 눌렀을 때
            {
                // this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                y = -1f;
            }

            if (_isInputRight) // 오른쪽 방향키를 눌렀을 때
            {
                // this.gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                x = 1f;
            }
            else if (_isInputLeft) // 왼쪽 방향키를 눌렀을 때
            {
                // this.gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
                x = -1f;
            }

            if (x != 0f || y != 0f)
            {
                //게임 오브젝트를 (방향(속도*시간)) 이동시켜라
                gameObject.transform.position += (new Vector3(x, 0, y).normalized * speed * Time.deltaTime);
                playerAnimator.SetBool("IsRun", true);
            }
        }
    }

    private void Update()
    {
        if (alive)
        {
            // 위쪽 키를 눌렀을 때
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) // 인풋시스템의 (위쪽방향키 키값) 키를 받오는 기능일때 
            {
                _isInputUp = true;
            }
            else
            {
                _isInputUp = false;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) // 아래 방향키를 눌렀을 때
            {
                _isInputDown = true;
            }
            else
            {
                _isInputDown = false;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) // 오른쪽 방향키를 눌렀을 때
            {
                _isInputRight = true;
            }
            else
            {
                _isInputRight = false;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) // 왼쪽 방향키를 눌렀을 때
            {
                _isInputLeft = true;
            }
            else
            {
                _isInputLeft = false;
            }

            if (!_isInputUp && !_isInputDown && !_isInputRight && !_isInputLeft)
            {
                playerAnimator.SetBool("IsRun", false);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(0))
            {
                if (_isFireReady)
                {
                    //파티클 소환
                    GameObject _bullet = Instantiate(bulletPrefab, spawnPos.position, this.transform.rotation);   //총알 소환
                    var _muzzle = Instantiate(muzzleEffect, spawnPos.position, this.transform.rotation);   //총알 소환
                    var _particle = Instantiate(particlePrefab, spawnPos.position, this.transform.rotation);   //총알 소환
                    
                    _isFireReady = false;
                }
            }

            if (!_isFireReady)
            {
                _fireDelay += Time.deltaTime;
                if (_fireDelay > fireInterval)
                {
                    _fireDelay = 0f;
                    _isFireReady = true;
                }
            }

            // === 2) 마우스가 가리키는 월드 지점으로 캐릭터 Yaw 회전 ===
            Vector3? aim = GetMouseAimPoint();
            if (aim.HasValue)
            {
                Vector3 to = aim.Value - transform.position;
                to.y = 0f;
                if (to.sqrMagnitude > 0.0001f)
                {
                    float yaw = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, yaw, 0f);
                }

                if (drawAimDebug)
                    Debug.DrawLine(transform.position + Vector3.up * 0.1f, aim.Value, Color.cyan, 0f, false);
            }
        }
    }

    Vector3? GetMouseAimPoint()
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return null;

        Vector2 screen = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(screen);

        // 1) 지면 콜라이더를 우선 사용
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, aimMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        Debug.Log("[Player] GetMouseAimPoint - 가상 평면 교차");

        // 2) 콜라이더가 없으면 가상 평면 교차
        Plane plane = new Plane(Vector3.up, new Vector3(0f, aimPlaneY, 0f));
        if (plane.Raycast(ray, out float dist))
            return ray.GetPoint(dist);

        return null;
    }
}
