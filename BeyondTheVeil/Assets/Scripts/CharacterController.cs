using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static CharacterController;

public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// This defines the player mask state
    /// </summary>
    public enum MaskState
    {
        none = 0,
        doubleJump = 1,
        grapple = 2,
        wallTangibility = 3
    }

    /// <summary>
    /// The player direction
    /// </summary>
    private Vector2 m_playerDirection;

    /// <summary>
    /// The move input action
    /// </summary>
    private InputAction m_move;

    /// <summary>
    /// The jump input action
    /// </summary>
    private InputAction m_jump;

    /// <summary>
    /// The Players Rigidbody2D
    /// </summary>
    private Rigidbody2D m_playerRB2D;

    /// <summary>
    /// Interact action
    /// </summary>
    private InputAction m_interact;

    /// <summary>
    /// The currently selectedMask
    /// </summary>
    private MaskState m_maskState;

    /// <summary>
    /// Defined for the velocity player jumps 
    /// </summary>
    [SerializeField] private float m_jumpSpeed;

    /// <summary>
    /// Defined for the velocity player moves 
    /// </summary>
    [SerializeField] private float m_moveSpeed;

    /// <summary>
    /// An event which toggles if the walls are visable or not
    /// </summary>
    UnityEvent m_toggleDisappearingTiles;
    /// <summary>
    /// Enables the disappearing tiles
    /// </summary>
    UnityEvent m_enableDisappearingTiles;
    /// <summary>
    /// Disables the disappearing tiles
    /// </summary>
    UnityEvent m_disableDisappearingTiles;

    /// <summary>
    /// The manager for the disappearing tiles
    /// </summary>
    private DisappearingTileManager m_disappearingTileManager;

    [SerializeField] private GrappleController grappleController;

    /// <summary>
    /// jump counter, for when double jump mask is equipped it increases.
    /// </summary>
    [SerializeField] private int m_jumpCounter = 1;

    private float m_jumpCooldownTime = 0.2f;

    private float m_currentJumpCooldown = 0;

    private Coroutine Cr_HandleJumpInstance;

    /// <summary>
    /// bools for if each mask has been collected.
    /// </summary>
    private bool grappleMaskEquipped = false;
    private bool wallTangibilityMaskEquipped = false;
    private bool doubleJumpMaskEquipped = false;
   

    [SerializeField] private Camera m_MainCamera;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //Initialising Inputs
        m_move = InputSystem.actions.FindAction("Move");
        m_jump = InputSystem.actions.FindAction("Jump");
        //getting references to gameobjects
        m_playerRB2D = gameObject.GetComponent<Rigidbody2D>();
        InitialiseManagerAndControllerReferences();
        //stopping character and children rotating unnessacerily 
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    /// <summary>
    /// initialises controller references within this script instance based on objects in scene
    /// </summary>
    void InitialiseManagerAndControllerReferences()
    {
        if (GameObject.Find("DisappearingTileManager"))//sets dissappearing tile manager if this is a valid object
        {
            m_disappearingTileManager = GameObject.Find("DisappearingTileManager").GetComponent<DisappearingTileManager>();
        }
        if (GameObject.Find("Grapple"))//sets dissappearing tile manager if this is a valid object
        {
            grappleController = GameObject.Find("Grapple").GetComponent<GrappleController>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialiseDisappearingTileEvents();
    }

    /// <summary>
    /// initialises nessecary event for the dissappearing tiles and thier manager
    /// </summary>
    private void InitialiseDisappearingTileEvents()
    {
        m_toggleDisappearingTiles = new UnityEvent();
        m_toggleDisappearingTiles.AddListener(() => m_disappearingTileManager.ToggleDisappearingTiles());
        m_enableDisappearingTiles = new UnityEvent();
        m_enableDisappearingTiles.AddListener(() => m_disappearingTileManager.EnableDisappearingTiles());
        m_disableDisappearingTiles = new UnityEvent();
        m_disableDisappearingTiles.AddListener(() => m_disappearingTileManager.DisableDisappearingTiles());
    }

    private void FixedUpdate()
    {
        //adds the move to position
        transform.position += new Vector3(m_playerDirection.x * m_moveSpeed, m_playerDirection.y * m_moveSpeed, 0);
        //Grapples to appropriate position while player grappling is true
        if (grappleController.m_grappling == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, grappleController.m_grappleHit.point, 15f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleMaskPickups(collision);
    }

    /// <summary>
    /// On collision with an object if it is a mask stores and quips the appropriate mask
    /// </summary>
    /// <param name="collision"></param>
    private void HandleMaskPickups(Collider2D collision)
    {
        if (collision.gameObject.tag == "doubleJumpMask")
        {
            m_maskState = MaskState.doubleJump;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "wallTangibilityMask")
        {
            m_maskState = MaskState.wallTangibility;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "grappleMask")
        {
            m_maskState = MaskState.grapple;
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// This defines how the player moves
    /// </summary>
    /// <param name="ctx"></param>
    public void HandleMove(InputAction.CallbackContext ctx)
    {
        // makes it so player cant jump with W key
        if (ctx.performed && ctx.ReadValue<Vector2>().y <= 0)
        {
            m_playerDirection = ctx.ReadValue<Vector2>();
        }
        else if (ctx.canceled)
        {
            m_playerDirection = Vector2.zero;
        }
    }

    /// <summary>
    /// This calls the coroutine that handles how the player jumps
    /// </summary>
    /// <param name="ctx"></param>
    public void StartHandleJump(InputAction.CallbackContext ctx)
    {
        if (Cr_HandleJumpInstance == null && m_jumpCounter >= 1)
        {
            Cr_HandleJumpInstance = StartCoroutine(CR_HandleJump(ctx));
            m_jumpCounter--;
        }
    }

    /// <summary>
    /// Handles Jump cooldowns and jumps player whether on ground or midair when called 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    IEnumerator CR_HandleJump(InputAction.CallbackContext ctx)
    {
        while (m_currentJumpCooldown < m_jumpCooldownTime)
        {
            m_currentJumpCooldown += Time.deltaTime;
            yield return null;
        }
        m_playerRB2D.AddForce(new Vector2(0, 1f * m_jumpSpeed), ForceMode2D.Impulse);
        m_currentJumpCooldown = 0;
        Cr_HandleJumpInstance = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0)//if the player collides with the ground from above resets jumps appropriately
        {
            if (m_maskState == MaskState.doubleJump)
            {
                m_jumpCounter = 2;
            }
            else if (m_maskState != MaskState.doubleJump)
            {
                m_jumpCounter = 1;
            }
        }
    }

    public void HandleMaskSwitchDJump(InputAction.CallbackContext ctx)
    {
            m_maskState = MaskState.doubleJump;
            grappleController.m_maskState = MaskState.doubleJump;
    }

    public void HandleMaskSwitchGrapple(InputAction.CallbackContext ctx)
    {
            m_maskState = MaskState.grapple;
            grappleController.m_maskState = MaskState.grapple;
    }

    public void HandleMaskSwitchWall(InputAction.CallbackContext ctx)
    {
            m_maskState = MaskState.wallTangibility;
            grappleController.m_maskState = MaskState.wallTangibility;
    }

    /// <summary>
    /// dependant on the mask selected handles what to do when interact key is pressed
    /// </summary>
    /// <param name="ctx"></param>
    public void HandleInteract(InputAction.CallbackContext ctx)
    {
        switch (m_maskState)
        {
            case MaskState.wallTangibility:
                m_toggleDisappearingTiles.Invoke();
                break;

            case MaskState.grapple:
                grappleController.m_grappling = true;
                grappleController.GrappleRayCast();
                break;

            default:
                break;
        }
    }
}
