using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SeleccionMapa : MonoBehaviour
{
    public DatosMapa[] listaMapas;
    public GameObject prefabBoton;
    public Transform contenedorGrid;
    public ScrollRect scrollRect;

    private DatosMapa mapaSeleccionado = null;
    private GameObject mapaInstanciado = null; // Referencia al mapa actualmente instanciado

    public Button mapaRandom;
    public RawImage renderMapaSeleccionado; // Muestra el video
    public Button pelear;

    public VideoPlayer videoPlayer;

    
    void Start()
    {
        Debug.Log("Start ejecutado");
        Debug.Log("Cantidad de mapas en listaMapas: " + listaMapas.Length);

        pelear.gameObject.SetActive(false); // Desactiva el botón de pelear al inicio

        CrearBotones();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        mapaRandom.onClick.AddListener(MapaAleatorio);
    }

    void CrearBotones()
    {
        foreach (DatosMapa datosMapa in listaMapas)
        {
            if (datosMapa == null)
            {
                Debug.LogError("Uno de los DatosMapa es null");
                continue;
            }

            GameObject boton = Instantiate(prefabBoton, contenedorGrid);
            Debug.Log("Botón creado para: " + datosMapa.nombre);

            BotonMapa botonComponent = boton.GetComponent<BotonMapa>();
            if (botonComponent != null)
            {
                botonComponent.Configurar(datosMapa, this);
            }
            else
            {
                Debug.LogError("El prefab no tiene componente BotonMapa");
            }
        }

        Canvas.ForceUpdateCanvases();
    }

    public void SeleccionarMapa(DatosMapa datosMapa)
    {
        mapaSeleccionado = datosMapa;
        Debug.Log("Mapa seleccionado: " + datosMapa.nombre);

        // Configurar el video
        if (videoPlayer != null && datosMapa.videoMapa != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = datosMapa.videoMapa;

            // Crear un nuevo RenderTexture
            RenderTexture rt = new RenderTexture(1920, 1080, 0);
            videoPlayer.targetTexture = rt;
            renderMapaSeleccionado.texture = rt;

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.Play();
        }

        // Instanciar el nuevo mapa
        InstanciarMapa(datosMapa);

        pelear.gameObject.SetActive(true);
    }

    private void InstanciarMapa(DatosMapa datosMapa)
    {
        // Destruir el mapa anterior si existe
        if (mapaInstanciado != null)
        {
            Debug.Log("Destruyendo mapa anterior: " + mapaInstanciado.name);
            DestroyImmediate(mapaInstanciado);
            mapaInstanciado = null;
        }

        // Instanciar el nuevo mapa
        if (datosMapa.prefabMapa != null)
        {
            // Instanciar el prefab manteniendo su posición y rotación original
            mapaInstanciado = Instantiate(datosMapa.prefabMapa);
            mapaInstanciado.name = "Mapa_" + datosMapa.nombre;
            
            Debug.Log($"Mapa instanciado: {mapaInstanciado.name}");
            Debug.Log($"Posición: {mapaInstanciado.transform.position}");
            Debug.Log($"Rotación: {mapaInstanciado.transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("El mapa " + datosMapa.nombre + " no tiene prefab asignado");
        }
    }

    public DatosMapa ObtenerMapaSeleccionado()
    {
        return mapaSeleccionado;
    }

    public GameObject ObtenerMapaInstanciado()
    {
        return mapaInstanciado;
    }

    public void ScrollToButton(GameObject boton)
    {
        if (scrollRect != null && boton != null)
        {
            Canvas.ForceUpdateCanvases();

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;
            RectTransform botonRect = boton.GetComponent<RectTransform>();

            Vector2 botonPos = (Vector2)scrollRect.transform.InverseTransformPoint(botonRect.position);
            Vector2 viewportPos = (Vector2)scrollRect.transform.InverseTransformPoint(viewport.position);

            float difference = viewportPos.y - botonPos.y;
            float normalizedDifference = difference / (content.rect.height - viewport.rect.height);

            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + normalizedDifference);
        }
    }

    void MapaAleatorio()
    {
        if (listaMapas.Length < 2)
        {
            Debug.LogWarning("Se necesita al menos 2 mapas para la selección aleatoria");
            return;
        }

        int randomIndex = Random.Range(0, listaMapas.Length);
        SeleccionarMapa(listaMapas[randomIndex]);
        Debug.Log("Mapa aleatorio seleccionado: " + listaMapas[randomIndex].nombre);

        pelear.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        // Limpiar el mapa instanciado cuando se destruye este script
        if (mapaInstanciado != null)
        {
            DestroyImmediate(mapaInstanciado);
        }
    }
}