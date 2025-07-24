using UnityEngine;

public class MovimientoPeleador : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzasalto = 7f;

    // Teclas para el movimiento del peleador.
    public KeyCode izquierda;
    public KeyCode derecha;
    public KeyCode salto;
    public KeyCode ataque;
    public KeyCode defensa;
    public KeyCode golpear;

    public Transform oponente; // ← NUEVO: para saber hacia dónde moverse

    private Rigidbody rb;
    private Animator animator;
    private bool enSuelo = true;
    private bool yaMurio = false;
    Salud salud;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        salud = GetComponent<Salud>();

        animator.SetBool("isWinner", false);
    }

    [System.Obsolete]
    void Update()
    {
        Vector3 movimiento = Vector3.zero;
        bool hayMovimiento = false;

        // === Movimiento relativo al oponente ===
        if (oponente != null)
        {
            Vector3 frente = oponente.position - transform.position;
            frente.y = 0;
            frente.Normalize();
            Vector3 derechaRelativa = Vector3.Cross(Vector3.up, frente).normalized;

            if (Input.GetKey(izquierda))
            {
                movimiento -= derechaRelativa;
                hayMovimiento = true;
            }
            if (Input.GetKey(derecha))
            {
                movimiento += derechaRelativa;
                hayMovimiento = true;
            }

            // Movimiento hacia delante/atrás (opcional)
            if (Input.GetKey(KeyCode.W))
            {
                movimiento += frente;
                hayMovimiento = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movimiento -= frente;
                hayMovimiento = true;
            }

            // Rotar hacia el oponente automáticamente
            if (frente != Vector3.zero)
            {
                Quaternion rotacion = Quaternion.LookRotation(frente);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 10f);
            }
        }

        // === Acciones ===
        animator.SetBool("isWalking", hayMovimiento);

        animator.SetBool("isHitting", Input.GetKeyDown(ataque));
        animator.SetBool("isBoxing", Input.GetKeyDown(golpear));
        animator.SetBool("isBlocking", Input.GetKey(defensa));

        rb.velocity = new Vector3(movimiento.x * velocidad, rb.velocity.y, movimiento.z * velocidad);

        if (Input.GetKeyDown(salto) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzasalto, ForceMode.Impulse);
            enSuelo = false;
        }

        // === Verificar muerte ===
        if (!yaMurio && salud.vidaActual <= 0)
        {
            yaMurio = true;
            animator.SetTrigger("Die");
            Debug.Log("El peleador ha sido derrotado.");
            this.enabled = false;

            GameObject[] personajes = GameObject.FindGameObjectsWithTag("Personaje");
            foreach (GameObject personaje in personajes)
            {
                if (personaje != this.gameObject)
                {
                    Salud saludOtro = personaje.GetComponent<Salud>();
                    if (saludOtro != null && saludOtro.vidaActual > 0)
                    {
                        Animator otroAnimator = personaje.GetComponent<Animator>();
                        if (otroAnimator != null)
                        {
                            otroAnimator.SetBool("isWinner", true);
                        }
                    }
                }
            }
        }
    }

    public void ReiniciarEstado()
    {
        yaMurio = false;
        enSuelo = true;

        // Reiniciar animaciones
        animator.SetBool("isWalking", false);
        animator.SetBool("isHitting", false);
        animator.SetBool("isBoxing", false);
        animator.SetBool("isBlocking", false);
        animator.SetBool("isWinner", false);
        animator.ResetTrigger("Die");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }
}
