using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Esta clase representa el botón de un peleador en la selección de personajes
public class BotonPeleador : MonoBehaviour
{
    // Referencia loa atributos del peleador
    public Image imagenUI;
    
    public TextMeshProUGUI nombreUI;

    // Referencia a los datos del peleador (se asigna desde otro script)
    [HideInInspector] public DatosPeleador datos;
    // Referencia al script que maneja la selección de personajes
    [HideInInspector] public SeleccionPersonaje selector;


    // Método para configurar el botón con los datos del peleador y el selector
    public void Configurar(DatosPeleador datosP, SeleccionPersonaje sel)
    {
        // Asigna los datos del peleador y el selector recibidos por parámetro
        datos = datosP;
        selector = sel;

        // Cambia la imagen del botón por la imagen del peleador
        imagenUI.sprite = datos.imagen;
        // Cambia el texto del botón por el nombre del peleador
        nombreUI.text = datos.nombre;

        // Agrega el método Seleccionar como listener al evento onClick del botón
        GetComponent<Button>().onClick.AddListener(Seleccionar);
    }

    // Método que se llama cuando se hace clic en el botón
    void Seleccionar()
    {
        // Llama al método Seleccionar del selector, pasando los datos del peleador seleccionado
        selector.Seleccionar(datos);
    }
}