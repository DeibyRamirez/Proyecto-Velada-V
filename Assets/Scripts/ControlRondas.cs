using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ControlRondas : MonoBehaviour
{
    [Header("Configuración de Rondas")]
    public int totalRondas = 3;
    public float duracionRound = 60f;

    [Header("UI")]
    public TextMeshProUGUI textoReloj;
    public TextMeshProUGUI textoRondas;

    [Header("Audio")]
    public ControlMusica controlMusica;

    private float tiempoRestante;
    private int rondaActual = 1;

    private int rondasGanadasJugador1 = 0;
    private int rondasGanadasJugador2 = 0;

    private bool enPelea = false;

    private Salud saludJugador1;
    private Salud saludJugador2;

    private Vector3 posicionInicialJugador1;
    private Vector3 posicionInicialJugador2;

    private Quaternion rotacionInicialJugador1;
    private Quaternion rotacionInicialJugador2;

    public void IniciarCombateDesdeBoton()
    {
        DatosCombate.nombreGanador = "";
        StartCoroutine(EsperarYPoblarPeleadores());
    }

    private IEnumerator EsperarYPoblarPeleadores()
    {
        GameObject[] personajes = null;
        while (personajes == null || personajes.Length < 2)
        {
            personajes = GameObject.FindGameObjectsWithTag("Personaje");
            yield return null;
        }

        // Según tu orden: índice 0 → peleador controlado por CPU, índice 1 → peleador controlado por jugador
        GameObject cpuGO = personajes[0];
        GameObject jugadorGO = personajes[1];

        // 1) Deshabilita el límite en el bot
        var limitBot = cpuGO.GetComponent<LimiteMovimiento>();
        if (limitBot != null) limitBot.enabled = false;

        saludJugador2 = cpuGO.GetComponent<Salud>(); // CPU es jugador 2
        saludJugador1 = jugadorGO.GetComponent<Salud>(); // Humano es jugador 1

        var mpHumano = jugadorGO.GetComponent<MovimientoPeleador>();
        var mpCPU = cpuGO.GetComponent<MovimientoPeleador>();
        var botCPU = cpuGO.GetComponent<PeleadorBot>();

        if (DatosCombate.tipoPelea == TipoPelea.JugadorVsJugador)
        {
            // Ambos controlados por jugador
            mpHumano.ConfigurarJugador(true);
            mpCPU.ConfigurarJugador(true);
            if (botCPU) botCPU.enabled = false;
        }
        else // Jugador vs CPU
        {
            // Sólo humano recibe input
            mpHumano.enabled = true;
            mpCPU.enabled = false;

            if (botCPU)
            {
                botCPU.enabled = true;
                botCPU.objetivo = jugadorGO.transform; // CPU persigue al humano
            }
        }

        // Guardar posiciones/rotaciones iniciales
        posicionInicialJugador1 = jugadorGO.transform.position;
        posicionInicialJugador2 = cpuGO.transform.position;
        rotacionInicialJugador1 = jugadorGO.transform.rotation;
        rotacionInicialJugador2 = cpuGO.transform.rotation;

        IniciarRonda();
    }

    // Agregar esta parte al método IniciarRonda() en ControlRondas

    // ✅ SOLUCIÓN: Cambiar el orden
    private void IniciarRonda()
    {
        tiempoRestante = duracionRound;
        enPelea = true;

        // 1. PRIMERO reiniciar el bot (mientras está en su posición anterior)
        var botCPU = saludJugador2.GetComponent<PeleadorBot>();
        if (botCPU != null)
        {
            botCPU.enabled = false; // Desactivar temporalmente
        }

        // 2. LUEGO mover las posiciones
        saludJugador1.gameObject.transform.position = posicionInicialJugador1;
        saludJugador2.gameObject.transform.position = posicionInicialJugador2;
        saludJugador1.gameObject.transform.rotation = rotacionInicialJugador1;
        saludJugador2.gameObject.transform.rotation = rotacionInicialJugador2;

        // 3. Restaurar salud y animaciones
        saludJugador1.NuevaPelea();
        saludJugador2.NuevaPelea();

        var anim1 = saludJugador1.GetComponent<Animator>();
        var anim2 = saludJugador2.GetComponent<Animator>();
        if (anim1) anim1.Play("Boxing", 0, 0f);
        if (anim2) anim2.Play("Boxing", 0, 0f);

        // 4. FINALMENTE reactivar el bot con las nuevas posiciones
        if (botCPU != null && DatosCombate.tipoPelea != TipoPelea.JugadorVsJugador)
        {
            // Esperar un frame para que las posiciones se asienten
            StartCoroutine(ReactivarBotDespuesDeReposicionar(botCPU));
        }

        if (controlMusica != null)
            controlMusica.ReproducirCampanaPorRonda(rondaActual);

        ActualizarUI();
    }

    private IEnumerator ReactivarBotDespuesDeReposicionar(PeleadorBot botCPU)
    {
        yield return new WaitForFixedUpdate();

        botCPU.ReiniciarBot();
        botCPU.enabled = true;

        Debug.Log($"Bot reactivado - Posición bot: {botCPU.transform.position}, Posición objetivo: {botCPU.objetivo.position}");
    }


    [System.Obsolete]
    private void Update()
    {
        if (!enPelea) return;

        tiempoRestante -= Time.deltaTime;
        ActualizarUI();

        if (tiempoRestante <= 0f)
            DeterminarGanadorPorTiempo();
    }

    [System.Obsolete]
    public void NotificarKO(GameObject perdedor)
    {
        if (!enPelea) return;

        // Si el que cayó es el humano (jugador1), gana CPU (jugador2)
        if (perdedor == saludJugador1.gameObject)
            rondasGanadasJugador2++;
        else
            rondasGanadasJugador1++;

        TerminarRonda();
    }

    [System.Obsolete]
    private void DeterminarGanadorPorTiempo()
    {
        enPelea = false;
        if (saludJugador1.vidaActual > saludJugador2.vidaActual)
            rondasGanadasJugador1++;
        else if (saludJugador2.vidaActual > saludJugador1.vidaActual)
            rondasGanadasJugador2++;
        TerminarRonda();
    }

    [System.Obsolete]
    private void TerminarRonda()
    {
        enPelea = false;
        if (rondaActual >= totalRondas)
        {
            // Decide al final
            string ganador = "Empate";
            if (rondasGanadasJugador1 > rondasGanadasJugador2) ganador = DatosCombate.nombreJugador1;
            else if (rondasGanadasJugador2 > rondasGanadasJugador1) ganador = DatosCombate.nombreJugador2;
            MostrarGanadorFinal(ganador);
        }
        else
        {
            rondaActual++;
            StartCoroutine(EsperarYComenzarSiguienteRonda());
        }
    }

    private IEnumerator EsperarYComenzarSiguienteRonda()
    {
        yield return new WaitForSeconds(5f);
        IniciarRonda();
    }

    [System.Obsolete]
    private void MostrarGanadorFinal(string ganador)
    {
        DatosCombate.nombreGanador = ganador;
        var pantalla = FindObjectOfType<ControlPantalla>();
        if (pantalla != null)
        {
            pantalla.SetGanador(ganador);
            pantalla.IrPantalla(5);
        }
    }

    private void ActualizarUI()
    {
        if (textoReloj != null)
        {
            int min = Mathf.FloorToInt(tiempoRestante / 60f);
            int sec = Mathf.FloorToInt(tiempoRestante % 60f);
            textoReloj.text = $"{min:00}:{sec:00}";
        }
        if (textoRondas != null)
        {
            textoRondas.text = $"Ronda {rondaActual}/{totalRondas}";
        }
    }

    public void ReiniciarRondas()
    {
        rondaActual = 1;
        rondasGanadasJugador1 = 0;
        rondasGanadasJugador2 = 0;
        enPelea = false;
        ActualizarUI();
    }
}