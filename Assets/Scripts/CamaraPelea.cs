using UnityEngine;

public class CamaraPelea : MonoBehaviour
{
    public Transform jugador1;
    public Transform jugador2;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float suavizado = 2f;
    public float distanciaMinima = 5f;
    public float distanciaMaxima = 10f;

    void LateUpdate()
    {
        Vector3 medio = (jugador1.position + jugador2.position) / 2f;

        float distancia = Vector3.Distance(jugador1.position, jugador2.position);
        float zoom = Mathf.Clamp(distancia, distanciaMinima, distanciaMaxima);

        Vector3 posicionDeseada = medio + offset.normalized * zoom;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, Time.deltaTime * suavizado);

        transform.LookAt(medio);
    }
}
