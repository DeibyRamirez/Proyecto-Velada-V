using UnityEngine;

public class HitBoxPuno : MonoBehaviour
{
    public float danio = 10f;
    public string nombreAnimacionAtaque = "Boxing_1"; // Asegúrate que sea exacto
    public Animator animator;

    private bool yaGolpeo = false;

    void Update()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Si ya pasó el golpe, pero la animación ya no está activa, reinicia la bandera
            if (!stateInfo.IsName(nombreAnimacionAtaque))
            {
                yaGolpeo = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(nombreAnimacionAtaque) && !yaGolpeo)
            {
                Salud salud = other.GetComponentInParent<Salud>();
                if (salud != null)
                {
                    salud.RecibirDanio(danio);
                    Debug.Log($"Golpe a {other.name}, daño infligido: {danio}");
                    yaGolpeo = true; // Evita más golpes durante la misma animación
                }
            }
        }
    }
}
