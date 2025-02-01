using UnityEngine;

public class SpiningPunch : MonoBehaviour
{
    [SerializeField] float speedSpining;

    private void Start()
    {
        transform.position = new Vector3(0, 1, 0);

    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * speedSpining * Time.fixedDeltaTime);    
    }
}
