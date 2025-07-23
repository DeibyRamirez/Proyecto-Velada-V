using UnityEngine;

public class ControlCombate : MonoBehaviour
{
    public string nombreJugador1;
    public string nombreJugador2;

    public Salud jugador1;
    public Salud jugador2;



    public void Start()
    {
        GameObject objJugador1 = GameObject.Find(nombreJugador1);
        GameObject objJugador2 = GameObject.Find(nombreJugador2);

        if (objJugador1 != null)
            jugador1 = objJugador1.GetComponent<Salud>();

        if (objJugador2 != null)
            jugador2 = objJugador2.GetComponent<Salud>();

        if (jugador1 == null || jugador2 == null)
        {
            Debug.LogError("No se encontraron los personajes especificados en el ControlCombate.");
            return;
        }
    }

}