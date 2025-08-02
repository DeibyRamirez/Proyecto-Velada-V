using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BotonAnimado : MonoBehaviour
{
    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(() => AnimarBoton());
    }

    void AnimarBoton()
    {
        // Rebote del botón (punch scale)
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1);
    }
}
