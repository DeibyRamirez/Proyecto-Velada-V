using UnityEngine;

public enum TipoPelea { JugadorVsJugador, JugadorVsCPU }


public static class DatosCombate
{
    public static string nombreMapa;
    public static string nombreJugador1;
    public static string nombreJugador2;

    public static string nombreGanador;
    public static GameObject prefabJugador1;
    public static GameObject prefabJugador2;

    public static TipoPelea tipoPelea;
}