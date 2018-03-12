using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{


    // Width and height of dungeon (in tiles)
    // (Making these divisible by three)

    public int m_DungeonWidth = 3 * 20;
    public int m_DungeonHeight = 3 * 15;
    public float m_OmitRoomProbability = 0.05f;

    private int m_RoomCellWidth;
    private int m_RoomCellHeight;

    private int m_MinRoomWidth = 4;
    private int m_MaxRoomWidth;
    private int m_MinRoomHeight = 4;
    private int m_MaxRoomHeight;

    private int m_HallWidth = 2;

    private int m_RoomPadding = 1;

    private ArrayList m_Rooms;

    // Colors of different pixels
    private Color[] m_RoomColors = new Color[9];
    private Color m_HallColor;
    private Color m_EmptyColor;

    // The image we will generate
    public Texture2D m_DungeonImage;

    // An image in the scene to check if this stupid thing works
    public RawImage m_RawImage;

    public Color m_StartRoomColor;
    public Color m_StairsRoomColor;


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
                m_DungeonImage.SetPixel(x, y, Color.white);
            }
        }

        CreateRooms();
        DrawRooms();
        DrawHalls();

    }

    void CreateRooms()
    {
        m_Rooms = new ArrayList();

        // Temporary room grid for adding neighbours because I'm too dumb
        // to figure out something more elegant?
        Room[][] tempRoomGrid = { new Room[3], new Room[3], new Room[3] };

        // Create the rooms (keep generating until you don't accidentally
        // generate a dungeon with no rooms!
        while (m_Rooms.Count == 0)
        {
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (Random.value > m_OmitRoomProbability)
                    {
                        tempRoomGrid[col][row] = CreateRoom(row, col);
                        m_Rooms.Add(tempRoomGrid[col][row]);
                    }
                }
            }
        }

        // Add every room's neighbours for later using the grid
        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                Room room = tempRoomGrid[col][row];
                if (room == null) continue;

                if (row - 1 >= 0)
                {
                    room.AddNeighbour(tempRoomGrid[col][row - 1]);
                }
                if (row + 1 < 3)
                {
                    room.AddNeighbour(tempRoomGrid[col][row + 1]);
                }
                if (col - 1 >= 0)
                {
                    room.AddNeighbour(tempRoomGrid[col - 1][row]);
                }
                if (col + 1 < 3) 
                { 
                    room.AddNeighbour(tempRoomGrid[col + 1][row]); 
                }

                Debug.Log("Added " + room.neighbours.Count + " neighbours for room " + room.row + "," + room.col);
            }
        }
    }

    Room CreateRoom(int row, int col)
    {

        int width = m_MinRoomWidth + Mathf.FloorToInt(Random.value * (m_MaxRoomWidth - m_MinRoomWidth));
        int height = m_MinRoomHeight + Mathf.FloorToInt(Random.value * (m_MaxRoomHeight - m_MinRoomHeight));

        int xOffset = col * m_RoomCellWidth + m_RoomPadding;
        int yOffset = row * m_RoomCellHeight + m_RoomPadding;

        int x = xOffset + Mathf.FloorToInt(Random.Range(0, m_MaxRoomWidth - width));
        int y = yOffset + Mathf.FloorToInt(Random.Range(0, m_MaxRoomHeight - height));

        return new Room(row, col, x, y, width, height, m_RoomColors[col + row * col], m_HallColor);
    }


    void DrawRooms()
    {
        for (int i = 0; i < m_Rooms.Count; i++)
        {
            Room room = (Room)m_Rooms[i];
            room.Draw(m_DungeonImage);
        }
    }


    void DrawHalls()
    {
        ArrayList unconnected = new ArrayList(m_Rooms);

        Debug.Log("There are " + unconnected.Count + " unconnected rooms.");

        // Choose a starting room (making sure it's not a "gone" room
        // (e.g. just a hallway junction)
        Room start = (Room)m_Rooms[Random.Range(0, m_Rooms.Count)];
        while (start.gone)
        {
            start = (Room)m_Rooms[Random.Range(0, m_Rooms.Count)];
        }
        start.connected = true;

        //Debug.Log("Stopping here.");
        //return;

        // Remember the color of the starting room for placing the chess pieces
        // (Oh god, what if the room is too small for your team?!)
        m_StartRoomColor = start.color;

        Room toConnect = start;
        while (toConnect != null)
        {
            unconnected.Remove(toConnect);
            // Repeatedly set the stairs room colour so it will end up as
            // the last room joined (in case there are no unconnected rooms
            // at this point
            // Make sure it's not a gone room through!
            if (!toConnect.gone) m_StairsRoomColor = toConnect.color;
            toConnect = ConnectToUnconnected(toConnect);
        }

        while (unconnected.Count > 0)
        {
            toConnect = (Room)unconnected[Random.Range(0, unconnected.Count)];
            bool success = ConnectToConnected(toConnect);
            if (success) {
                unconnected.Remove(toConnect);
                // Repeatedly set the stairs room colour to the most recently connected
                // non-gone room!
                if (!toConnect.gone) m_StairsRoomColor = toConnect.color;
            }
        }


        int extraConnections = Random.Range(0, 3);
        int connectionsMade = 0;
        int attempts = 0;
        while (connectionsMade < extraConnections)
        {
            toConnect = (Room)m_Rooms[Random.Range(0, m_Rooms.Count)];
            if (ConnectToRandomNeighbour(toConnect))
            {
                connectionsMade++;
            }
            else {
                attempts++;
            }
            // We need to give up if it's not working out, because there are
            // dungeon configurations where you just can't add connections.
            if (attempts > 20) {
                Debug.Log("Tried adding extra connections for too long!");
                break;
            }
        }

    }

    bool ConnectToRandomNeighbour(Room room)
    {
        Room randomNeighbour = (Room)room.neighbours[Random.Range(0, room.neighbours.Count)];
        if (room.connections.IndexOf(randomNeighbour) == -1)
        {
            DrawHall(room, randomNeighbour);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ConnectToConnected(Room room)
    {
        ArrayList connectedNeighbours = new ArrayList();
        for (int i = 0; i < room.neighbours.Count; i++)
        {
            Room neighbour = (Room)room.neighbours[i];
            if (neighbour.connected)
            {
                connectedNeighbours.Add(neighbour);
            }
        }

        if (connectedNeighbours.Count == 0) return false;

        Room randomConnectedNeighbour = (Room)connectedNeighbours[Random.Range(0, connectedNeighbours.Count)];
        DrawHall(room, randomConnectedNeighbour);
        room.connected = true;


        return true;
    }

    Room ConnectToUnconnected(Room room)
    {
        ArrayList unconnectedNeighbours = new ArrayList();
        for (int i = 0; i < room.neighbours.Count; i++)
        {
            Room neighbour = (Room)room.neighbours[i];
            if (!neighbour.connected)
            {
                unconnectedNeighbours.Add(neighbour);
            }
        }

        if (unconnectedNeighbours.Count == 0) return null;

        Room randomUnconnectedNeighbour = (Room)unconnectedNeighbours[Random.Range(0, unconnectedNeighbours.Count)];
        DrawHall(room,randomUnconnectedNeighbour);
        randomUnconnectedNeighbour.connected = true;

        return randomUnconnectedNeighbour;
    }


    void DrawHall(Room from, Room to)
    {
        Debug.Log("Drawing hall from " + from.row + "," + from.col + " to " + to.row + "," + to.col);

        // Note that the rooms are connected (for the algorithm later on)
        from.connections.Add(to);
        to.connections.Add(from);

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
            startX = Mathf.FloorToInt(Random.Range(start.x, start.x + start.width - 2));
            endX = Mathf.FloorToInt(Random.Range(end.x, end.x + end.width - 2));
            startY = start.y + start.height;
            endY = end.y;
        }

        else if (from.row == to.row)
        {
            startX = start.x + start.width;
            endX = end.x;
            startY = Mathf.FloorToInt(Random.Range(start.y, start.y + start.height - 2));
            endY = Mathf.FloorToInt(Random.Range(end.y, end.y + end.height - 2));
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

        int progress = 0;

        if (from.col == to.col)
        {
            while (y != endY)
            {
                if (x != endX && progress > 1)
                {
                    if (y == endY - 3 || y == endY + 3 || Random.value < 0.33)
                    {
                        while (x != endX)
                        {
                            m_DungeonImage.SetPixel(x, y, m_HallColor);
                            m_DungeonImage.SetPixel(x, y + 1, m_HallColor);
                            m_DungeonImage.SetPixel(x + 1, y, m_HallColor);
                            m_DungeonImage.SetPixel(x + 1, y + 1, m_HallColor);

                            x += xInc;
                        }
                    }
                }

                m_DungeonImage.SetPixel(x, y, m_HallColor);
                m_DungeonImage.SetPixel(x + 1, y, m_HallColor);
                y += yInc;
                progress++;
            }
        }
        else if (from.row == to.row)
        {
            while (x != endX)
            {
                if (y != endY && progress > 1)
                {
                    if (x == endX - 3 || x == endX + 3 || Random.value < 0.33)
                    {
                        while (y != endY)
                        {
                            m_DungeonImage.SetPixel(x, y, m_HallColor);
                            m_DungeonImage.SetPixel(x + 1, y, m_HallColor);
                            m_DungeonImage.SetPixel(x, y + 1, m_HallColor);
                            m_DungeonImage.SetPixel(x + 1, y + 1, m_HallColor);
                            y += yInc;
                        }
                    }

                }

                m_DungeonImage.SetPixel(x, y, m_HallColor);
                m_DungeonImage.SetPixel(x, y + 1, m_HallColor);
                x += xInc;
                progress++;
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
    public Color hallColor;
    public bool connected = false;

    public int WEST = 0;
    public int EAST = 1;
    public int NORTH = 2;
    public int SOUTH = 3;

    public bool gone = false;

    // Track the rooms this room is connected to
    public ArrayList connections;
    // Track the rooms this room is adjascent to
    public ArrayList neighbours;

    public Room(int _row, int _col, int _x, int _y, int _width, int _height, Color _color, Color _hallColor)
    {
        row = _row;
        col = _col;
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        color = _color;
        hallColor = _hallColor;
        connections = new ArrayList();
        neighbours = new ArrayList();

        if (Random.value < 0.05f)
        {
            gone = true;
            x = x + width / 2;
            y = y + height / 2;
            width = 2;
            height = 2;
        }

        //doors = new Room[4];
    }

    public void Draw(Texture2D image)
    {
        for (int yy = y; yy < y + height; yy++)
        {
            for (int xx = x; xx < x + width; xx++)
            {
                if (!gone)
                {
                    image.SetPixel(xx, yy, color);
                }
                else
                {
                    image.SetPixel(xx, yy, hallColor);
                }
            }
        }
    }

    public void AddNeighbour(Room room)
    {
        if (room != null)
        {
            Debug.Log("Adding " + room.row + "," + room.col + " as neighbour of " + row + "," + col);
            neighbours.Add(room);
        }
        else 
        {
            Debug.Log("NOT adding null room as neighbour of " + row + "," + col);
        }
    }
}