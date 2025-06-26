using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

public class ControlVideo : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    public VideoClip[] videos;

    public void ActualizarVideo(int index)
    {
        
        if (index == -1)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null; // Detiene el video y lo establece como nulo
            return; // Detiene el video si el índice es 8
        } 

        if (index < 0 || index > videos.Length) return;
        videoPlayer.Stop();
        videoPlayer.clip = videos[index];
        videoPlayer.Play();
    }
}
