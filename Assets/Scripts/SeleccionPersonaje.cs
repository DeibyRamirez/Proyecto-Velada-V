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

            foreach (var texto in textosJugador1)
                texto.text = datos.nombre;

            seleccionandoJugador1 = false;
        }
        else
        {
            personajeJugador2 = datos;

            foreach (var texto in textosJugador2)
                texto.text = datos.nombre;

            seleccionandoJugador1 = true;
        }

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
            Debug.LogWarning("Se nesecita al menos 2 peleadores para la selección aleatoria");
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

    // Me permite un pequeño descanso para los presentación de los peleadores
    System.Collections.IEnumerator ReproducirVozLuego(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
