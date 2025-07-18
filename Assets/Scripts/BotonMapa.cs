using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonMapa : MonoBehaviour
{
    public Image imagenUI;
    public TextMeshProUGUI nombreUI;

    [HideInInspector] public DatosMapa datosMapa;
    [HideInInspector] public SeleccionMapa selector;

    public void Configurar(DatosMapa datosM, SeleccionMapa sel)
    {
        datosMapa = datosM;
        selector = sel;

        if (imagenUI != null)
            imagenUI.sprite = datosMapa.imagenMapa;
        else
            Debug.LogError("imagenUI no está asignado en " + gameObject.name);

        if (nombreUI != null)
            nombreUI.text = datosMapa.nombre;
        else
            Debug.LogError("nombreUI no está asignado en " + gameObject.name);

        // Validación mejorada del botón
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.RemoveAllListeners(); // Limpia listeners previos
            boton.onClick.AddListener(SeleccionarMapa);
        }
        else
        {
            Debug.LogError("No se encontró componente Button en " + gameObject.name);
        }
    }

    void SeleccionarMapa()
    {
        if (selector != null && datosMapa != null)
        {
            selector.SeleccionarMapa(datosMapa);
        }
        else
        {
            Debug.LogError("Selector o datosMapa son null en " + gameObject.name);
        }
    }
}