using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionPersonaje : MonoBehaviour
{

    public Transform contenedorGrid;
    public GameObject prefabBoton;
    public DatosPeleador[] listaPeleadores;

    public Button pelear;

    public TextMeshProUGUI textoJugador1;
    public TextMeshProUGUI textoJugador2;

    public Transform modeloP1;
    public Transform modeloP2;

    private bool seleccionandoJugador1 = true;

    private DatosPeleador personajeJugador1 = null;
    private DatosPeleador personajeJugador2 = null;

    private AudioSource audioSource;



    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // Ocultar boton de pelea al inicio...
        pelear.gameObject.SetActive(false);

        // Recorre todos los personajes disponibles y crea un botón por cada uno en el grid
        foreach (DatosPeleador datos in listaPeleadores)
        {
            // Crea una instancia del botón de personaje
            GameObject boton = Instantiate(prefabBoton, contenedorGrid);
            boton.GetComponent<BotonPeleador>().Configurar(datos, this);
        }
    }

    public void Seleccionar(DatosPeleador datos)
    {
        Debug.Log("Seleccionado: " + datos.nombre + " - Jugador1? " + seleccionandoJugador1);

        if (seleccionandoJugador1)
        {
            personajeJugador1 = datos;
            textoJugador1.text = datos.nombre;
            // InstanciarModelo(datos.prefab3D, modeloP1);
            seleccionandoJugador1 = false;
            Debug.Log("Ahora le toca al Jugador 2");
        }
        else
        {
            personajeJugador2 = datos;
            textoJugador2.text = datos.nombre;
            //InstanciarModelo(datos.prefab3D, modeloP2);
            seleccionandoJugador1 = true;
            Debug.Log("Ahora le toca al Jugador 1");
        }

        if (datos.vozPresentacion != null)
        {
            audioSource.Stop();
            audioSource.clip = datos.vozPresentacion;
            audioSource.Play();

        }

        VerificarSeleccionCompleta();


        void VerificarSeleccionCompleta()
        {
            if (personajeJugador1 != null && personajeJugador2 != null)
            {
                pelear.gameObject.SetActive(true);
            }
            else
            {
                pelear.gameObject.SetActive(false);
            }
        }
    }


    //void InstanciarModelo(GameObject prefab, Transform zona)
    // {
    //     // Elimina cualquier modelo anterior en la zona
    //    foreach (Transform hijo in zona)
    //    {//            Destroy(hijo.gameObject);
    //    }
    //
    //        GameObject modelo = Instantiate(prefab, zona);

    // Ajusta posición, rotación y escala del modelo para que se vea bien
    //   modelo.transform.localPosition = Vector3.zero;
    //   modelo.transform.localRotation = Quaternion.Euler(0, 180, 0);
    //   modelo.transform.localScale = Vector3.one * 100;
    //  }

}