using System.Collections;
using System.Collections.Generic;


public class Move
{
    public int Value = -10;
    public Piece piece;
    public TileType DestinationTile;
    public bool CanTake = false;
    public Move(Piece _piece, TileType _dest, int _val, bool _cantake)
    {
        piece = _piece;
        DestinationTile = _dest;
        Value = _val;
        CanTake = _cantake;
    }
}
