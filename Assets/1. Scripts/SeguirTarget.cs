using UnityEngine;

public class SeguirTarget : MonoBehaviour
{
    public Transform target;
    public Transform salidaChorro;
    private GameObject refInstancia;
    public GameObject objectoPrefab;
    public KeyCode toggleKey;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        { 
            CrearChorro();
        }

        #region Voltear Salida llave al objetivo

        transform.LookAt(target);     
        transform.rotation = Quaternion.LookRotation(target.position - transform.position) * Quaternion.Euler(90f, 0f, 0f);

        #endregion
    }

    private void CrearChorro() // instancia LineRenderer Objeto
    {
        if (refInstancia == null)
        {
            refInstancia = Instantiate(objectoPrefab, salidaChorro.transform.position, Quaternion.identity);
        }
    }
}
