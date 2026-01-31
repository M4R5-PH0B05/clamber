using System.Linq;
using UnityEngine;

public class DisappearingTileManager : MonoBehaviour
{
    private DisappearingTile[] m_disappearingTiles;

    void Awake()
    {
        //initialising disappearing tiles in the scene
        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("DisappearingTile");
        m_disappearingTiles = tileObjects.Select(obj => obj.GetComponent<DisappearingTile>()).ToArray();
    }

    /// <summary>
    /// Enables all dissappearing tile objects in the scene
    /// </summary>
    public void EnableDisappearingTiles()
    {
        // Implementation to enable disappearing tiles
        Debug.Log("Disappearing tiles enabled.");
        foreach (var tile in m_disappearingTiles)
        {
            tile.EnableTile();
        }
    }

    /// <summary>
    /// disables all dissappearing tile objects in the scene
    /// </summary>
    public void DisableDisappearingTiles()
    {
        // Implementation to disable disappearing tiles
        Debug.Log("Disappearing tiles disabled.");
        foreach (var tile in m_disappearingTiles)
        {
            tile.DisableTile();
        }
    }

    /// <summary>
    /// Toggles the state of all dissappearing tile objects to be enabled or disabled
    /// </summary>
    public void ToggleDisappearingTiles()
    {
        // Implementation to toggle disappearing tiles
        Debug.Log("Disappearing tiles toggled.");
        foreach (var tile in m_disappearingTiles)
        {
            tile.ToggleTileState();
        }
    }
}
