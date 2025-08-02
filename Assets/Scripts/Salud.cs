using UnityEngine;

public class Salud : MonoBehaviour
{
    public float vidaMaxima = 100f;
    [HideInInspector] public float vidaActual;
    private Animator animator;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();

        // Guarda posición y rotación iniciales
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }


    /// Prepara al personaje para una nueva ronda: restaura vida, posición,
    /// giro, animación y movement script.

    public void NuevaPelea()
    {
        vidaActual = vidaMaxima;

        // Reinicia animaciones de muerte/victoria
        animator.ResetTrigger("Muerto");
        animator.SetBool("esGanador", false);
        animator.Play("Boxing", 0, 0f); // Asegúrate que "Idle" sea tu estado base

        // Vuelve a la posición y rotación inicial
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        // Reactiva el script de movimiento
        var mover = GetComponent<MovimientoPeleador>();
        if (mover != null)
            mover.enabled = true;
    }


    /// Aplica daño y, si la vida llega a 0, dispara el KO.

    [System.Obsolete]
    public void RecibirDanio(float cantidad)
    {
        vidaActual = Mathf.Max(vidaActual - cantidad, 0f);
        if (vidaActual <= 0f)
            Perdedor();
    }


    /// Lógica que se ejecuta al quedar KO: anima, desactiva movimiento y notifica al ControlRondas.

    [System.Obsolete]
    private void Perdedor()
    {
        // Dispara animación de muerte
        animator.SetTrigger("Die");

        // Desactiva este componente para parar movimiento/inputs
        enabled = false;

        // Notifica al controlador de rondas
        var controlRondas = FindObjectOfType<ControlRondas>();
        if (controlRondas != null)
            controlRondas.NotificarKO(gameObject);
    }
}
