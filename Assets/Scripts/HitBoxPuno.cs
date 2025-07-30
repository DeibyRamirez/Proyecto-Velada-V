using UnityEngine;

public class HitBoxPuno : MonoBehaviour
{
    public float danio = 10f;
    public string[] animacionesAtaque = {"Golpe Gancho Arriba", "GanchoArriba", "Golpe Bajo", "GolpeBajo", "Patada Voladora Combo", "PatadaVoladora", "Patada con Salto", "PatadaConSalto", "Amague y Patada", "AmaguePatada", "Amague y Patada Avanzada", "AmaguePatadaAvanzada", "Patada a la Cabeza", "PatadaCabeza", "Gancho Derecho", "GanchoDerecho", "Golpes de Rodilla Combo", "RodillaCombo", "Fireball"}; // Asegúrate que sea exacto

    public Animator animator;
    private bool yaGolpeo = false;

    void Update()
    {
        if (animator != null)
        {
            AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);

            // Si la animación termina, se resetea el golpe
            if (estado.normalizedTime >= 1f || !EsAnimacionAtaque(estado))
            {
                yaGolpeo = false;
            }
        }
    }

    [System.Obsolete]
    void OnTriggerEnter(Collider other)
    {
        if (yaGolpeo || animator == null) return;

        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);

        if (EsAnimacionAtaque(estado))
        {
            ZonasGolpe zona = other.GetComponent<ZonasGolpe>();
            if (zona != null)
            {
                Salud salud = other.GetComponentInParent<Salud>();
                if (salud != null)
                {
                    float danioFinal = danio * zona.multiplicadorDanio;
                    salud.RecibirDanio(danioFinal);
                    Debug.Log($"✅ Golpe: {estado.fullPathHash} → daño: {danioFinal}");
                    yaGolpeo = true;
                }
            }
        }
    }

    private bool EsAnimacionAtaque(AnimatorStateInfo estado)
    {
        foreach (string anim in animacionesAtaque)
        {
            if (estado.IsName(anim))
                return true;
        }
        return false;
    }
}
