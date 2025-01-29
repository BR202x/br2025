using UnityEngine;
public class TestAudio : MonoBehaviour
{   
    private void Start()
    {        
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            AudioImp.Instance.Reproducir("PlayerSword");
                       
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioImp.Instance.Reproducir("Test2");
        }        
        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.T))
        {

        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            
        }

    }
}
