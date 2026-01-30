using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;

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


    void Awake()
    {
        m_move = InputSystem.actions.FindAction("Move");
        m_jump = InputSystem.actions.FindAction("Jump");
        
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

}
