using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float speed = 6.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    [SerializeField] float cameraYOffset = 1f;
    [SerializeField] TableViewEvents tableViewEvents;
    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
 
    void OnEnable()
    {
        tableViewEvents.OnPlayerInTrigger += SetCursorLockForDraw;
    }

    void OnDisable()
    {
        tableViewEvents.OnPlayerInTrigger -= SetCursorLockForDraw;
    }
    
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
        
        SetCursorLock(cursorLock);
       
    }

    void SetCursorLock(bool status)
    {
        if (status)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    void SetCursorLockForDraw(bool status)
    {
        if (status)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            SetCursorLock(cursorLock);
        }

    }



    void Update()
    {
        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity,
            mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);
        float horizontalRotation = currentMouseDelta.x * mouseSensitivity;

        Transform playerTransform = transform;
        playerCamera.position = new Vector3(playerTransform.position.x, playerTransform.position.y + cameraYOffset, playerTransform.position.z); // playerTransform.position;
        Quaternion targetCameraRotation = Quaternion.Euler(cameraCap, playerTransform.eulerAngles.y, 0);
        playerCamera.rotation = targetCameraRotation;
        playerTransform.Rotate(Vector3.up, horizontalRotation);

       
    }

    void UpdateMove()
    {
        //isGrounded = Physics.CheckSphere(transform.position, 0.7f, ground);
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, ground);
        
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed +
                           Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && velocity.y < 0f)
        {
            velocityY = -8f;
        }
    }
}