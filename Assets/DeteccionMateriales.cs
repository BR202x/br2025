using UnityEngine;

public class DeteccionMateriales : MonoBehaviour
{    
    void Start()
    {
        
    }
     
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agua"))
        {
            Debug.Log("TOCANDO AGUA");
        }

        if (collision.gameObject.CompareTag("Metal"))
        {
            Debug.Log("TOCANDO METAL");
        }

        if (collision.gameObject.CompareTag("Ropa"))
        {
            Debug.Log("TOCANDO ROPA");
        }

        if (collision.gameObject.CompareTag("RopaMojada"))
        {
            Debug.Log("TOCANDO ROPA MOJADA");
        }
    }
}
