using UnityEngine;
using UnityEngine.UI;

public class ControlMusica : MonoBehaviour
{
    public AudioSource musica;
    public Button botonmusica;
    public Sprite iconiEncendido;
    public Sprite iconoApagado;

    private bool musicaEncendida = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        botonmusica.onClick.AddListener(ToogleMusica);
        ActualizarIcono();
    }

    public void ToogleMusica()
    {
        musicaEncendida = !musicaEncendida;


        if (musicaEncendida)
        {
            musica.Play();
        }
        else
        {
            musica.Pause();
        }
        ActualizarIcono();
    }

    public void ActualizarIcono()
    {
        Image imagenBoton = botonmusica.GetComponent<Image>();
        if(imagenBoton != null)
            imagenBoton.sprite = musicaEncendida ? iconiEncendido : iconoApagado;
        else
            Debug.LogWarning("El componente Image no se encontró en el botón de música.");
    }
    
}
