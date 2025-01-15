using UnityEngine;

public class PropsVisualController : MonoBehaviour
{
    [SerializeField] GameObject shieldHand;
    [SerializeField] GameObject shielBack;
    [SerializeField] GameObject needleHand;
    [SerializeField] GameObject needleBack;
    [HideInInspector] PlayerMovement player;
    private float timer;
    [SerializeField] float timeToSheathNeedle;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        //Shield management
        if (player.IsShield() && player.controllerCam.GetCanShield())
        {
            shieldHand.SetActive(true);
            shielBack.SetActive(false);
        }
        else if (!player.controllerCam.GetCanShield())
        {
            shieldHand.SetActive(false);
            shielBack.SetActive(false);
        }
        else if (!player.IsShield())
        {
            shieldHand.SetActive(false);
            shielBack.SetActive(true);

        }
        //Needle management
        if(player.GetCurrentState() == player.stateAttack)
        {
            timer = 0;
            needleHand.SetActive(true);
            needleBack.SetActive(false);
        }
        if (needleHand.activeSelf)
        {
            timer += Time.deltaTime;
            if(timer > timeToSheathNeedle)
            {
                needleHand.SetActive(false);
                needleBack.SetActive(true);

            }
        }

    }
}
