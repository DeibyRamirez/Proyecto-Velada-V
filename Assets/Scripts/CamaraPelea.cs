using UnityEngine;

public class CamaraPelea : MonoBehaviour
{
    public Transform jugador1;
    public Transform jugador2;

    public float altura = 5f;               // Altura de la cámara
    public float distanciaBase = 7f;        // Distancia mínima entre cámara y centro
    public float distanciaExtra = 5f;       // Cuánto se aleja si se separan mucho

    public float suavizadoPosicion = 3f;    // Qué tan suave sigue a los personajes
    public float suavizadoRotacion = 5f;    // Qué tan suave rota hacia ellos

    public float limiteMinZ = -20f;         // Limites para evitar alejarse demasiado
    public float limiteMaxZ = -5f;

    void LateUpdate()
    {
        if (jugador1 == null || jugador2 == null) return;

        // Centro entre los dos jugadores
        Vector3 centro = (jugador1.position + jugador2.position) * 0.5f;

        // Distancia entre ellos en el plano horizontal
        float distancia = Vector3.Distance(jugador1.position, jugador2.position);
        float distanciaFinal = distanciaBase + (distancia * 0.5f);
        distanciaFinal = Mathf.Clamp(distanciaFinal, Mathf.Abs(limiteMaxZ), Mathf.Abs(limiteMinZ));

        // Dirección lateral (jugador2 - jugador1), pero ignoramos Y
        Vector3 direccionLateral = (jugador2.position - jugador1.position);
        direccionLateral.y = 0;
        direccionLateral.Normalize();

        // Obtenemos una dirección perpendicular (para colocar la cámara)
        Vector3 direccionCamara = -Vector3.Cross(direccionLateral, Vector3.up);

        // Posición deseada
        Vector3 posicionDeseada = centro + direccionCamara * distanciaFinal + Vector3.up * altura;

        // Suavizado en la posición
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, Time.deltaTime * suavizadoPosicion);

        // Rotar suavemente hacia el centro
        Quaternion rotacionDeseada = Quaternion.LookRotation(centro - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * suavizadoRotacion);
    }
}
