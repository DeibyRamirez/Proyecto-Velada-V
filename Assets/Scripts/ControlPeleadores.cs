using UnityEngine;

public class ControlPeleadores : MonoBehaviour
{
    public Transform puntoSpawnJugador1;
    public Transform puntoSpawnJugador2;

    void Start()
    {
        if (DatosCombate.prefabJugador1 != null && DatosCombate.prefabJugador2 != null)
        {
            Instantiate(DatosCombate.prefabJugador1, puntoSpawnJugador1.position, puntoSpawnJugador1.rotation);
            Instantiate(DatosCombate.prefabJugador2, puntoSpawnJugador2.position, puntoSpawnJugador2.rotation);
        }
        else
        {
            Debug.LogError("No se asignaron los prefabs de los jugadores.");
        }
    }
}