using UnityEngine;
using UnityEngine.SceneManagement;

public class Preparadorcombate : MonoBehaviour
{
    public SeleccionMapa seleccionMapa;
    public SeleccionPersonaje seleccionPersonaje;

    public ControlPantalla controlPantalla;

    public void PrepararDatosYPelear()
    {
        var datosMapa = seleccionMapa.ObtenerMapaSeleccionado();
        var datosJugador1 = seleccionPersonaje.ObtenerJugador1();
        var datosJugador2 = seleccionPersonaje.ObtenerJugador2();


        if (datosMapa == null || datosJugador1 == null || datosJugador2 == null)
        {
            Debug.LogError("Faltan datos para preparar el combate.");
            return;
        }
        // Aquí puedes usar los datos obtenidos para preparar el combate
        DatosCombate.nombreMapa = datosMapa.nombre;
        DatosCombate.nombreJugador1 = datosJugador1.nombre;
        DatosCombate.nombreJugador2 = datosJugador2.nombre;

        Debug.Log($"Jugador 1: {DatosCombate.nombreJugador1}");
        Debug.Log($"Jugador 2: {DatosCombate.nombreJugador2}");
        Debug.Log($"Mapa seleccionado: {DatosCombate.nombreMapa}");

        controlPantalla.IrPantalla(4);
    }
}