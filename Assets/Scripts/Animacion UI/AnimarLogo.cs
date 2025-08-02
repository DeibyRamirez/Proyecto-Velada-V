using UnityEngine;
using DG.Tweening;

public class AnimarLogo : MonoBehaviour
{
    public float escalaMaxima = 1.1f;    // Tamaño máximo
    public float duracion = 1f;          // Duración de cada fase (agrandar y reducir)

    void Start()
    {
        // Escalado suave infinito
        transform.DOScale(escalaMaxima, duracion)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }
}
