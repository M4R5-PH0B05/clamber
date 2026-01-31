using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor.Rendering;

public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// This defines the player mask state
    /// </summary>
    private enum MaskState
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
    private Rigidbody2D RB2D;

    private InputAction m_interact;

    /// <summary>
    /// The currently selectedMask
    /// </summary>
    private MaskState m_maskState;

    private bool m_canjump = false;
    private float m_jumpcooldown = 0.1f;
    private float m_jumptimeout;

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

    private bool m_grappling = false;

    private RaycastHit2D m_grappleHit;
    /// <summary>
    /// jump counter, for when double jump mask is equipped it increases.
    /// </summary>
    private int m_jumpcounter = 1;
    /// <summary>
    /// bools for if each mask has been collected.
    /// </summary>
    private bool grapplemaskequipped = false;
    private bool walltangibilitymaskequipped = false;
    private bool doublejumpmaskequipped = false;
    [SerializeField] private LayerMask m_grappleLayerMask;

    [SerializeField] private Camera m_MainCamera;

    void Awake()
    {

        m_move = InputSystem.actions.FindAction("Move");
        m_jump = InputSystem.actions.FindAction("Jump");
        RB2D = gameObject.GetComponent<Rigidbody2D>();
        m_disappearingTileManager = FindObjectOfType<DisappearingTileManager>();
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        if (m_grappling == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, m_grappleHit.point, 15f * Time.deltaTime);
        }



        //takes the current mouse position from the current player position compared to the camera
        Vector2 grappleDirection = m_MainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)) - this.transform.position;
        Debug.DrawRay(this.transform.position, grappleDirection, Color.red, 10);
        //casts the acctual ray to whereever the mouse is on screen, ignores the player to stop bugs
        m_grappleHit = Physics2D.Raycast(this.transform.position, grappleDirection, 10, m_grappleLayerMask);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Barriers")//when player hit platform grapple retracts
        {
            m_grappling = false;
            m_grappleHit = new RaycastHit2D();
        }
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "doublejumpmask")
        {
            doublejumpmaskequipped = true;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "walltangibilitymask")
        {
            walltangibilitymaskequipped = true;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "grapplemask")
        {
            grapplemaskequipped = true;
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
    /// This defines how the player jumps
    /// </summary>
    /// <param name="ctx"></param>
    public void HandleJump(InputAction.CallbackContext ctx)
    {
        if (RB2D.linearVelocityY < 0.001 && RB2D.linearVelocityY > -0.001)
        {
            if (m_maskState == MaskState.doubleJump)
            {
                m_jumpcounter = 2;
            }
            else if (m_maskState != MaskState.doubleJump)
            {
                m_jumpcounter = 1;
            }
        }
        if (m_jumpcounter > 0 && Time.time > m_jumptimeout)
        {
            m_jumpcounter--;
            RB2D.AddForce(new Vector2(0, 15 * m_jumpSpeed), ForceMode2D.Impulse);
            m_jumptimeout = Time.time + m_jumpcooldown;
        }
    }

    public void HandleMaskSwitchDJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && doublejumpmaskequipped == true)
        {
            m_maskState = MaskState.doubleJump;
            Debug.Log("double jump");
            m_disableDisappearingTiles.Invoke();
        }
    }

    public void HandleMaskSwitchGrapple(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grapplemaskequipped == true)
        {

            m_maskState = MaskState.grapple;
            Debug.Log("grapple");
            m_disableDisappearingTiles.Invoke();
        }
    }

    public void HandleMaskSwitchWall(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && walltangibilitymaskequipped == true)
        {
            m_maskState = MaskState.wallTangibility;
            Debug.Log("wall");
            m_enableDisappearingTiles.Invoke();
        }
    }

    public void HandleInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (m_maskState == MaskState.wallTangibility)
            {
                m_toggleDisappearingTiles.Invoke();
                Debug.Log("m_maskstate");
            }
            if (m_maskState == MaskState.grapple)
            {
                m_grappling = true;
            }
        }
    }

    




}
