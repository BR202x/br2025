using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Test1")]
    [field: SerializeField] public EventReference test1 { get; private set; }

    [field: Header("Test2 Pasos ")]
    [field: SerializeField] public EventReference pasosTest1 { get; private set; }    
    [field: SerializeField] public EventReference pasosTest2 { get; private set; }


    [field: Header("Test3 Cercania")]
    [field: SerializeField] public EventReference cercaniaObjeto1 { get; private set; }    
    [field: SerializeField] public EventReference cercaniaObjeto2 { get; private set; }

    [field: Header("Test4 Ambiente")]

    [field: SerializeField] public EventReference ambiente { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Mas de un Evento sucediendo FMOD Events");
        }

        instance = this;
    }


}
