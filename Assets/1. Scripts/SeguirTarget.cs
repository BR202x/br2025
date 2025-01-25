using UnityEngine;

public class SeguirTarget : MonoBehaviour
{
    public Transform target;
    public Transform salidaChorro;
    private GameObject refInstancia;
    public GameObject objectoPrefab;
    public KeyCode botonAbrir;
    public KeyCode botonCerrar;
    public InstanciaNewChorro chorro;

    void Update()
    {
        if (Input.GetKeyDown(botonAbrir))
        { 
            CrearChorro();
        }

        if (Input.GetKeyDown(botonCerrar))
        {
            DestruirChorro();
        }

        chorro = FindFirstObjectByType<InstanciaNewChorro>();   

        #region Voltear Salida llave al objetivo

        transform.LookAt(target);     
        transform.rotation = Quaternion.LookRotation(target.position - transform.position) * Quaternion.Euler(90f, 0f, 0f);

        #endregion
    }

    public void CrearChorro() // instancia LineRenderer Objeto
    {
        if (refInstancia == null)
        {
            refInstancia = Instantiate(objectoPrefab, salidaChorro.transform.position, Quaternion.identity);
        }
    }
    public void DestruirChorro()
    {
        if (chorro != null)
        {
            chorro.CerrarChorro();         
        }    
    }
}
