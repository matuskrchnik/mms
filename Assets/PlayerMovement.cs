using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private Transform cameraTransform;

    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private Animator anim;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    GameObject dialog;
    GameObject showDialog;
    GameObject menu;


    // Start is called before the first frame update
    void Start()
    {

        dialog = GameObject.Find("Dialog");
        dialog.SetActive(false);

        menu = GameObject.Find("Menu");
        menu.SetActive(true);

        showDialog = GameObject.Find("showDialog");
        showDialog.SetActive(false);
        Debug.Log(GameObject.Find("Sola").transform.position);
        Debug.Log(this.gameObject.transform.position);
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        anim = GetComponentInChildren<Animator>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(GameObject.Find("Sola").transform.position, this.gameObject.transform.position) < 50)
        {
            showDialog.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                dialog.SetActive(true);
            }
        }
        else
        {
            showDialog.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeInHierarchy == false)
            {
                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //{
           // inputMagnitude /= 2;
       // }

        //animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

        float speed = inputMagnitude * maximumSpeed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * speed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        if (movementDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            Walk();
        }

        if (movementDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }

        if (movementDirection == Vector3.zero)
        {
            Idle();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Idle()
    {
        anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Walk()
    {
        anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

    public void CloseDialog()
    {
        dialog.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}