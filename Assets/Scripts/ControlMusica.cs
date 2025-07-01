using UnityEngine;
using UnityEngine.UI;

public class ControlMusica : MonoBehaviour
{
    public AudioSource audioSource;         // Solo un AudioSource en la escena
    public AudioClip[] canciones;           // Aquí sí puedes arrastrar los .mp3
    public Button botonmusica;
    public Slider sliderVolumen;
    public Sprite iconiEncendido;
    public Sprite iconoApagado;

    private bool musicaEncendida = true;
    private AudioClip cancionActual;

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
}
