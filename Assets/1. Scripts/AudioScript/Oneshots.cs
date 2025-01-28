using UnityEngine;
using FMODUnity;

public class Oneshots : MonoBehaviour
{
    [Header("FMOD Event")]

public EventReference Attack;
public EventReference Defense;
public EventReference UnDefense;
public EventReference ThrowShield;
public EventReference StepsCloth;
public EventReference StepsWetCloth;
public EventReference StepsMetal;
public EventReference StepsWater;
public EventReference RollWater;
public EventReference RollCloth;
public EventReference RollWetCloth;
public EventReference RollMetal;
public EventReference JumpWater;
public EventReference JumpCloth;
public EventReference JumpWetCloth;
public EventReference JumpMetal;
public EventReference ChorroOn;
public EventReference CallChorro;

    public EventReference Event; // Aqui toca cambiarlo, cambia 'string' por 'EventReference'
    public bool PlayOnAwake;

    public void PlayOneShot(EventReference Event,GameObject gameObject)
    {
        RuntimeManager.PlayOneShotAttached(Event, gameObject);      
    }

    private void Start()
    {
        if (PlayOnAwake)
        {
            PlayOneShot(Event, gameObject);
        }
    }
}