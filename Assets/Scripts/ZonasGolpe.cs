using UnityEngine;

public class ZonasGolpe : MonoBehaviour
{
    public enum Zonacuerpo
    {
        Cabeza, Pecho, Abdomen, BrazoDerecho, BrazoIzquierdo, PiernaDerecha, PiernaIzquierda, PieDerecho, PieIzquierdo
    }

    public Zonacuerpo zona;
    public float multiplicadorDanio = 1f;


}