using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ControlRondas : MonoBehaviour
{
    public int totalRondas = 3;
    public float duracionRound = 60f;

    public TextMeshProUGUI textoReloj;    // Asignar desde el Inspector
    public TextMeshProUGUI textoRondas;   // Asignar desde el Inspector

    private float tiempoRestante;
    private int rondaActual = 1;

    private int rondasGanadasJugador1 = 0;
    private int rondasGanadasJugador2 = 0;

    private bool enPelea = false;

    private Salud saludJugador1;
    private Salud saludJugador2;

    // 👉 LLAMAR ESTE MÉTODO DESDE EL BOTÓN
    public void IniciarCombateDesdeBoton()
    {
        StartCoroutine(EsperarYPoblarPeleadores());
    }

    IEnumerator EsperarYPoblarPeleadores()
    {
        Debug.Log("Esperando a que los peleadores se carguen...");
        GameObject[] personajes = null;

        while (personajes == null || personajes.Length < 2)
        {
            personajes = GameObject.FindGameObjectsWithTag("Personaje");
            yield return null;
        }

        saludJugador1 = personajes[0].GetComponent<Salud>();
        saludJugador2 = personajes[1].GetComponent<Salud>();

        Debug.Log("¡Peleadores encontrados! Iniciando primera ronda...");
        IniciarRonda();
    }

    void IniciarRonda()
    {
        tiempoRestante = duracionRound;
        enPelea = true;

        saludJugador1.NuevaPelea();
        saludJugador2.NuevaPelea();

        ActualizarUI();
    }

    void Update()
    {
        if (!enPelea) return;

        tiempoRestante -= Time.deltaTime;
        ActualizarUI();

        if (tiempoRestante <= 0)
        {
            DeterminarGanadorPorTiempo();
        }

        if (saludJugador1.vidaActual <= 0 || saludJugador2.vidaActual <= 0)
        {
            DeterminarGanadorPorKO();
        }
    }

    void DeterminarGanadorPorTiempo()
    {
        Debug.Log("Fin de ronda por tiempo.");

        if (saludJugador1.vidaActual > saludJugador2.vidaActual)
        {
            rondasGanadasJugador1++;
        }
        else if (saludJugador2.vidaActual > saludJugador1.vidaActual)
        {
            rondasGanadasJugador2++;
        }
        else
        {
            Debug.Log("Empate en la ronda.");
        }

        TerminarRonda();
    }

    void DeterminarGanadorPorKO()
    {
        Debug.Log("Fin de ronda por KO.");

        if (saludJugador1.vidaActual <= 0)
        {
            rondasGanadasJugador2++;
        }
        else if (saludJugador2.vidaActual <= 0)
        {
            rondasGanadasJugador1++;
        }

        TerminarRonda();
    }

    void TerminarRonda()
    {
        enPelea = false;

        if (rondaActual < totalRondas)
        {
            rondaActual++;
            StartCoroutine(EsperarYComenzarSiguienteRonda());
        }
        else
        {
            MostrarGanadorFinal();
        }
    }

    IEnumerator EsperarYComenzarSiguienteRonda()
    {
        yield return new WaitForSeconds(3f);
        IniciarRonda();
    }

    void MostrarGanadorFinal()
    {
        string ganadorFinal = "Empate";

        if (rondasGanadasJugador1 > rondasGanadasJugador2)
        {
            ganadorFinal = DatosCombate.nombreJugador1;
        }
        else if (rondasGanadasJugador2 > rondasGanadasJugador1)
        {
            ganadorFinal = DatosCombate.nombreJugador2;
        }

        Debug.Log($"GANADOR FINAL: {ganadorFinal}");

        ControlPantalla pantalla = FindObjectOfType<ControlPantalla>();
        if (pantalla != null)
        {
            pantalla.SetGanador(ganadorFinal);
            pantalla.IrPantalla(5);
        }
    }

    void ActualizarUI()
    {
        if (textoReloj != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
            textoReloj.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        if (textoRondas != null)
        {
            textoRondas.text = $"Ronda {rondaActual}/3\n" +
                               $"{DatosCombate.nombreJugador1}: {rondasGanadasJugador1} | " +
                               $"{DatosCombate.nombreJugador2}: {rondasGanadasJugador2}";
        }
    }


    public void ReiniciarRondas()
    {
        Debug.Log("Reiniciando sistema de rondas...");

        rondaActual = 1;
        rondasGanadasJugador1 = 0;
        rondasGanadasJugador2 = 0;
        tiempoRestante = duracionRound;
        enPelea = true;

        if (saludJugador1 != null) saludJugador1.NuevaPelea();
        if (saludJugador2 != null) saludJugador2.NuevaPelea();

        ActualizarUI();
    }

}
