using UnityEngine;

public class OrientarHaciaOponente : MonoBehaviour
{
    public Transform oponente;

    void Update()
    {
        if (oponente == null) return;

        Vector3 direccion = oponente.position - transform.position;
        direccion.y = 0; // Ignorar la altura para rotación horizontal
        if (direccion != Vector3.zero)
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 10f);
        }
    }
}