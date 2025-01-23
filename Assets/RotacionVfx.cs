using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class RotacionVfx : MonoBehaviour
{

    private void Update()
    {
        Quaternion rotacion = transform.rotation;
        VisualEffect vfx = GetComponent<VisualEffect>();
        vfx.SetVector3("Rotacion", rotacion.eulerAngles);
    }
}
