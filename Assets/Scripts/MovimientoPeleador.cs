using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoPeleador : MonoBehaviour
{
    public enum PlayerNumber { Uno, Dos }
    [Tooltip("Elige si este componente es para el Jugador Uno o Dos")]
    public PlayerNumber jugador = PlayerNumber.Uno;

    public float velocidad = 5f;
    public float fuerzasalto = 7f;
    public Transform oponente;

    // Estado interno
    private Rigidbody rb;
    private Animator animator;
    private bool enSuelo = true;
    private bool yaMurio = false;
    private Salud salud;

    // Movimiento
    private Vector2 movimiento;
    private bool saltoPresionado;

    // Input System
    private Movimientos_Peleador controles;
    private InputActionMap mapa;

    // Buffer de botones para combos
    private List<string> bufferBotones = new List<string>();
    private float tiempoMaxCombo = 0.5f;
    private float tiempoCombo = 0f;

    void Awake()
    {
        controles = new Movimientos_Peleador();
        mapa = jugador == PlayerNumber.Uno ? controles.Jugador1 : controles.Jugador2;

        // — MOVIMIENTO Y SALTO —
        mapa["Mover"].performed += ctx => movimiento = ctx.ReadValue<Vector2>();
        mapa["Mover"].canceled  += ctx => movimiento = Vector2.zero;
        mapa["Saltar"].performed += ctx => saltoPresionado = true;

        // — ATAQUES SIMPLES (Triggers) —
        AsignarTrigger("Golpe Gancho Arriba",    "GanchoArriba");
        AsignarTrigger("Golpe Bajo",             "GolpeBajo");
        AsignarTrigger("Patada Voladora Combo",  "PatadaVoladora");
        AsignarTrigger("Patada con Salto",       "PatadaConSalto");
        AsignarTrigger("Amague y Patada",        "AmaguePatada");
        AsignarTrigger("Amague y Patada Avanzada","AmaguePatadaAvanzada");
        AsignarTrigger("Patada a la Cabeza",     "PatadaCabeza");
        AsignarTrigger("Gancho Derecho",         "GanchoDerecho");
        AsignarTrigger("Golpes de Rodilla Combo","RodillaCombo");
        AsignarTrigger("Fireball",               "Fireball");

        // — BLOQUEO (Bool) —
        mapa["Bloquear"].performed += ctx => animator.SetBool("estaBloqueando", true);
        mapa["Bloquear"].canceled  += ctx => animator.SetBool("estaBloqueando", false);

        // — COMBOS: también registran en buffer —
        mapa["Golpe Gancho Arriba"].performed      += ctx => RegistrarBoton("C");
        mapa["Golpe Bajo"].performed               += ctx => RegistrarBoton("V");
        mapa["Patada a la Cabeza"].performed       += ctx => RegistrarBoton("X");
        mapa["Patada con Salto"].performed         += ctx => RegistrarBoton("J");
        mapa["Amague y Patada Avanzada"].performed += ctx => RegistrarBoton("K");
        mapa["Patada Voladora Combo"].performed    += ctx => RegistrarBoton("U");
    }

    void AsignarTrigger(string accion, string trigger)
    {
        if (mapa[accion] != null)
            mapa[accion].performed += ctx => animator.SetTrigger(trigger);
    }

    private void RegistrarBoton(string codigo)
    {
        bufferBotones.Add(codigo);
        tiempoCombo = tiempoMaxCombo;
        if (bufferBotones.Count > 3)
            bufferBotones.RemoveAt(0);
        VerificarCombo();
    }

    private void VerificarCombo()
    {
        string combo = string.Join("-", bufferBotones);
        switch (combo)
        {
            case "C-K":
                animator.SetTrigger("ComboPunos");
                bufferBotones.Clear();
                break;
            case "C-V":
                animator.SetTrigger("Combo4Punos");
                bufferBotones.Clear();
                break;
            case "V-V":
                animator.SetTrigger("GanchoDoble");
                bufferBotones.Clear();
                break;
            case "X-V":
                animator.SetTrigger("GolpeCabeza");
                bufferBotones.Clear();
                break;
            case "X-J":
                animator.SetTrigger("PatadaAbajo");
                bufferBotones.Clear();
                break;
            case "X-U":
                animator.SetTrigger("PatadaLateralAlta");
                bufferBotones.Clear();
                break;
        }
    }

    void OnEnable()  => mapa.Enable();
    void OnDisable() => mapa.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        salud = GetComponent<Salud>();
        animator.SetBool("esGanador", false);
    }

    [System.Obsolete]
    void Update()
    {
        // Limpieza de buffer si expira timer
        if (tiempoCombo > 0f)
        {
            tiempoCombo -= Time.deltaTime;
            if (tiempoCombo <= 0f)
                bufferBotones.Clear();
        }

        // Root Motion vs movimiento manual
        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);
        if (estado.IsTag("RootMotion"))
        {
            animator.applyRootMotion = true;
            animator.SetBool("estaCaminando", false);
        }
        else
        {
            animator.applyRootMotion = false;

            // Movimiento en eje lateral / adelante-atrás
            Vector3 mov3D = Vector3.zero;
            if (oponente != null)
            {
                Vector3 frente = oponente.position - transform.position;
                frente.y = 0; frente.Normalize();
                Vector3 derechaRel = Vector3.Cross(Vector3.up, frente).normalized;

                mov3D += derechaRel * movimiento.x;
                mov3D += frente     * movimiento.y;

                if (frente != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(frente), transform.rotation, Time.deltaTime * 10f);
            }

            rb.velocity = new Vector3(mov3D.x * velocidad, rb.velocity.y, mov3D.z * velocidad);
            animator.SetBool("estaCaminando", movimiento != Vector2.zero);

            if (saltoPresionado && enSuelo)
            {
                rb.AddForce(Vector3.up * fuerzasalto, ForceMode.Impulse);
                enSuelo = false;
            }
        }

        saltoPresionado = false;

        // Verificar muerte
        if (!yaMurio && salud.vidaActual <= 0)
        {
            yaMurio = true;
            animator.SetTrigger("Muerto");
            enabled = false;
            foreach (var p in GameObject.FindGameObjectsWithTag("Personaje"))
            {
                if (p != gameObject)
                {
                    var s = p.GetComponent<Salud>();
                    if (s != null && s.vidaActual > 0)
                        p.GetComponent<Animator>().SetBool("esGanador", true);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Suelo"))
            enSuelo = true;
    }

    public void ReiniciarEstado()
    {
        yaMurio = false;
        enSuelo = true;
        animator.SetBool("estaCaminando", false);
        animator.SetBool("estaBloqueando", false);
        animator.SetBool("esGanador", false);
        animator.ResetTrigger("Muerto");
    }
}
