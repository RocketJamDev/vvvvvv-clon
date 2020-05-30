using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    // Public members
    public float speed;
    public float gravity;
    public float checkRadius;
    public float waitOnDeath;
    public bool isDebug;
    public GameController gameController;
    public LayerMask whatIsGround;
    public GameObject spawnPoint;
    
    // Audio clips
    public AudioClip jump;
    public AudioClip death;
    public AudioClip trinketOrCheckpoint;

    // Components
    private Rigidbody2D rb;
    private BoxCollider2D mCollider;
    private Animator playerAnimator;
    private AudioSource audioSource;

    // Public members
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool facingUp = true;

    // Private members
    float moveInput;
    bool isGrounded = true;
    bool isDying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Al pulsar la W o la S, solo si estamos en el suelo y estamos vivos
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && isGrounded && !isDying)
        {
            audioSource.PlayOneShot(jump);

            // Invertimos la gravedad y el sprite del personaje
            gravity = -gravity;
            FlipVertically();
        }
    }

    void FixedUpdate()
    {
        if (!isDying)
        {
            isGrounded = CheckIsGrounded();
            moveInput = Input.GetAxis("Horizontal");

            rb.velocity = new Vector2(moveInput * speed, isGrounded ? 0.0f : gravity);

            // Activamos la animación si es necesario.
            playerAnimator.SetBool("isWalking", false);

            // Activamos la animación de caminar.
            if (moveInput > 0.0f || moveInput < 0.0f)
            {
                playerAnimator.SetBool("isWalking", true);
            }

            // Volteamos horizontalmente.
            if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
            {
                FlipHorizontally();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger"))
        {
            StartCoroutine(ManageDeath());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            Checkpoint checkpoint = collision.gameObject.GetComponent<Checkpoint>();
            
            // Comprobamos si el checkpoint está desactivado.
            if(!checkpoint.isActive)
            {
                // Reproducimos sonido y activamos el checkpoint
                audioSource.PlayOneShot(trinketOrCheckpoint);
                gameController.ActivateCheckpoint(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Transition"))
        {
            ChangeRoom(collision.GetComponent<Transition>());
        }
        else if (collision.gameObject.CompareTag("Trinket"))
        {
            // Reproducimos sonido y eliminamos el Trinket
            audioSource.PlayOneShot(trinketOrCheckpoint);
            Destroy(collision.gameObject);

            // Añadimos el trinket al game controller
            gameController.AddTrinket();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Transition"))
        {
            ChangeRoom(collision.GetComponent<Transition>());
        }
    }

    void ChangeRoom(Transition transition)
    {
        if (transition.verticalTransition)
        {
            if (facingUp)
            {
                Vector3 newCameraPos = new Vector3(transition.bottomRoom.position.x, transition.bottomRoom.position.y, Camera.main.transform.position.z);
                gameController.MoveCamera(newCameraPos);
            }
            else
            {
                Vector3 newCameraPos = new Vector3(transition.topRoom.position.x, transition.topRoom.position.y, Camera.main.transform.position.z);
                gameController.MoveCamera(newCameraPos);
            }
        }
        else
        {
            if (facingRight)
            {
                Vector3 newCameraPos = new Vector3(transition.rightRoom.position.x, transition.rightRoom.position.y, Camera.main.transform.position.z);
                gameController.MoveCamera(newCameraPos);
            }
            else
            {
                Vector3 newCameraPos = new Vector3(transition.leftRoom.position.x, transition.leftRoom.position.y, Camera.main.transform.position.z);
                gameController.MoveCamera(newCameraPos);
            }
        }
    }

    bool CheckIsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(mCollider.bounds.center, mCollider.bounds.size, 0f, facingUp ? Vector2.down : Vector2.up, 0.1f, whatIsGround);

        PaintDebugBox(raycastHit);
        
        return raycastHit.collider != null;
    }

    void PaintDebugBox(RaycastHit2D raycastHit)
    {
        if (isDebug)
        {
            Color rayColor;

            if (raycastHit.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Vector2 abajo = facingUp ? Vector2.down : Vector2.up;

            Debug.DrawRay(mCollider.bounds.center + new Vector3(mCollider.bounds.extents.x, 0), abajo * (mCollider.bounds.extents.y + 0.1f), rayColor);
            Debug.DrawRay(mCollider.bounds.center - new Vector3(mCollider.bounds.extents.x, 0), abajo * (mCollider.bounds.extents.y + 0.1f), rayColor);

            if (facingUp)
            {
                Debug.DrawRay(mCollider.bounds.center - new Vector3(mCollider.bounds.extents.x, mCollider.bounds.extents.y + 0.1f), Vector3.right * (mCollider.bounds.size.y), rayColor);
            }
            else
            {
                Debug.DrawRay(mCollider.bounds.center + new Vector3(mCollider.bounds.extents.x, mCollider.bounds.extents.y + 0.1f), -Vector3.right * (mCollider.bounds.size.y), rayColor);
            }

            Debug.Log(raycastHit.collider);
        }
    }

    void FlipHorizontally()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void FlipVertically()
    {
        facingUp = !facingUp;
        Vector2 Scaler = transform.localScale;
        Scaler.y *= -1;
        transform.localScale = Scaler;
    }

    IEnumerator ManageDeath() 
    {
        isDying = true;
        playerAnimator.SetBool("isDying", true);
        audioSource.PlayOneShot(death);

        // Desactivamos el rigidbody, para que no le afecten las físicas.
        rb.Sleep();

        // Esperamos unos segundos.
        yield return new WaitForSeconds(waitOnDeath);
        
        playerAnimator.SetBool("isDying", false);

        // Si estabamos boca abajo antes de morir
        if (gravity > 0.0f) 
        {
            // Reseteamos la gravedad y la dirección del personaje
            FlipVertically();
            gravity *= -1;
        }

        // Movemos al jugador a la posición del checkpoint activo
        transform.position = gameController.activeCheckpoint.transform.position;

        // Pedimos al controlador del juego que mueva la cámara a la habitación de respawn
        gameController.MoveCameraToRespawn();
        
        rb.WakeUp();
        isDying = false;
    }
}
