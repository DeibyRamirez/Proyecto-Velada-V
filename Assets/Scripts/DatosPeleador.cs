using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPeleador", menuName = "Pelea/DatosPeleador")]
public class DatosPeleador : ScriptableObject
{
    public string nombre;
    public Sprite imagen;

    public AudioClip vozPresentacion;
    //public GameObject prefab3D;

}
