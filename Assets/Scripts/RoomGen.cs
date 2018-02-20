using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MonoBehaviour
{

    public int m_LevelWidth = 40;
    public int m_LevelHeight = 25;

    public int padding = 1;
    public int minRoomWidth = 3;
    public int minRoomHeight = 3;
    public int hallSize = 2;

    private int roomZoneWidth;
    private int roomZoneHeight;

    private int maxRoomWidth;
    private int maxRoomHeight;

    private Room[][] rooms;

    private Vector2[][] hDoors;
    private Vector2[][] vDoors;
     
    public Texture2D m_Level;


    void Start()
    {
        rooms = new Room[][] { new Room[3], new Room[3], new Room[3] };

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

        CreateRooms();


        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                AddDoors(rooms[col][row]);
                DrawRoom(rooms[col][row]);
                DrawHalls(rooms[col][row]);
            }
        }

        //m_Level.SetPixel(0,0,Color.white);

    }

    private void CreateRooms()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                CreateRoom(row,col);
            }
        }
    }

    private void CreateRoom(int row, int col)
    {
        int width = minRoomWidth + Mathf.FloorToInt(Random.value * (maxRoomWidth - minRoomWidth));
        int height = minRoomHeight + Mathf.FloorToInt(Random.value * (maxRoomHeight - minRoomHeight));
        //width = maxRoomWidth;
        //height = maxRoomHeight;
        int x = col * (roomZoneWidth + hallSize) + padding;
        int y = row * (roomZoneHeight + hallSize) + padding;
        rooms[col][row] = new Room(row, col, x, y, width, height);
    }

    private void AddDoors(Room room)
    {
        if (room.row + 1 <= 2) room.doors[room.SOUTH] = rooms[room.col][room.row + 1];
        if (room.col + 1 <= 2) room.doors[room.EAST] = rooms[room.col + 1][room.row];
    }

    private void DrawRoom(Room room)
    {

        for (int y = room.y; y <= room.y + room.height; y++)
        {
            for (int x = room.x; x <= room.x + room.width; x++)
            {
                m_Level.SetPixel(x,y,Color.white);
            }
        }
    }

    private void DrawHalls(Room room)
    {
        Room eastRoom = room.doors[room.EAST];
        if (eastRoom != null) {
            int startX = room.x + room.width;
            int startY = room.y + Mathf.FloorToInt(Random.value * (room.height - 1));
            int endX = eastRoom.x;
            int endY = eastRoom.y + Mathf.FloorToInt((Random.value * (eastRoom.height - 1)));

            int currentX = startX;
            int currentY = startY;

            int moveX = 1;
            int moveY = 0;
            while (currentX < endX/2)
            {
                m_Level.SetPixel(currentX, currentY, Color.white);
                currentX++;
            }
            while (currentY != endY)
            {
                if (endY - currentY < 0) currentY--;
                else if (endY - currentY > 0) currentY++;
                m_Level.SetPixel(currentX, currentY, Color.white);
            }
            //while (currentX < endX)
            //{
            //    m_Level.SetPixel(currentX, currentY, Color.white);
            //    currentX += moveX;
            //}

            //m_Level.SetPixel(endX - 1, endY,Color.white);
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

    public int WEST = 0;
    public int EAST = 1;
    public int NORTH = 2;
    public int SOUTH = 3;

    public Room[] doors;

    public Room(int _row, int _col, int _x, int _y, int _width, int _height)
    {
        row = _row;
        col = _col;
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        doors = new Room[4];
    }
}