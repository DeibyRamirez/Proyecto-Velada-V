using UnityEngine;

public class MovimientoPeleador : MonoBehaviour
{
    // Valores que se ingresar por dafault al codigo.
    public float velocidad = 5f;
    public float fuerzasalto = 7f;

    // Teclas para el movimiento del peleador.
    public KeyCode izquierda;
    public KeyCode derecha;
    public KeyCode salto;
    public KeyCode ataque;

    private Rigidbody rb;
    private Animator animator;
    private bool enSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    [System.Obsolete]
    void Update()
    {
        Vector3 movimiento = Vector3.zero;

        bool hayMovimiento = false;
        bool isAttacking = false;

        if (Input.GetKey(izquierda))
        {
            movimiento += Vector3.left;
            hayMovimiento = true;
        }
        if (Input.GetKey(derecha))
        {
            movimiento += Vector3.right;
            hayMovimiento = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            movimiento += Vector3.forward;
            hayMovimiento = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movimiento += Vector3.back;
            hayMovimiento = true;
        }

        if (Input.GetKeyDown(ataque))
        {
            isAttacking = true;
            animator.SetBool("isHitting", isAttacking);
        }
        else
        {
            animator.SetBool("isHitting", false);
        }

        rb.velocity = new Vector3(movimiento.x * velocidad, rb.velocity.y, movimiento.z * velocidad);

        // Actualizar la animación
        animator.SetBool("isWalking", hayMovimiento);

        

        if (Input.GetKeyDown(salto) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzasalto, ForceMode.Impulse);
            enSuelo = false;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true; // Permite saltar nuevamente al tocar el suelo
        }
    }

}
