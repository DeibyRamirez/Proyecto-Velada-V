using UnityEngine;

using TMPro;
public class Consejos : MonoBehaviour
{ 
    public TextMeshProUGUI[] textoConsejo;

    public float intervaloEspera = 7f;

    private float tiempoRestante;

    public string[] listaConsejos = {
        "Recuerda esquivar los ataques especiales de tu oponente.",
        "Utiliza el entorno a tu favor para ganar ventaja.",
        "No olvides usar tus habilidades especiales en el momento adecuado.",
        "Mantén la calma y observa los movimientos de tu rival.",
        "Practica tus combos para maximizar el daño infligido.",
        "Aprovecha las debilidades de tu oponente para ganar la pelea.",
        "La paciencia es clave, no te precipites en atacar.",
        "Estudia los patrones de ataque de tu enemigo para anticiparte.",
        "La defensa es tan importante como el ataque, no la descuides.",
        "Recuerda que cada personaje tiene sus propias fortalezas y debilidades.",
        "Usa el tiempo de carga de tus habilidades para planear tu próximo movimiento.",
     };


    void Start()
    {
        tiempoRestante = intervaloEspera;
        MostrarConsejoAleatorio();
    }


    void Update()
    {
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            MostrarConsejoAleatorio();
            tiempoRestante = intervaloEspera;
        }
    }

    public void MostrarConsejoAleatorio()
    {
        if (listaConsejos.Length == 0) return;

        for (int i = 0; i < textoConsejo.Length; i++)
        {
            int index = Random.Range(0, listaConsejos.Length);
            textoConsejo[i].text = listaConsejos[index];
        }
    }
}