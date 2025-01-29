using Cinemachine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    [Header("Propiedades de camara")]
    [SerializeField] GameObject followTransform;
    [SerializeField, Range(0.1f, 1f)] float sensivity;
    [SerializeField] float maxClamp;
    [SerializeField] float minClamp;
    [Header("Apuntado")]
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject aimCamera;
    [SerializeField] GameObject crossAir;
    [SerializeField] LayerMask colliderMask;
    [Header("Disparo")]
    [SerializeField] Transform spawnProjectile;
    [SerializeField] GameObject projectile;
    [SerializeField] bool shotShield;
    private InputReader input;
    private PlayerMovement player;
    private Vector3 angles;
    private Vector3 mouseWorldPosition;
    private CinemachineImpulseSource cameraShake;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        player = GetComponent<PlayerMovement>();
        input.OnAttack += ThrowShield;
        cameraShake = GetComponent<CinemachineImpulseSource>();
        mainCamera = GameObject.Find("Normal camera");
        aimCamera = GameObject.Find("Aim camera");
    }


    private void Update()
    {
        CameraAimHandler();
    }

    private void ThrowShield(object sender, EventArgs e)
    {
        if (!player.IsShield()) { return; }
                
        if(!shotShield)
        {            
            AudioImp.Instance.Reproducir("ShieldThrow");

            player.ChangeAnimation("Throw");
            cameraShake.GenerateImpulse();
            shotShield = true;
            Vector3 direction = (mouseWorldPosition - spawnProjectile.position).normalized;
            GameObject shield = Instantiate(projectile, spawnProjectile.position, Quaternion.LookRotation(direction));
            shield.GetComponent<Shield>().Configure(player.model.transform);
        }

    }
    //funcion que el escudo lanzado llama cuando vuelve hacia el jugador
    public void RecoverShield()
    {
        shotShield = false;
    }

    private void CameraAimHandler()
    {
        if (player.IsShield() && !shotShield)
        {
            if(player.GetCurrentAnimation() != "Shield")
            {
                player.ChangeAnimation("Shield");
            }


            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
            crossAir.SetActive(true);


            mouseWorldPosition = Vector3.zero;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, colliderMask))
            {
                mouseWorldPosition = raycastHit.point;
            }

            //rotar al jugador hacia donde esta apuntando/defendiendose
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = player.model.position.y;
            Vector3 aimDirection = (worldAimTarget - player.model.position).normalized;
            player.model.forward = Vector3.Lerp(player.model.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {






            mainCamera.SetActive(true);
            aimCamera.SetActive(false);
            crossAir.SetActive(false);

        }

        
    }
    private void CameraMove()
    {
        Vector2 look = input.GetMouseInput();
        followTransform.transform.rotation *= Quaternion.AngleAxis(look.x * sensivity, Vector3.up);

        followTransform.transform.rotation *= Quaternion.AngleAxis(look.y * -sensivity, Vector3.right);

        angles = followTransform.transform.localEulerAngles;
        angles.z = 0;
        float angle = followTransform.transform.localEulerAngles.x;
        if (angle > 180 && angle < maxClamp)
        {
            angles.x = maxClamp;
        }
        else if (angle < 180 && angle > minClamp)
        {
            angles.x = minClamp;
        }
        followTransform.transform.localEulerAngles = angles;

    }
    private void LateUpdate()
    {
        CameraMove();
    }

    public  bool GetCanShield()
    {
        return !shotShield;
    }
}
