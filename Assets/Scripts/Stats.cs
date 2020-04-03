using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Stats : MonoBehaviour
{
    public int CurrentGame = 0;
    public int NormalizedMoves = 10;
    public List<List<Move>> Moves = new List<List<Move>>();
    // Start is called before the first frame update

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("chess");
    }

    public void SaveStats()
    {
        //save stats
        
        string path = @"d:\echecs\numchoices.txt";

        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("Number of available choices for white on each turn");
            }
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            string line = Moves.Count + "|";
            foreach (List<Move> moves in Moves)
            {
                line += moves.Count + "|";
            }
            sw.WriteLine(line);
        }

        //Now do the same thing but normalized

        path = @"d:\echecs\numchoices_normalized.txt";

        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("Number of available choices for white on each turn, normalized to 50 turns");
            }
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            string line = NormalizedMoves + "|";
            float ratio = (float)Moves.Count / NormalizedMoves;
            for(float i = 0; i< NormalizedMoves; i++)
            {   
                int index = Mathf.Abs(Mathf.RoundToInt(ratio * i));
                if (index < Moves.Count)
                {
                    if (Moves[index].Count > 0)
                        line += Moves[index].Count + "|";
                }


            }
            sw.WriteLine(line);
        }
            //Now record moves that have tension (threatened or possible capture)

            path = @"d:\echecs\tension.txt";

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("proportion of moves that have tension (piece threatened or can take)");
                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                string line = Moves.Count + "|";



                foreach (List<Move> moves in Moves)
                {
                    //count moves with tension
                    float MovesWithTension = 0;
                    foreach (Move choice in moves)
                    {
                        if ((choice.CanTake) || (choice.Threatened))
                        {
                            MovesWithTension++;
                        }
                    }

                    //save here

                    line += MovesWithTension / moves.Count + "|";
                }
                sw.WriteLine(line);
            }
            
            //Now record moves that have tension but normalized (threatened or possible capture)
            path = @"d:\echecs\tension_normalized.txt";

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("nb of moves that have tension (piece can take)");
                }
            }

        using (StreamWriter sw = File.AppendText(path))
        {
            string line = NormalizedMoves + "|";
            float ratio = (float)Moves.Count / NormalizedMoves;
            for (float i = 0; i < NormalizedMoves; i++)
            {
                float MovesWithTension = 0;
                int index = Mathf.Abs(Mathf.RoundToInt(ratio * i));
                if (index > Moves.Count)
                {
                    foreach (Move choice in Moves[index])
                    {
                        if ((choice.CanTake) || (choice.Threatened))
                        {
                            MovesWithTension++;
                        }
                    }
                    if (Moves[index].Count > 0)
                        line += MovesWithTension / Moves[index].Count + "|";
                }

            }


            sw.WriteLine(line);
        }
        //save % of choices related to each piece type
        // order: P, Ki, Q, Kn, B, R
        float TotalChoices = 0;
        float PawnChoices = 0;
        float KingChoices = 0;
        float QueenChoices = 0;
        float KnightChoices = 0;
        float BishopChoices = 0;
        float RookChoices = 0;
        foreach(List<Move> moves in Moves)
        {
            foreach(Move move in moves)
            {
                TotalChoices++;
                if (move.piece.PieceType == "pawn")
                    PawnChoices++;
                if (move.piece.PieceType == "king")
                    KingChoices++;
                if (move.piece.PieceType == "queen")
                    QueenChoices++;
                if (move.piece.PieceType == "knight")
                    KnightChoices++;
                if (move.piece.PieceType == "bishop")
                    BishopChoices++;
                if (move.piece.PieceType == "rook")
                    RookChoices++;


            }
        }
        path = @"d:\echecs\piece_choices.txt";

        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("Percentage of choices for each piece type, order: P, Ki, Q, Kn, B, R");
            }
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            string line = PawnChoices/TotalChoices + "|" + KingChoices / TotalChoices + "|"+ QueenChoices / TotalChoices + "|" + KnightChoices / TotalChoices + "|" + +BishopChoices / TotalChoices + "|" + RookChoices / TotalChoices + "|";
            
            sw.WriteLine(line);
        }


    }

    public void NextGame()
    {

        if (Moves.Count > 3)
        {
            SaveStats();
        }
        Moves.Clear();

        CurrentGame++;
        SceneManager.LoadScene("chess");
    }



}
