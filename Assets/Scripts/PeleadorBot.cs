using System.Collections.Generic;
using UnityEngine;

public class PeleadorBot : MonoBehaviour
{
    public Transform objetivo;      // El jugador humano
    public float velocidad = 3f;
    public float distanciaAtaque = 2f;

    [Header("Rangos de tiempo (segundos)")]
    public Vector2 rangoAtaque = new Vector2(2f, 5f);
    public Vector2 rangoCombo  = new Vector2(4f, 8f);

    [Header("Comportamiento de retirada")]
    public float margenSeguridad = 1f;      // Cuánto se aleja tras atacar
    public float probRetiradaBase = 0.3f;   // Probabilidad base de retroceder

    private float intervaloAtaque;
    private float intervaloCombo;
    private float tiempoUltimoAtaque;
    private float tiempoUltimoCombo;
    private float tiempoUltimaAccion;

    private Rigidbody rb;
    private Animator animator;
    private Salud salud;
    private bool yaMurio = false;

    // Mejorar el avance inicial
    private float tiempoAvanceInicial = 2f; // Avanzar por 2 segundos iniciales
    private float tiempoInicioBot;

    private List<string> ataquesDisponibles = new List<string> {
        "GanchoArriba","GolpeBajo","PatadaVoladora","PatadaConSalto","AmaguePatada",
        "AmaguePatadaAvanzada","PatadaCabeza","GanchoDerecho","RodillaCombo","Fireball"
    };
    private List<string> combosDisponibles = new List<string> {
        "ComboPunos","Combo4Punos","GanchoDoble","GolpeCabeza","PatadaAbajo","PatadaLateralAlta"
    };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        salud = GetComponent<Salud>();
        tiempoInicioBot = Time.time;

        intervaloAtaque = Random.Range(rangoAtaque.x, rangoAtaque.y);
        intervaloCombo  = Random.Range(rangoCombo.x,  rangoCombo.y);
        
