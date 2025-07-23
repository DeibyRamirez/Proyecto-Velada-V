using UnityEngine;

public class ControlPantalla : MonoBehaviour
{
    public GameObject[] pantalla;
    public GameObject rawImagenFondo;
    public ControlVideo controladorVideo;
    public TMPro.TMP_Text textoGanador;
    
    private int pantallaActual = 0;

    void Start()
    {
        MostrarPantalla(pantallaActual);
    }

    public void SiguientePantalla()
    {
        pantallaActual++;
        if (pantallaActual >= pantalla.Length)
            pantallaActual = pantalla.Length - 1;

        MostrarPantalla(pantallaActual);
    }
    
    public void AnteriorPantalla()
    {
        pantallaActual--;
        if (pantallaActual < 0)
            pantallaActual = 0;
        MostrarPantalla(pantallaActual);
    }

    public void IrPantalla(int index)
    {
        MostrarPantalla(index);
    }

    public void MostrarPantalla(int index)
    {
        for (int i = 0; i < pantalla.Length; i++)
        {
            pantalla[i].SetActive(i == index);
        }

        pantallaActual = index;

        // VERIFICA QUE HAY UN GANADOR VÁLIDO ANTES DE MOSTRARLO
        if (pantallaActual == 5 && textoGanador != null)
        {
            if (!string.IsNullOrEmpty(DatosCombate.nombreGanador))
            {
                textoGanador.text = $"¡{DatosCombate.nombreGanador} ha ganado la pelea!";
                Debug.Log($"Mostrando ganador: {DatosCombate.nombreGanador}");
            }
            else
            {
                textoGanador.text = "Error: No se pudo determinar el ganador";
                Debug.LogError("No hay ganador válido para mostrar");
            }
        }

        if (pantallaActual == 4)
        {
            controladorVideo.ActualizarVideo(-1);
            rawImagenFondo.SetActive(false);
        }
        else
        {
            controladorVideo.ActualizarVideo(pantallaActual);
            rawImagenFondo.SetActive(true);
        }
    }

    public void SetGanador(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre))
        {
            textoGanador.text = $"¡{nombre} ha ganado la pelea!";
            Debug.Log($"Ganador establecido: {nombre}");
        }
        else
        {
            Debug.LogWarning("Se intentó establecer un ganador vacío");
        }
    }
}