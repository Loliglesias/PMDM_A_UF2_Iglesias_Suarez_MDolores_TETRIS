using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

/*
Controlador da animación creada cun timeline. 
No momento no que remata de reproducirse, pasa ao menú principal
*/

public class IntroAnimationController : MonoBehaviour
{
    public string mainMenuSceneName = "01_Intro"; 
    public PlayableDirector introTimeline; // Reproductor do timeline a partir dun compoñente PlayableDirector

    void Start()
    {
        if (introTimeline != null)
        {
            introTimeline.stopped += OnIntroAnimationFinished;
        }
        else
        {
            Debug.LogError("Non hai timeline asignado.");
        }
    }

    /*
    Ao rematar a animación, pasa ao menú principal
    */
    void OnIntroAnimationFinished(PlayableDirector director)
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
