using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "DatosMapa", menuName = "Mapa/DatosMapa")]
public class DatosMapa : ScriptableObject
{
    [Header("Información del Mapa")]
    public string nombre;
    public Sprite imagenMapa;
    public GameObject prefabMapa;
    public VideoClip videoMapa;
   
}
