using UnityEngine;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector timelineDirector; // El componente PlayableDirector que controla el Timeline
    public GameObject player;                // El objeto Player
    public GameObject ratonJack;             // El objeto RATON JACK

    // Llama este m�todo para liberar el control del Player y RATON JACK del Timeline
    public void FreeObjectsFromTimeline()
    {
        // Recorre todas las salidas del Timeline
        foreach (var output in timelineDirector.playableAsset.outputs)
        {
            // Obt�n el binding asociado a la pista
            var binding = timelineDirector.GetGenericBinding(output.sourceObject);

            // Si el binding corresponde al Animator del Player, desvinc�lalo
            if (binding == player.GetComponent<Animator>())
            {
                timelineDirector.SetGenericBinding(output.sourceObject, null);
                Debug.Log("Player desvinculado del Timeline");
            }

            // Si el binding corresponde al Animator de RATON JACK, desvinc�lalo
            if (binding == ratonJack.GetComponent<Animator>())
            {
                timelineDirector.SetGenericBinding(output.sourceObject, null);
                Debug.Log("RATON JACK desvinculado del Timeline");
            }
        }
    }

    // Llama este m�todo para devolver el control del Timeline al Player y RATON JACK
    public void RebindObjectsToTimeline()
    {
        // Recorre todas las salidas del Timeline
        foreach (var output in timelineDirector.playableAsset.outputs)
        {
            // Rebind al Animator del Player
            if (output.streamName == "Animation Track (2)") // Verifica por nombre de pista
            {
                timelineDirector.SetGenericBinding(output.sourceObject, player.GetComponent<Animator>());
                Debug.Log("Player vinculado nuevamente al Timeline");
            }

            // Rebind al Animator de RATON JACK
            if (output.streamName == "Animation Track")
            {
                timelineDirector.SetGenericBinding(output.sourceObject, ratonJack.GetComponent<Animator>());
                Debug.Log("RATON JACK vinculado nuevamente al Timeline");
            }
        }
    }
}
