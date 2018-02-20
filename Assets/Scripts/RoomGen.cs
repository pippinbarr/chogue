using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MonoBehaviour
{

    public int m_LevelWidth = 40;
    public int m_LevelHeight = 25;

    public int padding = 1;
    public int minRoomWidth = 2;
    public int minRoomHeight = 2;
    public int hallSize = 2;

    private int roomZoneWidth;
    private int roomZoneHeight;

    private int maxRoomWidth;
    private int maxRoomHeight;

    public Texture2D m_Level;


    void Start()
    {
        // Calculate how big the space a room can occupy is
        // (a third of the screen - the two hallways)
        roomZoneWidth = (m_LevelWidth - 2 * hallSize) / 3;
        // And for height
        roomZoneHeight = (m_LevelHeight - 2 * hallSize) / 3;
        // Then calculate the maximum width a room can take up, minus padding
        maxRoomWidth = roomZoneWidth - 2 * padding;
        maxRoomHeight = roomZoneHeight - 2 * padding;

        GenerateLevel();
    }


    private void GenerateLevel()
    {
        m_Level = new Texture2D(m_LevelWidth, m_LevelHeight);
        m_Level.filterMode = FilterMode.Point;
        GetComponent<Renderer>().material.mainTexture = m_Level;

        // Every pixel is black by default, so we "only" need to fill the texture
        // with space

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                AddRoom(row * (roomZoneWidth + hallSize) + padding, col * (roomZoneHeight + hallSize) + padding);
            }
        }

    }

    private void AddRoom(int roomX, int roomY)
    {
        int width = minRoomWidth + Mathf.FloorToInt(Random.value * (maxRoomWidth - minRoomWidth));
        int height = minRoomHeight + Mathf.FloorToInt(Random.value * (maxRoomHeight - minRoomHeight));
        width = maxRoomWidth;
        height = maxRoomHeight;

        for (int y = roomY; y < roomY + height; y++)
        {
            for (int x = roomX; x < roomX + width; x++)
            {
                m_Level.SetPixel(x,y,Color.white);
            }
        }
    }


}
