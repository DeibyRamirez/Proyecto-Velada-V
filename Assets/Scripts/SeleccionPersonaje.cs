using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SeleccionPersonaje : MonoBehaviour
{
    public Transform contenedorGrid;
    public GameObject prefabBoton;
    public DatosPeleador[] listaPeleadores;

    public Button pelear;
    public Button peleadorRandom;

    public List<TextMeshProUGUI> textosJugador1;
    public List<TextMeshProUGUI> textosJugador2;

    public Transform modeloP1;
    public Transform modeloP2;

    private bool seleccionandoJugador1 = true;

    private DatosPeleador personajeJugador1 = null;
    private DatosPeleador personajeJugador2 = null;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        pelear.gameObject.SetActive(false);

        foreach (DatosPeleador datos in listaPeleadores)
        {
            GameObject boton = Instantiate(prefabBoton, contenedorGrid);
            boton.GetComponent<BotonPeleador>().Configurar(datos, this);
        }

        peleadorRandom.onClick.AddListener(PeleadorRandom);
    }

    public void Seleccionar(DatosPeleador datos)
    {
        if (seleccionandoJugador1)
        {
            personajeJugador1 = datos;
            // GUARDA EL NOMBRE EN DATOSCOMBATE
            DatosCombate.nombreJugador1 = datos.nombre;
            DatosCombate.prefabJugador1 = datos.prefab3D; // <<< AQUÍ
            Debug.Log($"Jugador 1 seleccionado: {datos.nombre}");
            Debug.Log($"Verificación - DatosCombate.nombreJugador1: '{DatosCombate.nombreJugador1}'");

            foreach (var texto in textosJugador1)
                texto.text = datos.nombre;

            seleccionandoJugador1 = false;
        }
        else
        {
            personajeJugador2 = datos;
            // GUARDA EL NOMBRE EN DATOSCOMBATE
            DatosCombate.nombreJugador2 = datos.nombre;
            DatosCombate.prefabJugador2 = datos.prefab3D; // <<< AQUÍ
            Debug.Log($"Jugador 2 seleccionado: {datos.nombre}");
            Debug.Log($"Verificación - DatosCombate.nombreJugador2: '{DatosCombate.nombreJugador2}'");

            foreach (var texto in textosJugador2)
                texto.text = datos.nombre;

            seleccionandoJugador1 = true;
        }

        // VERIFICACIÓN FINAL DE AMBOS NOMBRES
        Debug.Log($"ESTADO ACTUAL - J1: '{DatosCombate.nombreJugador1}', J2: '{DatosCombate.nombreJugador2}'");

        if (datos.vozPresentacion != null)
        {
            audioSource.Stop();
            audioSource.clip = datos.vozPresentacion;
            audioSource.Play();
        }

        VerificarSeleccionCompleta();
    }

    void VerificarSeleccionCompleta()
    {
        if (personajeJugador1 != null && personajeJugador2 != null)
            pelear.gameObject.SetActive(true);
        else
            pelear.gameObject.SetActive(false);
    }

    public void PeleadorRandom()
    {
        if (listaPeleadores.Length < 2)
        {
            Debug.LogWarning("Se necesita al menos 2 peleadores para la selección aleatoria");
            return;
        }

        int index1 = Random.Range(0, listaPeleadores.Length);
        int index2;

        do
        {
            index2 = Random.Range(0, listaPeleadores.Length);
        } while (index2 == index1);

        personajeJugador1 = listaPeleadores[index1];
        personajeJugador2 = listaPeleadores[index2];

        // GUARDA LOS NOMBRES EN DATOSCOMBATE
        DatosCombate.nombreJugador1 = personajeJugador1.nombre;
        DatosCombate.nombreJugador2 = personajeJugador2.nombre;

        DatosCombate.prefabJugador1 = personajeJugador1.prefab3D;
        DatosCombate.prefabJugador2 = personajeJugador2.prefab3D;


        Debug.Log($"Jugador 1 aleatorio: {DatosCombate.nombreJugador1}");
        Debug.Log($"Jugador 2 aleatorio: {DatosCombate.nombreJugador2}");

        foreach (var texto in textosJugador1)
        {
            texto.text = personajeJugador1.nombre;
        }

        foreach (var texto in textosJugador2)
        {
            texto.text = personajeJugador2.nombre;
        }

        // Reproduce la voz del jugador 1 primero
        if (personajeJugador1.vozPresentacion != null)
        {
            audioSource.clip = personajeJugador1.vozPresentacion;
            audioSource.Play();
        }

        if (personajeJugador2.vozPresentacion != null)
        {
            StartCoroutine(ReproducirVozLuego(personajeJugador2.vozPresentacion, 1.5f));
        }

        pelear.gameObject.SetActive(true);
    }

    // Me permite un pequeño descanso para las presentaciones de los peleadores
    System.Collections.IEnumerator ReproducirVozLuego(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }

    public DatosPeleador ObtenerJugador1()
    {
        return personajeJugador1;
    }

    public DatosPeleador ObtenerJugador2()
    {
        return personajeJugador2;
    }
}