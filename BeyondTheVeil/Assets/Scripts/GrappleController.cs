using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleController : MonoBehaviour
{
   
    [SerializeField] public CharacterController.MaskState m_maskState;
    [SerializeField] private LayerMask m_grappleLayerMask;
    [SerializeField] private CharacterController characterController;
    

    public RaycastHit2D m_grappleHit;
    public bool m_grappling = false;
    

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Barriers")//when player hit platform grapple retracts
        {
            m_grappling = false;
            m_grappleHit = new RaycastHit2D();
        }
    }

    /// <summary>
    /// Casts a ray from the player to the mouse position on screen
    /// </summary>
    public void GrappleRayCast()
    {
        Debug.Log("Raycast");
        //takes the current mouse position from the current player position compared to the camera
        Vector2 grappleDirection = characterController.m_MainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)) - this.transform.position;
        //casts the acctual ray to whereever the mouse is on screen, ignores the player to stop bugs
        m_grappleHit = Physics2D.Raycast(this.transform.position, grappleDirection, 10, m_grappleLayerMask);
    }
}
