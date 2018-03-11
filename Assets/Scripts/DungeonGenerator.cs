using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{

    // Width and height of dungeon (in tiles)
    // (Making these divisible by three)
    private int m_DungeonWidth = 42;
    private int m_DungeonHeight = 27;

    private int m_RoomCellWidth;
    private int m_RoomCellHeight;

    private int m_MinRoomWidth = 4;
    private int m_MaxRoomWidth;
    private int m_MinRoomHeight = 3;
    private int m_MaxRoomHeight;

    private int m_HallWidth = 2;

    private int m_RoomPadding = 1;

    private Room[][] m_Rooms;

    // Colors of different pixels
    private Color[] m_RoomColors = new Color[9];
    private Color m_HallColor;
    private Color m_EmptyColor;

    // The image we will generate
    public Texture2D m_DungeonImage;

    public RawImage m_RawImage;


    void Start()
    {
        m_RoomCellWidth = m_DungeonWidth / 3;
        m_RoomCellHeight = m_DungeonHeight / 3;

        m_MaxRoomWidth = m_RoomCellWidth - (m_RoomPadding * 2) - m_HallWidth;
        m_MaxRoomHeight = m_RoomCellHeight - (m_RoomPadding * 2) - m_HallWidth;

        // Set up the colors
        for (int i = 0; i < m_RoomColors.Length; i++)
        {
            m_RoomColors[i] = new Color((i + 1) / 10f, (i + 1) / 10f, (i + 1) / 10f);
        }
        m_HallColor = new Color(1.0f, 0f, 0f);
        m_EmptyColor = new Color(0f, 0f, 0f);

        GenerateDungeon();

        m_DungeonImage.Apply();
        m_RawImage.texture = m_DungeonImage;
    }

    void GenerateDungeon()
    {
        // Create the Texture2D that will contain the map
        m_DungeonImage = new Texture2D(m_DungeonWidth, m_DungeonHeight);
        // No aliasing
        m_DungeonImage.filterMode = FilterMode.Point;
        // Make the renderer of the object this image
        GetComponent<Renderer>().material.mainTexture = m_DungeonImage;

        for (int x = 0; x < m_DungeonImage.width; x++)
        {
            for (int y = 0; y < m_DungeonImage.height; y++)
            {
                m_DungeonImage.SetPixel(x, y, new Color(Random.Range(0.7f, 0.8f), Random.Range(0.7f, 0.8f), Random.Range(0.7f, 0.8f)));
            }
        }

        CreateRooms();
        DrawRooms();
        DrawHalls();

    }

    void CreateRooms()
    {
        m_Rooms = new Room[][] { new Room[3], new Room[3], new Room[3] };

        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                CreateRoom(row, col);
            }
        }
    }

    void CreateRoom(int row, int col)
    {
        int width = m_MinRoomWidth + Mathf.FloorToInt(Random.value * (m_MaxRoomWidth - m_MinRoomWidth));
        int height = m_MinRoomHeight + Mathf.FloorToInt(Random.value * (m_MaxRoomHeight - m_MinRoomHeight));

        //width = m_MaxRoomWidth;
        //height = m_MaxRoomHeight;

        int xOffset = col * m_RoomCellWidth + m_RoomPadding;
        int yOffset = row * m_RoomCellHeight + m_RoomPadding;

        Debug.Log("Max:" + m_MaxRoomHeight);
        Debug.Log("Actual:" + height);

        int x = xOffset + Mathf.FloorToInt(Random.Range(0, m_MaxRoomWidth - width));
        int y = yOffset + Mathf.FloorToInt(Random.Range(0, m_MaxRoomHeight - height));

        Debug.Log("y:" + y);

        m_Rooms[col][row] = new Room(row, col, x, y, width, height, m_RoomColors[col + row * col]);
    }

    void DrawRooms()
    {
        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                m_Rooms[col][row].Draw(m_DungeonImage);
            }
        }
    }

    void DrawHalls()
    {
        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                if (col + 1 < 3) DrawHall(m_Rooms[col][row], m_Rooms[col + 1][row]);
                if (row + 1 < 3) DrawHall(m_Rooms[col][row], m_Rooms[col][row + 1]);
            }
        }
    }


    void DrawHall(Room from, Room to)
    {
        Room start;
        Room end;

        if ((from.col == to.col && from.row < to.row) || (from.row == to.row && from.col < to.col))
        {
            start = from;
            end = to;
        }
        else
        {
            start = to;
            end = from;
        }

        int startX;
        int endX;

        int startY;
        int endY;

        if (from.col == to.col)
        {
            startX = Mathf.FloorToInt(Random.Range(start.x, start.x + start.width - 1));
            endX = Mathf.FloorToInt(Random.Range(end.x, end.x + end.width - 1));
            startY = start.y + start.height;
            endY = end.y;
        }

        else if (from.row == to.row)
        {
            startX = start.x + start.width;
            endX = end.x;
            startY = Mathf.FloorToInt(Random.Range(start.y, start.y + start.height - 1));
            endY = Mathf.FloorToInt(Random.Range(end.y, end.y + end.height - 1));
        }
        else
        {
            startX = 0;
            startY = 0;
            endX = 0;
            endY = 0;
        }

        int y = startY;
        int x = startX;

        int xInc = startX < endX ? 1 : -1;
        int yInc = startY < endY ? 1 : -1;


        if (from.col == to.col)
        {
            while (y != endY)
            {
                if (y == startY + yInc * (endY - startY) / 2)
                {
                    while (x != endX)
                    {
                        m_DungeonImage.SetPixel(x, y, m_HallColor);
                        x += xInc;
                    }
                }
                m_DungeonImage.SetPixel(x, y, m_HallColor);
                y += yInc;
            }
        }
        else if (from.row == to.row)
        {
            while (x != endX)
            {
                if (x == startX + xInc * (endX - startX) / 2)
                {
                    while (y != endY)
                    {
                        m_DungeonImage.SetPixel(x, y, m_HallColor);
                        y += yInc;
                    }
                }
                m_DungeonImage.SetPixel(x, y, m_HallColor);
                x += xInc;
            }
        }
    }
}



class Room
{
    public int row;
    public int col;
    public int x;
    public int y;
    public int width;
    public int height;
    public Color color;

    public int WEST = 0;
    public int EAST = 1;
    public int NORTH = 2;
    public int SOUTH = 3;

    public Room[] doors;

    public Room(int _row, int _col, int _x, int _y, int _width, int _height, Color _color)
    {
        row = _row;
        col = _col;
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        color = _color;

        doors = new Room[4];
    }

    public void Draw(Texture2D image)
    {
        for (int yy = y; yy < y + height; yy++)
        {
            for (int xx = x; xx < x + width; xx++)
            {
                image.SetPixel(xx, yy, color);
            }
        }
    }
}