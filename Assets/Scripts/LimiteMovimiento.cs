using UnityEngine;

public class LimiteMovimiento : MonoBehaviour
{
    public Transform otroJugador;
    public float distanciaMaxima = 10f;


    void Update()
    {
        float distancia = Vector3.Distance(transform.position, otroJugador.position);

        if (distancia > distanciaMaxima)
        {
            Vector3 direccion = (transform.position - otroJugador.position).normalized;
            transform.position = otroJugador.position + direccion * distanciaMaxima;
        }
    }
}
