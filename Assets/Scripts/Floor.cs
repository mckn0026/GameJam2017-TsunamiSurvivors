using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Tower m_Tower;
    public Transform startPos;
    public FloorDirection ExitDirection = FloorDirection.Right;
    public bool m_IsActive;

    public bool Cleared = false;
    private static float xPosition = 0.0f;
    private bool m_HasPlayer = false;
    public bool GetPlayer
    {
        get
        {
            return m_HasPlayer;
        }
    }

    private readonly Dictionary<Enum, Func<GameObject, bool>> CheckLevelUpdate = new Dictionary<Enum, Func<GameObject, bool>>()
    {
        {FloorDirection.Right, HasPassedRight },
        {FloorDirection.Left, HasPassedLeft }

    };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MovePlayerToNextFloor(GameObject player)
    {
        m_Tower.MoveToNextFloor(player, this.gameObject);
    }

    public void ActivateFloor(Vector3 position)
    {
        m_IsActive = true;
        //m_Overlay.SetActive(false);
        //TODO: Remove any overlays hiding the contents of this floor
        transform.position = new Vector3(position.x, position.y + GetComponent<SpriteRenderer>().bounds.size.y, position.z);
    }

    internal void AddTower(Tower tower)
    {
        this.m_Tower = tower;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(other.tag, "Player", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            m_Tower.CheckIsLastFloor(this);
            if(!Cleared)
            {
                Cleared = true;
                m_Tower.IncreateWaterSpeed();
            }
        }
    }

    public static bool HasPassedRight(GameObject player)
    {
        if (player.transform.position.x < xPosition)
        {
            return true;
        }

        return false;
    }

    public static bool HasPassedLeft(GameObject player)
    {
        if (player.transform.position.x > xPosition)
        {
            return true;
        }

        return false;
    }

    internal void DeactivateFloor()
    {
        m_IsActive = false;
        Cleared = false;
        this.gameObject.SetActive(false);
    }
}
