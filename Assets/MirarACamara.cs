using UnityEngine;

public class MirarACamara : MonoBehaviour
{
    private Camera mainCamera;
    public bool mirarCamara;
    void Start()
    {

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mirarCamara)
        {
            // Haz que este objeto siempre mire hacia la cámara
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void DesactivarPuntero()
    {
        this.gameObject.SetActive(false);
    }
}
