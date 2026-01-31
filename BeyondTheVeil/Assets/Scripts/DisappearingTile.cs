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
    [SerializeField] private Sprite activeSprite;

    /// <summary>
    /// The sprite used when this tile is inactive
    /// </summary>
    [SerializeField] private Sprite inactiveSprite;

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
        m_collider.enabled = false;
        m_enabled = false;
        m_spriteRenderer.sprite = null;
    }

    /// <summary>
    /// Enables this tile
    /// </summary>
    private void ActivateTile()
    {
        m_collider.enabled = true;
        m_spriteRenderer.sprite = activeSprite;
    }

    /// <summary>
    /// Disables this tile
    /// </summary>
    private void DeactivateTile()
    {
        m_collider.enabled = false;
        m_spriteRenderer.sprite = inactiveSprite;
    }
}
