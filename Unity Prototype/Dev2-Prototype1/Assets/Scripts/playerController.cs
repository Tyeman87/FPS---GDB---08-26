using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask ignoreLayer;

    [Range(1, 10)] [SerializeField]int HP;
    [Range(1, 10)] [SerializeField]int speed;
    [Range(2, 10)] [SerializeField]int sprintMod;
    [Range(5, 30)] [SerializeField]int jumpSpeed;
    [Range(1, 5)] [SerializeField]int jumpMax;
    [Range(15, 40)] [SerializeField]int gravity;
    
    [Range(1, 10)] [SerializeField]int shootDamage;
    [Range(3, 1000)] [SerializeField]int shootDist;
    [Range(0.1f, 5)] [SerializeField] float shootRate;

    int jumpCount;
    int HPOrig;
    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();
        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > shootRate)
        {
            //TODO:
            //shoot();
        }
    }

    void sprint()
    {
        
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }
}
