using UnityEngine;

public class Salud : MonoBehaviour
{
    public float vidaMaxima = 100f;
    public float vidaActual;
    private Animator animator;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        animator.SetBool("isDead", false);

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    public void NuevaPelea()
    {
        vidaActual = vidaMaxima;
        animator.ResetTrigger("Die");
        animator.SetBool("isWinner", false);

        // 🧠 Fuerza la animación a volver a Idle
        animator.Play("Idle");

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        MovimientoPeleador mover = GetComponent<MovimientoPeleador>();
        if (mover != null)
        {
            mover.enabled = true;
            mover.ReiniciarEstado();
        }

        DatosCombate.nombreGanador = "";
        Debug.Log("Nueva pelea iniciada - Ganador limpiado");
    }


    [System.Obsolete]
    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        if (vidaActual <= 0)
        {
            Perdedor();
        }
    }

    public void Curar(float cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
    }

    [System.Obsolete]
    void Perdedor()
    {
        // DEBUGGING EXTRA
        Debug.Log("=== DEBUGGING PERDEDOR ===");
        Debug.Log($"GameObject que murió: '{gameObject.name}'");
        Debug.Log($"DatosCombate.nombreJugador1: '{DatosCombate.nombreJugador1}'");
        Debug.Log($"DatosCombate.nombreJugador2: '{DatosCombate.nombreJugador2}'");
        Debug.Log($"DatosCombate.nombreGanador actual: '{DatosCombate.nombreGanador}'");

        // DETERMINA EL NOMBRE DEL PERSONAJE PERDEDOR
        string nombrePerdedor = DeterminarNombrePersonaje(gameObject.name);
        Debug.Log($"El personaje {nombrePerdedor} ({gameObject.name}) ha sido derrotado.");


        // BUSCA AL GANADOR SOLO SI AUN NO HAY UNO DEFINIDO
        if (string.IsNullOrEmpty(DatosCombate.nombreGanador))
        {
            GameObject[] personajes = GameObject.FindGameObjectsWithTag("Personaje");
            Debug.Log($"Personajes encontrados con tag 'Personaje': {personajes.Length}");

            foreach (GameObject obj in personajes)
            {
                Debug.Log($"Revisando personaje: '{obj.name}'");
                Salud s = obj.GetComponent<Salud>();

                if (s != null && s != this && s.vidaActual > 0)
                {
                    Debug.Log($"Personaje '{obj.name}' tiene vida: {s.vidaActual}");
                    // USA EL NOMBRE DEL PERSONAJE SELECCIONADO, NO EL GAMEOBJECT
                    string nombreGanador = DeterminarNombrePersonaje(obj.name);
                    DatosCombate.nombreGanador = nombreGanador;
                    Debug.Log($"Ganador detectado: {nombreGanador} ({obj.name})");
                    break;
                }
            }
        }

        Debug.Log("=== FIN DEBUG PERDEDOR ===");

        if (string.IsNullOrEmpty(DatosCombate.nombreGanador))
        {
            Debug.LogWarning("No se encontró un ganador con vida.");
            return;
        }

        // SOLO CAMBIA DE PANTALLA SI HAY UN GANADOR VÁLIDO
        ControlPantalla controlPantalla = FindObjectOfType<ControlPantalla>();
        if (controlPantalla != null)
        {
            controlPantalla.SetGanador(DatosCombate.nombreGanador);
            StartCoroutine(EsperarYCambiar(controlPantalla));
        }
    }

    // MÉTODO PARA DETERMINAR QUE NOMBRE MOSTRAR SEGÚN EL GAMEOBJECT
    string DeterminarNombrePersonaje(string nombreGameObject)
    {
        Debug.Log($"Determinando nombre para GameObject: {nombreGameObject}");
        Debug.Log($"Nombres guardados - Jugador1: '{DatosCombate.nombreJugador1}', Jugador2: '{DatosCombate.nombreJugador2}'");

        string nombreLower = nombreGameObject.ToLower();

        // VERIFICA TODAS LAS VARIANTES POSIBLES PARA PELEADOR 1
        if (nombreLower.Contains("peleador_1") ||    // CON GUIÓN BAJO ← ESTA ES LA CLAVE
            nombreLower.Contains("peleador1") ||
            nombreLower.Contains("peleador 1") ||
            nombreLower.Contains("player_1") ||
            nombreLower.Contains("player1") ||
            nombreLower.Contains("player 1"))
        {
            if (!string.IsNullOrEmpty(DatosCombate.nombreJugador1))
            {
                Debug.Log($"Devolviendo nombre Jugador1: {DatosCombate.nombreJugador1}");
                return DatosCombate.nombreJugador1;
            }
        }
        // VERIFICA TODAS LAS VARIANTES POSIBLES PARA PELEADOR 2
        else if (nombreLower.Contains("peleador_2") ||  // CON GUIÓN BAJO ← ESTA ES LA CLAVE
                 nombreLower.Contains("peleador2") ||
                 nombreLower.Contains("peleador 2") ||
                 nombreLower.Contains("player_2") ||
                 nombreLower.Contains("player2") ||
                 nombreLower.Contains("player 2"))
        {
            if (!string.IsNullOrEmpty(DatosCombate.nombreJugador2))
            {
                Debug.Log($"Devolviendo nombre Jugador2: {DatosCombate.nombreJugador2}");
                return DatosCombate.nombreJugador2;
            }
        }

        // SI NO COINCIDE CON NINGÚN PATRÓN, DEVUELVE EL NOMBRE DEL GAMEOBJECT
        Debug.LogError($"ERROR: No se pudo determinar el personaje para el GameObject: {nombreGameObject}");
        Debug.LogError($"Verifica que el GameObject se llame con alguna variante de 'Peleador_1'/'Peleador_2'");
        Debug.LogError($"Nombres disponibles en DatosCombate - J1: '{DatosCombate.nombreJugador1}', J2: '{DatosCombate.nombreJugador2}'");
        return nombreGameObject;
    }

    private System.Collections.IEnumerator EsperarYCambiar(ControlPantalla controlPantalla)
    {
        yield return new WaitForSeconds(10f); // Espera 10 segundos
        controlPantalla.IrPantalla(5);        // Va a la pantalla 5
    }

}