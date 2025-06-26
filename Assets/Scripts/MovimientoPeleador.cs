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

    private Rigidbody rb;
    private bool enSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    
    }

    [System.Obsolete]
    void Update()
    {
        Vector3 movimiento = Vector3.zero;

        if (Input.GetKey(izquierda))
        {
            movimiento += Vector3.left; // Mueve a la izquierda
        }
        else if (Input.GetKey(derecha))
        {
            movimiento += Vector3.right; // Mueve a la derecha
        }

        // Movimiento con profundidad

        if (Input.GetKey(KeyCode.W))
        {
            movimiento += Vector3.forward; // Mueve hacia adelante
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movimiento += Vector3.back; // Mueve hacia atrás
        }

        rb.velocity = new Vector3(movimiento.x * velocidad, rb.velocity.y, movimiento.z * velocidad);

        // Saltar solo si está en el suelo
        if (Input.GetKeyDown(salto) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzasalto, ForceMode.Impulse);
            enSuelo = false; // Evita saltos múltiples en el aire
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
