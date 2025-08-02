using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class ControlMusica : MonoBehaviour
{
    public AudioSource audioSource;         // Solo un AudioSource en la escena
    public AudioClip[] canciones;
    public AudioClip[] audiosGente;
    public AudioClip[] audiosCampana;           // Aquí sí puedes arrastrar los .mp3

    public ControlRondas controlRondas;
    public Button botonmusica;
    public Slider sliderVolumen;
    public Sprite iconiEncendido;
    public Sprite iconoApagado;


    private bool musicaEncendida = true;
    private AudioClip cancionActual;
    private AudioClip audioActual;
    private AudioClip audioCampana;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        float volumenInicial = 0.25f;

        sliderVolumen.value = volumenInicial;
        audioSource.volume = volumenInicial;

        SeleccionarCancionAleatoria();

        if (musicaEncendida)
        {
            audioSource.Play();
        }

        botonmusica.onClick.AddListener(ToogleMusica);
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);

        ActualizarIcono();
    }

    void SeleccionarCancionAleatoria()
    {
        int aleatorio = Random.Range(0, canciones.Length);
        cancionActual = canciones[aleatorio];
        audioSource.clip = cancionActual;
    }

    public void ToogleMusica()
    {
        musicaEncendida = !musicaEncendida;

        if (musicaEncendida)
        {
            if (!audioSource.isPlaying)
            {
                SeleccionarCancionAleatoria();
                audioSource.Play();
            }
            sliderVolumen.gameObject.SetActive(true);
        }
        else
        {
            audioSource.Pause();
            sliderVolumen.gameObject.SetActive(false);
        }

        ActualizarIcono();
    }

    public void CambiarVolumen(float valor)
    {
        audioSource.volume = valor;
    }

    public void ActualizarIcono()
    {
        Image imagenBoton = botonmusica.GetComponent<Image>();
        if (imagenBoton != null)
            imagenBoton.sprite = musicaEncendida ? iconiEncendido : iconoApagado;
    }

    public void ReproducirAudioGente()
    {
        if (audioSource == null || audiosGente.Length == 0)
        {
            return;
        }

        int aleatorio = Random.Range(0, audiosGente.Length);
        AudioClip clip = audiosGente[aleatorio];

        audioSource.PlayOneShot(clip);
    }

    public void ReproducirCampanaPorRonda(int ronda)
    {
        StartCoroutine(SonarCampana(ronda));
    }

    IEnumerator SonarCampana(int ronda)
    {
        if (audioSource == null || audiosCampana.Length == 0)
            yield break;

        if (ronda == 1)
        {
            audioSource.PlayOneShot(audiosCampana[0]);  // un golpe
        }
        else if (ronda == 2)
        {
            audioSource.PlayOneShot(audiosCampana[1]);  // dos golpes

        }
        else if (ronda == 3 && audiosCampana.Length > 1)
        {
            audioSource.PlayOneShot(audiosCampana[1]);  // tres golpes
        }
    }

}
