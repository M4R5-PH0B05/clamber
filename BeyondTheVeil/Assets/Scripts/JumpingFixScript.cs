using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static CharacterController;

public class JumpingFixScript : MonoBehaviour
{
    /// <summary>
    /// This calls the coroutine that handles how the player jumps
    /// </summary>
    /// <param name="ctx"></param>
    /// 










    /*
    public void StartHandleJump(InputAction.CallbackContext ctx)
    {
        if (Cr_HandleJumpInstance == null && m_jumpCounter >= 1)
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
        Coroutine CR_WaitForJumpAnimInstance = StartCoroutine(CR_WaitForJumpAmim());
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        //for every object the player is colliding with, if it is the ground reset jump counter
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0)//if the player collides with the ground from above resets jumps appropriately
            {

                m_playerAnimation.SetBool("Jumping", false);
                m_playerAnimation.SetBool("Grounded", true);
                print
                if (m_maskState == MaskState.doubleJump && doubleJumpCollected)
                {
                    m_jumpCounter = 2;
                }
                else
                {
                    m_jumpCounter = 1;
                }
            }
        }
        if (collision.gameObject.tag == "climbable" && m_maskState == MaskState.climbingmask)
        {
            isclimbing = true;

        }

       

    }*/
}
