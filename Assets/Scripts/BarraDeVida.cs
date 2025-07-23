using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public string nombreDelPersonaje;  // Asigna "Rubius 1" o "Rubius 2" en el Inspector
    public Slider sliderVida;
    private Salud salud;

    void Start()
    {
        GameObject personaje = GameObject.Find(nombreDelPersonaje);
        if (personaje != null)
        {
            salud = personaje.GetComponent<Salud>();
        }
        else
        {
            Debug.LogError($"No se encontró el personaje con el nombre '{nombreDelPersonaje}'");
        }
    }

    void Update()
    {
        if (salud != null)
        {
            sliderVida.value = salud.vidaActual / salud.vidaMaxima;
        }
    }
}
