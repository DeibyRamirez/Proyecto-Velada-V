using UnityEngine;

public class ControlPantalla : MonoBehaviour
{
    public GameObject[] pantalla;

    public GameObject rawImagenFondo;
    public ControlVideo controladorVideo;
    private int pantallaActual = 0;

    void Start()
    {
        MostrarPantalla(pantallaActual);
    }

    public void SiguientePantalla()
    {
        pantallaActual++;
        if (pantallaActual >= pantalla.Length)
            pantallaActual = pantalla.Length - 1; // Asegura que no se salga del rango

        MostrarPantalla(pantallaActual);
    }
    public void AnteriorPantalla()
    {
        pantallaActual--;
        if (pantallaActual < 0)
            pantallaActual = 0; // Asegura que no se salga del rango
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

        
        if (pantallaActual == 4)
        {
            controladorVideo.ActualizarVideo(-1); // Oculta o detiene el video
            rawImagenFondo.SetActive(false); // Muestra la imagen de fondo
        }
        else
        {
            controladorVideo.ActualizarVideo(pantallaActual);
            rawImagenFondo.SetActive(true); // Muestra el video
        }
    }

}
