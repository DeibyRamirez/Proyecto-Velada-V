using System.Collections.Generic;
using UnityEngine;

public class HitBoxPuno : MonoBehaviour
{
    public float danio = 10f;
    public string[] animacionesAtaque = {
        "Golpe Gancho Arriba","GanchoArriba","Golpe Bajo","GolpeBajo",
        "Patada Voladora Combo","PatadaVoladora","Patada con Salto","PatadaConSalto",
        "Amague y Patada","AmaguePatada","Amague y Patada Avanzada","AmaguePatadaAvanzada",
        "Patada a la Cabeza","PatadaCabeza","Gancho Derecho","GanchoDerecho",
        "Golpes de Rodilla Combo","RodillaCombo","Fireball"
    };

    public Animator animator;     // animator del atacante
    private bool yaGolpeo = false;

    void Update()
    {
        if (animator == null) return;
        var state = animator.GetCurrentAnimatorStateInfo(0);
        if (!EsAnimacionAtaque(state) || state.normalizedTime >= 1f)
            yaGolpeo = false;
    }

    [System.Obsolete]
    void OnTriggerEnter(Collider other)
    {
        if (yaGolpeo || animator == null) return;

        var state = animator.GetCurrentAnimatorStateInfo(0);
        if (!EsAnimacionAtaque(state)) return;

        // ¿Es zona válida?
        var zona = other.GetComponent<ZonasGolpe>();
        if (zona == null) return;

        // Obtenemos Salud y Animator del receptor
        var salud = other.GetComponentInParent<Salud>();
        var animVictima = other.GetComponentInParent<Animator>();
        if (salud == null || animVictima == null) return;

        // ** Aquí comprobamos bloqueo **
        if (animVictima.GetBool("estaBloqueando"))
        {
            // opcional: animVictima.SetTrigger("ReciveBloqueo");
            return;
        }

        // Aplicamos daño
        float danioFinal = danio * zona.multiplicadorDanio;
        salud.RecibirDanio(danioFinal);

        // disparar la animación de recibir golpe según zona...
        ReproducirAnimacionGolpe(animVictima, zona.zona);

        yaGolpeo = true;
    }

    private bool EsAnimacionAtaque(AnimatorStateInfo state)
    {
        foreach (string a in animacionesAtaque)
            if (state.IsName(a))
                return true;
        return false;
    }

    void ReproducirAnimacionGolpe(Animator anim, ZonasGolpe.Zonacuerpo zona)
    {
        switch (zona)
        {
            case ZonasGolpe.Zonacuerpo.Cabeza:
                anim.SetTrigger("ReciveGolpeCara"); break;
            case ZonasGolpe.Zonacuerpo.Abdomen:
                anim.SetTrigger("ReciveGolpeAbdomen"); break;
            default:
                anim.SetTrigger("ReciveGolpeCuerpo"); break;
        }
    }
}
