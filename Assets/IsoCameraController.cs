using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class IsoCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    public float rotationSpeed = 10f; // Speed of rotation, adjustable from Inspector
    public float velocidadZoom = 1;

    private float minOrthoSize = 4f;
    private float maxOrthoSize = 16f;
    // private bool isRotating;    
    private bool isRotationEnabled = false;    
    private Vector3 lastMousePosition;

    void Update()
    {
        Cursor.visible = true; // Ensure the mouse cursor is always visible
        Cursor.lockState = CursorLockMode.None; // Ensure the cursor is not locked

        HandleZoom();
        HandleRotation();
    }


    private void HandleZoom()
    {
        if (cinemachineCamera != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                CinemachineComponentBase lensComponent = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                if (cinemachineCamera.m_Lens.Orthographic)
                {
                    cinemachineCamera.m_Lens.OrthographicSize = Mathf.Clamp(
                        cinemachineCamera.m_Lens.OrthographicSize - scrollInput * velocidadZoom,
                        minOrthoSize,
                        maxOrthoSize
                    );
                }
            }
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Enable rotation mode
        {
            isRotationEnabled = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl)) // Disable rotation mode
        {
            isRotationEnabled = false;
        }

        if (isRotationEnabled)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaMousePosition = currentMousePosition - lastMousePosition;

            if (deltaMousePosition.sqrMagnitude > Mathf.Epsilon) // Detect if the mouse is moving
            {
                //isRotating = true;

                // Adjust rotation based on mouse movement direction
                float rotationDelta = deltaMousePosition.x * rotationSpeed * Time.deltaTime;
                Vector3 currentRotation = cinemachineCamera.gameObject.transform.rotation.eulerAngles;
                cinemachineCamera.gameObject.transform.rotation = Quaternion.Euler(
                    currentRotation.x,
                    currentRotation.y + rotationDelta,
                    currentRotation.z
                );

                lastMousePosition = currentMousePosition;
            }
            else
            {
                // isRotating = false; // Mouse is not moving
            }
        }
    }
}
