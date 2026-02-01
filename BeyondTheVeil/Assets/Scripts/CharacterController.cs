using JetBrains.Annotations;
using System;
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
        wallTangibility = 3,
        climbingmask = 4,
    }

    private bool doubleJumpCollected;
    private bool grappleCollected;
    private bool wallTangibilityCollected;
    private bool climbingmaskCollected;
        
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
    public MaskState m_maskState;

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
    [SerializeField] private MaskController maskController;

    /// <summary>
    /// jump counter, for when double jump mask is equipped it increases.
    /// </summary>
    [SerializeField] private int m_jumpCounter = 1;

    private float m_jumpCooldownTime = 0.2f;

    private float m_currentJumpCooldown = 0;

    private Coroutine Cr_HandleJumpInstance;

  
    /// <summary>
    /// set true/false when climbing is true
    /// </summary>
    private bool isclimbing = false;
    private float m_timeSinceStuck;

    private GameObject m_CustomSceneManager;

    [SerializeField] public Camera m_MainCamera;

    /// <summary>
    /// The layer mask for the ground detection
    /// </summary>
    private LayerMask m_layerMask;

    private Animator m_playerAnimation;

    void Awake()
    {
        m_CustomSceneManager = GameObject.Find("CustomSceneManager");
        DontDestroyOnLoad(gameObject);
        //Initialising Inputs
        m_move = InputSystem.actions.FindAction("Move");
        m_jump = InputSystem.actions.FindAction("Jump");
        //getting references to gameobjects
        m_playerRB2D = gameObject.GetComponent<Rigidbody2D>();
        InitialiseManagerAndControllerReferences();
        //stopping character and children rotating unnessacerily 
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;

        m_layerMask = LayerMask.GetMask("Default");
        m_playerAnimation = GetComponent<Animator>();

        
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
        maskController.CurrentMask();
        m_MainCamera = GetComponentInChildren<Camera>();
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
        m_playerAnimation.SetFloat("VelocityY", m_playerRB2D.linearVelocityY);
        

        //adds the move to position
        transform.position += new Vector3(m_playerDirection.x * m_moveSpeed, m_playerDirection.y * m_moveSpeed, 0);
        //Grapples to appropriate position while player grappling is true
        if (grappleController.m_grappling == true && grappleController.m_grappleHit.distance != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, grappleController.m_grappleHit.point, 15f * Time.deltaTime);
        }
        else if (grappleController.m_grappling == false || grappleController.m_grappleHit.distance == 0)
        {
            grappleController.SetActiveFalse();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("help");
        HandleDoorExits(collision);
        HandleMaskPickups(collision);
        if (collision.gameObject.tag == "climbable" )
        {
            isclimbing = true;
            

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "climbable")
        {
            isclimbing = false;
            m_playerDirection.y = 0;
            m_playerRB2D.gravityScale = 1;
        }
        if (collision.gameObject.tag == "climbable" && m_maskState == MaskState.climbingmask)
        {
            isclimbing = false;
            m_playerDirection.y = 0;
            m_playerRB2D.gravityScale = 1;

        }
    }

    /// <summary>
    /// On collision with an object if it is a mask stores and quips the appropriate mask
    /// </summary>
    /// <param name="collision"></param>
    private void HandleMaskPickups(Collider2D collision)
    {
        if (collision.gameObject.tag == "doubleJumpMask")
        {
            maskController.CurrentMask();
            grappleController.m_maskState = MaskState.doubleJump;
            m_maskState = MaskState.doubleJump;
            doubleJumpCollected = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "wallTangibilityMask")
        {
            maskController.CurrentMask();
            grappleController.m_maskState = MaskState.doubleJump;
            m_maskState = MaskState.wallTangibility;
            wallTangibilityCollected = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "grappleMask")
        {
            maskController.CurrentMask();
            grappleController.m_maskState = MaskState.doubleJump;
            m_maskState = MaskState.grapple;
            grappleCollected = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "climbMask")
        {
            maskController.CurrentMask();
            grappleController.m_maskState = MaskState.doubleJump;
            m_maskState = MaskState.climbingmask;
            climbingmaskCollected = true;
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
        if (ctx.performed && ctx.ReadValue<Vector2>().y <= 0 )
        {
            //Animation
            m_playerAnimation.SetBool("Idle", false);
            m_playerDirection = ctx.ReadValue<Vector2>();

            
            
            if (m_playerDirection.x < 0)//rotate character sprite depending which way they are moving
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            { 
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

        }
        
        else if (ctx.canceled)
        {
            m_playerDirection = Vector2.zero;
            //Animation
            m_playerAnimation.SetBool("Idle", true);
            

        }  
     
    }
    public void HandleClimbing(InputAction.CallbackContext ctx)
    {
        Debug.Log(isclimbing);
        if (m_maskState == MaskState.climbingmask && isclimbing == true)
        {
            m_playerRB2D.AddForce(new Vector2(0,1), ForceMode2D.Impulse);
            m_playerRB2D.gravityScale = 0;
        }
        else if (ctx.canceled)
        {
            m_playerRB2D.gravityScale = 1;
            m_playerDirection.y = 0;
        }
        
        
    }

    /// <summary>
    /// This calls the coroutine that handles how the player jumps
    /// </summary>
    /// <param name="ctx"></param>
    public void StartHandleJump(InputAction.CallbackContext ctx)
    {
        if (Cr_HandleJumpInstance == null && m_jumpCounter >= 1 && isclimbing != true)
        {
            Cr_HandleJumpInstance = StartCoroutine(CR_HandleJump(ctx));
            m_playerAnimation.SetBool("Jumping", true);
            m_playerAnimation.SetBool("Grounded", false);
            if (Physics2D.Raycast(this.transform.position, Vector2.down, 0.1f, m_layerMask))//if the player is on the ground reset jump counter
            {
                return;
            }
            else
            {
                m_jumpCounter--;
            }
                
        }
    }

    /// <summary>
    /// Handles Jump cooldowns and jumps player whether on ground or midair when called 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    IEnumerator CR_HandleJump(InputAction.CallbackContext ctx)
    {
        //Coroutine CR_WaitForJumpAnimInstance = StartCoroutine(CR_WaitForJumpAmim());
        while (m_currentJumpCooldown < m_jumpCooldownTime)
        {
            m_currentJumpCooldown += Time.deltaTime;
            yield return null;
        }
        m_playerRB2D.totalForce = new Vector2(0, 0);
        m_playerRB2D.AddForce(new Vector2(0, 1f * m_jumpSpeed), ForceMode2D.Impulse);
        m_currentJumpCooldown = 0;
        Cr_HandleJumpInstance = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.GetContact(0).point.y < transform.position.y)
        {

            m_playerAnimation.SetBool("Jumping", false);
            m_playerAnimation.SetBool("Grounded", true);

            if (m_maskState == MaskState.doubleJump && doubleJumpCollected)
            {
                m_jumpCounter = 2;
            }
            else //the double jump when only 1 jump is being set here after leaving ground
            {
                m_jumpCounter = 1;
            }
           
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.GetContact(0).point.y < transform.position.y && m_jumpCounter == 0 && m_timeSinceStuck > 0.1f)
        {
            transform.position += new Vector3(0,1,0);
            m_timeSinceStuck = 0;
        }
    }
    private void HandleDoorExits(Collider2D collision)
    {
        if (collision.gameObject.name == "Level1Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 2", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level2SpawnPosition);
        }
        else if (collision.gameObject.name == "Level2Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 3", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level3SpawnPosition);
        }
        else if (collision.gameObject.name == "Level3Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 4", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level4SpawnPosition);
        }
        else if (collision.gameObject.name == "Level4Exit")
        {
            Destroy(gameObject);
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Credits", new Vector3(0,0,0));
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Level1Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 2", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level2SpawnPosition);
        }
        else if (collision.gameObject.name == "Level2Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 3", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level3SpawnPosition);
        }
        else if (collision.gameObject.name == "Level3Exit")
        {
            m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level 4", m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().m_level4SpawnPosition);
        }
        if (collision.GetContact(0).point.y < transform.position.y && m_jumpCounter == 0)
        {
            m_timeSinceStuck += Time.deltaTime;
        }
        
    }

    public void HandleMaskSwitchDJump(InputAction.CallbackContext ctx)
    {
        if (doubleJumpCollected)
        {
            m_maskState = MaskState.doubleJump;
            grappleController.m_maskState = MaskState.doubleJump;
            maskController.CurrentMask();
            m_disableDisappearingTiles.Invoke();
        }
    }

    public void HandleMaskSwitchGrapple(InputAction.CallbackContext ctx)
    {
        if (grappleCollected)
        {
            m_jumpCounter = 1;
            m_maskState = MaskState.grapple;
            Debug.Log("Grapple Selected");
            grappleController.m_maskState = MaskState.grapple;
            maskController.CurrentMask();
            m_disableDisappearingTiles.Invoke();
        }
    }

    public void HandleMaskSwitchWall(InputAction.CallbackContext ctx)
    {
        if (wallTangibilityCollected)
        {
            m_jumpCounter = 1;
            m_maskState = MaskState.wallTangibility;
            grappleController.m_maskState = MaskState.wallTangibility;
            maskController.CurrentMask();
            m_enableDisappearingTiles.Invoke();
        }
    }
    public void HandleMaskClimbing(InputAction.CallbackContext ctx)
    {
        if (climbingmaskCollected)
        {
            m_jumpCounter = 1;
            m_maskState = MaskState.climbingmask;
            grappleController.m_maskState = MaskState.climbingmask;
            maskController.CurrentMask();
            m_disableDisappearingTiles.Invoke();
        }
        
    }

    /// <summary>
    /// dependant on the mask selected handles what to do when interact key is pressed
    /// </summary>
    /// <param name="ctx"></param>
    public void HandleInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started && m_maskState == MaskState.wallTangibility)
        {
            m_toggleDisappearingTiles.Invoke();
        }
        else if (ctx.started && grappleController.m_maskState == MaskState.grapple)
        {
            grappleController.m_grappling = true;
            grappleController.GrappleRayCast();
            Debug.Log("Grappling");
        }
    }

    IEnumerator CR_WaitForJumpAmim()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
