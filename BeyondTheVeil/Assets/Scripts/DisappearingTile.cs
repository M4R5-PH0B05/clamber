using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Tiles that change according to the wall tagibility mask
/// </summary>
public class DisappearingTile : MonoBehaviour
{
    /// <summary>
    /// If the tile is currently active
    /// </summary>
    private bool m_isTileActive;

    /// <summary>
    /// If the tile is enabled
    /// </summary>
    private bool m_enabled;

    /// <summary>
    /// The sprite used when the tile is active
    /// </summary>
    [SerializeField] private Sprite m_activeSprite;

    /// <summary>
    /// The sprite used when this tile is inactive
    /// </summary>
    [SerializeField] private Sprite m_inactiveSprite;

    /// <summary>
    /// The sprite used when the tile is disabled, if invisable set to null
    /// </summary>
    [SerializeField] private Sprite m_disabledSprite;

    /// <summary>
    /// This determines if the tile is collidable by default (including if there is no wall mask applied)
    /// </summary>
    [SerializeField] private bool m_collisionsOnDefault;

    /// <summary>
    /// The sprite renderer for this tile
    /// </summary>
    private SpriteRenderer m_spriteRenderer;

    /// <summary>
    /// The tile collider component
    /// </summary>
    private BoxCollider2D m_collider;

    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        //set to initial state
        DisableTile();
    }

    /// <summary>
    /// Toggles if this tile is active or not
    /// </summary>
    public void ToggleTileState()
    {
        if (m_isTileActive)
        {            
            DeactivateTile();
        }
        else
        {
            ActivateTile();
        }

        m_isTileActive = !m_isTileActive;
    }

    //Enable or disable this tile, this will be called by an event
    /// <summary>
    /// Enables this tile
    /// </summary>
    public void EnableTile()
    {
        if (m_isTileActive)
        {
            ActivateTile();
        }
        else
        {
            DeactivateTile();
        }
        m_enabled = true;
    }

    /// <summary>
    /// Disables the tile
    /// </summary>
    public void DisableTile()
    {
        if (m_collisionsOnDefault)
        {
            m_collider.enabled = true;
        }
        else
        {
            m_collider.enabled = false;
        }
        m_enabled = false;
        m_spriteRenderer.sprite = m_disabledSprite;
    }

    /// <summary>
    /// Enables this tile
    /// </summary>
    private void ActivateTile()
    {
        if (m_collisionsOnDefault)
        {
            m_collider.enabled = false;
        }
        else
        {
            m_collider.enabled = true;
        }
        m_spriteRenderer.sprite = m_activeSprite;
    }

    /// <summary>
    /// Disables this tile
    /// </summary>
    private void DeactivateTile()
    {
        if (m_collisionsOnDefault)
        {
            m_collider.enabled = true;
        }
        else
        {
            m_collider.enabled = false;
        }
        m_spriteRenderer.sprite = m_inactiveSprite;
    }
}