        // Debug para verificar configuración
        Debug.Log($"Bot iniciado. Objetivo: {(objetivo != null ? objetivo.name : "NULL")}");
        Debug.Log($"Velocidad: {velocidad}, Distancia ataque: {distanciaAtaque}");
    }

    [System.Obsolete]
    void Update()
    {
        // Verificaciones básicas
        if (yaMurio || objetivo == null || salud.vidaActual <= 0) 
        {
            if (objetivo == null) Debug.LogWarning("Bot: Objetivo es NULL!");
            return;
        }

        float now = Time.time;
        float distancia = Vector3.Distance(transform.position, objetivo.position);
        float distSegura = distanciaAtaque + margenSeguridad;

        // Debug de distancia
        if (Time.frameCount % 60 == 0) // Cada segundo aprox
        {
            Debug.Log($"Bot distancia al objetivo: {distancia:F2}");
        }

        // FASE INICIAL: Avance forzado por tiempo determinado
        if (now - tiempoInicioBot < tiempoAvanceInicial)
        {
            MoverHaciaObjetivo();
            return;
        }

        // Dar respiro después de acciones
        if (now - tiempoUltimaAccion < 0.8f) // Aumentado el tiempo de respiro
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("estaCaminando", false);
            return;
        }

        // LÓGICA PRINCIPAL DE MOVIMIENTO
        if (distancia > distSegura)
        {
            // Muy lejos: acercarse
            MoverHaciaObjetivo();
        }
        else if (distancia > distanciaAtaque)
        {
            // En zona intermedia: decidir si retroceder o quedarse
            float saludPct = salud.vidaActual / salud.vidaMaxima;
            float probRetiro = probRetiradaBase + (1f - saludPct) * 0.5f;
            
            if (Random.value < probRetiro)
            {
                RetrocederDelObjetivo();
                tiempoUltimaAccion = now;
            }
            else
            {
                // Quedarse quieto y esperar
                rb.velocity = Vector3.zero;
                animator.SetBool("estaCaminando", false);
                MirarAlObjetivo();
            }
        }
        else
        {
            // DENTRO DE RANGO DE ATAQUE
            rb.velocity = Vector3.zero;
            animator.SetBool("estaCaminando", false);
            MirarAlObjetivo();

            // Intentar combo primero
            if (now - tiempoUltimoCombo > intervaloCombo)
            {
                EjecutarComboAleatorio();
                tiempoUltimoCombo = now;
                intervaloCombo = Random.Range(rangoCombo.x, rangoCombo.y);
                tiempoUltimaAccion = now;
                return;
            }

            // Luego ataque simple
            if (now - tiempoUltimoAtaque > intervaloAtaque)
            {
                AtacarAleatoriamente();
                tiempoUltimoAtaque = now;
                intervaloAtaque = Random.Range(rangoAtaque.x, rangoAtaque.y);
                tiempoUltimaAccion = now;
            }
        }

        // Verificar muerte
        if (!yaMurio && salud.vidaActual <= 0)
        {
            yaMurio = true;
            animator.SetTrigger("Muerto");
            rb.velocity = Vector3.zero;
            animator.SetBool("estaCaminando", false);
            enabled = false;
        }
    }

    [System.Obsolete]
    private void MoverHaciaObjetivo()
    {
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        direccion.y = 0; // Solo movimiento horizontal
        
        // Aplicar movimiento
        rb.velocity = new Vector3(direccion.x * velocidad, rb.velocity.y, direccion.z * velocidad);
        
        // Activar animación de caminar
        animator.SetBool("estaCaminando", true);
        
        // Rotar hacia el objetivo
        MirarAlObjetivo();
        
        Debug.DrawRay(transform.position, direccion * 3f, Color.green, 0.1f);
    }

    [System.Obsolete]
    private void RetrocederDelObjetivo()
    {
        Vector3 direccion = (transform.position - objetivo.position).normalized;
        direccion.y = 0;
        
        rb.velocity = new Vector3(direccion.x * velocidad, rb.velocity.y, direccion.z * velocidad);
        animator.SetBool("estaCaminando", true);
        
        // Seguir mirando al objetivo mientras retrocede
        MirarAlObjetivo();
        
        Debug.DrawRay(transform.position, direccion * 3f, Color.red, 0.1f);
    }

    private void MirarAlObjetivo()
    {
        Vector3 direccionMirada = (objetivo.position - transform.position).normalized;
        direccionMirada.y = 0;
        
        if (direccionMirada != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMirada);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 10f);
        }
    }

    private void AtacarAleatoriamente()
    {
        if (ataquesDisponibles.Count > 0)
        {
            int i = Random.Range(0, ataquesDisponibles.Count);
            string ataque = ataquesDisponibles[i];
            animator.SetTrigger(ataque);
            Debug.Log($"Bot ejecuta ataque: {ataque}");
        }
    }

    private void EjecutarComboAleatorio()
    {
        if (combosDisponibles.Count > 0)
        {
            int i = Random.Range(0, combosDisponibles.Count);
            string combo = combosDisponibles[i];
            animator.SetTrigger(combo);
            Debug.Log($"Bot ejecuta combo: {combo}");
        }
    }

    // Método para reiniciar el bot cuando empiece nueva ronda
    [System.Obsolete]
    public void ReiniciarBot()
    {
        tiempoInicioBot = Time.time;
        tiempoUltimaAccion = 0f;
        tiempoUltimoAtaque = 0f;
        tiempoUltimoCombo = 0f;
        yaMurio = false;
        
        intervaloAtaque = Random.Range(rangoAtaque.x, rangoAtaque.y);
        intervaloCombo = Random.Range(rangoCombo.x, rangoCombo.y);
        
        // Asegurar que el Rigidbody esté completamente detenido
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (animator != null)
        {
            animator.SetBool("estaCaminando", false);
        }
        
        Debug.Log($"Bot reiniciado. Posición actual: {transform.position}");
    }

    [System.Obsolete]
    void OnEnable()
    {
        // Cuando el bot se reactiva, hacer una verificación adicional
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log($"Bot reactivado en posición: {transform.position}");
    }

    // Debug visual en el editor
    void OnDrawGizmosSelected()
    {
        if (objetivo != null)
        {
            // Línea hacia el objetivo
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, objetivo.position);
            
            // Rango de ataque
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
            
            // Distancia segura
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, distanciaAtaque + margenSeguridad);
        }
    }
}