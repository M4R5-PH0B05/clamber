using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    /// Current Active Camera in the scene
    /// </summary>
    [SerializeField] private Camera m_MainCamera;

    /// <summary>
    /// provides a layer mask for what player's grapple acctual is able to hit (specific layers)
    /// </summary>
    [SerializeField] private LayerMask m_grappleLayerMask;

    private float CR_Timer;

    void Awake()
    {
        m_move = InputSystem.actions.FindAction("Move");
        m_jump = InputSystem.actions.FindAction("Jump");
        this.GetComponent<Rigidbody2D>().freezeRotation = true;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //adds the move to position
        transform.position += new Vector3(m_playerDirection.x * m_moveSpeed, m_playerDirection.y * m_moveSpeed, 0);

    }

    /// <header> Charecter Inputs </header>

    /// <summary>
    /// This defines how the player moves
    /// </summary>
    /// <param name="ctx"></param>
    public void HandleMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
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
        this.transform.position += new Vector3(0,1 * m_jumpSpeed,0);
    }

    public void HandleMaskSwitchDJump(InputAction.CallbackContext ctx)
    {
        m_maskState = MaskState.doubleJump;
        Debug.Log("double jump");
    }

    public void HandleMaskSwitchGrapple(InputAction.CallbackContext ctx)
    {
        m_maskState = MaskState.grapple;
        Debug.Log("grapple");
    }

    public void HandleMaskSwitchWall(InputAction.CallbackContext ctx)
    {
        m_maskState = MaskState.wallTangibility;
        Debug.Log("wall");
    }

    /// <summary>
    /// when grapple pressed, grapples appropriately
    /// </summary>
    /// <param name="ctx"></param>
    public void GrappleInput(InputAction.CallbackContext ctx)
    {
        Grapple();
    }

    IEnumerable CR_KeepGrappling(RaycastHit2D grappleHit)
    {
        while (CR_Timer < 5f)
        {
            if (grappleHit && m_maskState == MaskState.grapple)
            {
                transform.position = Vector2.MoveTowards(transform.position, grappleHit.point, 0.1f);
            }
            CR_Timer += Time.deltaTime;
        }
        CR_Timer = 0;
        yield return null;
    }
    

    private void Grapple()
    {
        //takes the current mouse position from the current player position compared to the camera
        Vector2 grappleDirection = m_MainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,0)) - this.transform.position ;
        Debug.DrawRay(this.transform.position, grappleDirection, Color.red, 10);
        //casts the acctual ray to whereever the mosue is on screen, ignores the player to stop bugs
        RaycastHit2D grappleHit = Physics2D.Raycast(this.transform.position, grappleDirection, 10, m_grappleLayerMask);
        StartCoroutine(CR_KeepGrappling(grappleHit));
    }


}
