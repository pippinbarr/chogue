using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using System.Linq;


public class MainManager : MonoBehaviour {

    //Titles
    public Text title;
    public Text subtitle;
    bool firstmove = false;
    public Text resignbutton;
    public Transform Popup;

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    private Piece LastSelectedPiece;
    public  bool PieceSelected = false;
    public bool WaitingForPlayerMove = false;
    public bool WaitingForCPUMove = false;
    public bool WaitingForMove = false;
    public bool WaitingForMoveBack = false;
    public List<Piece> PieceList = new List<Piece>();
    public List<TileType> TileList = new List<TileType>();
    int CurrentPieceIndex = 0;
    bool gameover = false;
    public bool HumanTurn = false;
    public bool Automated = false;
    public AudioClip gulp;
    public AudioClip sliding;
    public AudioClip putdown;
    public AudioClip tic;
    public AudioClip endchord;
    public AudioClip winchord;
    public Text statusline;
    public Text msgline;
    public Text congratulationsText;
    public bool firstscene = false;


    private bool dothisonce = true; //hack
    public bool restartgame = false;
    public bool HitPointVersion = false;
    private bool changinglevel = false;
    private Vector3 touchStartPos;


    // Message and notation vars
    int Turn = 1;
    private string TempMessage = "";
    private string WhiteMoveNotation = "";
    private string WhiteMoveMessage = "";

    private int statusLineLength = 103; // Oh hardcoding, oh god
    private string[] files = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz" };
    private string[] hits = new string[] { "hits", "swings and hits", "has injured", "scored an excellent hit on" };
    private string[] misses = new string[] { "misses", "swings and misses", "barely misses", "doesn't hit" };

    //camera stuff
    public bool MovingCamera = false;
    private float touchbegan = 0f;
    //public Vector3 InitialMousePosition;





    // Use this for initialization
    void Start () {

        //Debug.Log("MainManager.Start()");
        //Debug.Log("grandmaster is " + PlayerPrefs.GetInt("grandmaster", 0));




        //Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "LastLevel")
        {
            // Submit their achievement
    
            // Load the victory scene (which will submit the score itself)

            // GetComponent<AudioSource>().clip = winchord;
            //GetComponent<AudioSource>().Play();
            int rating = PlayerPrefs.GetInt("Choguelo",0);

            // SET CUSTOM CONGRATULATIONS TEXT
            congratulationsText.text = "You have captured the King of Yendor. You journey home, sell your loot at a great profit and ";
            if (PlayerPrefs.GetInt("newgrandmaster",0) == 1)
            {
                congratulationsText.text += "are admitted to the Grandmasters Guild with a rating of " + rating + ".";
            }
            else
            {
                congratulationsText.text += "are celebrated at the Grandmasters Guild for your rating of " + rating + ".";
            }
            PlayerPrefs.SetInt("newgrandmaster", 0);

            PlayerPrefs.SetInt("continued", 0);

        }
        if (restartgame)
        {
            PlayerPrefs.DeleteAll();
        }


        //if this is the first time, setup basic vars if not load them from playerprefs
        if ((PlayerPrefs.GetInt("maxlevel",0) == 0))
        {
            Debug.Log("new game");
            PlayerPrefs.SetInt("maxlevel", 1);
            PlayerPrefs.SetInt("maxgold", 0);
            PlayerPrefs.SetInt("taken", 0);
            PlayerPrefs.SetInt("maxtaken", 0);
            PlayerPrefs.SetInt("hptaken", 0);
            PlayerPrefs.SetInt("gold", 0);
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            PlayerPrefs.SetInt("continued", 0);
            PlayerPrefs.SetInt("Choguelo", 0);
            //knight is chevalier to distinguish and rook is tour

        }
        if ((PlayerPrefs.GetInt("level") == 1)&&(PlayerPrefs.GetInt("continued")==0))
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            PlayerPrefs.SetInt("taken", 0);
            PlayerPrefs.SetInt("hptaken", 0);
            PlayerPrefs.SetInt("gold", 0);
            PlayerPrefs.SetInt("continued", 0);
            PlayerPrefs.SetInt("Choguelo", 0);
            Debug.Log("choguelo:"+ PlayerPrefs.GetInt("Choguelo"));
        }
        if (!firstscene)
        {

            if ((PlayerPrefs.GetInt("level") > 1)||(PlayerPrefs.GetInt("continued")==1))
            {
                if (title != null)
                {
                    title.text = "Level " + PlayerPrefs.GetInt("level");
                }
                if (subtitle != null)
                {

                    subtitle.text = LevelNameGenerator.GetName();
                    subtitle.text += "\n\nGold: " + PlayerPrefs.GetInt("gold");
                    subtitle.text += "\nCaptures: " + PlayerPrefs.GetInt("taken");
                    subtitle.text += "\nCurrent rating: " + PlayerPrefs.GetInt("Choguelo");
                }
            }

            //create a level
            GetComponent<MapMaker>().CreateLevel();
        }
        //select a default human piece
        foreach (Piece piece in PieceList)
        {

            if (piece.human)
            {
                CurrentActivePiece = piece;
                //CurrentActivePiece.SetActive(true);
                
            }
        }

        //update visibility manually



        WaitingForPlayerMove = true;
        WaitingForCPUMove = false;


        

        // Debug.Log("player's turn");
        StartCoroutine(InitialStuff());
        
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(LevelNameGenerator.GetName());
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeTurn();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (dothisonce)
        {

            UpdateThreats();
            UpdateVisibility();

            dothisonce = false;
            UpdateStatus();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetString("Executor", "resigned");
            SceneManager.LoadScene("GameOver");
        }
        /*if ((Automated)&& (!WaitingForMove))
        {
            ChangeTurn();

            return;
        }*/
        //Get click on destination (only if in "move" mode)
        if ((WaitingForPlayerMove)&&(!WaitingForMove))
        {
           if (Automated)
            {
                CPUPlay("white");
                WaitingForPlayerMove = false;
                
                return;
            }
            if (!PieceSelected)
            {
                
                if (LastSelectedPiece != null)
                {
                    CurrentActivePiece = LastSelectedPiece;
                   // CurrentActivePiece.SetActive(true);
                    CurrentActivePiece.FindAvailableDestinations();
                    CurrentActivePiece.ShowDestinations();
                   // CurrentActivePiece.SetActive(false);
                    PieceSelected = true;
                    UpdateStatus();
                }
            }
            bool tapped = false;

            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    touchbegan = Time.time;
                    touchStartPos = Input.GetTouch(0).position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    float dist = Vector3.Distance(touchStartPos,Input.GetTouch(0).position);
                    Debug.Log(dist);
                    if ((Time.time - touchbegan) < 0.5f && dist < 50)
                    {
                        tapped = true;
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                tapped = true;
            }
            if (tapped)
            {
                //DisplayMsg("");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                int layerMask = LayerMask.GetMask("mouse");
                //  Debug.Log("Using layerMask of " + layerMask);

                if( (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)))
                {

                    Transform selection = hit.collider.transform;

                   // Debug.Log("Hit " + selection.name + " on layer " + selection.gameObject.layer);

                    // Transform gototile = (selection.parent ? selection.parent : selection);
                    Transform gototile = selection.parent;

                    //if this is an available destination, move!
                    if ((gototile.GetComponent<TileType>()!=null)&&(gototile.GetComponent<TileType>().AvailableDestination))
                    {
                        if (gototile.GetComponent<TileType>().CurrentPiece != null)
                        {
                            //Debug.Log("going to piece instead of tile");
                            gototile = gototile.GetComponent<TileType>().CurrentPiece;

                        }

                        
                        StartCoroutine(MoveToTile(gototile.GetComponent<TileType>()));

                        //clear titles on first move
                        if (!firstmove)
                        {
                            if (title != null)
                            {
                                title.enabled = false;
                            }
                            if (subtitle != null)
                            {
                                subtitle.enabled = false;
                            }
                        }

                        //WaitingForPlayerMove = false;
                        //UpdateVisibility();
                        

                    }
                    //if this is a human piece, select it
                    if((hit.transform.GetComponent<Piece>()!=null) &&(hit.transform.GetComponent<Piece>().human))
                    {
                        if (CurrentActivePiece != null)
                        {
                            CurrentActivePiece.SetActive(false);
                            CurrentActivePiece.HideDestinations();
                        }

                        CurrentActivePiece = hit.transform.GetComponent<Piece>();
                       // CurrentActivePiece.SetActive(true);
                        CurrentActivePiece.FindAvailableDestinations();
                        CurrentActivePiece.ShowDestinations();
                        LastSelectedPiece = CurrentActivePiece;
                        PieceSelected = true;
                        /*if (CurrentActivePiece.PieceType == "king")
                        {
                            DisplayMsg("Press [R] to resign");
                        }*/
                        UpdateStatus();
                        foreach (Piece piece in PieceList)
                        {
                            piece.SetActive(false);
                        }
                    }

                }

            }
        }
        /*else if((!WaitingForCPUMove)&&(!WaitingForMove))
        {
            // Debug.Log("calling play black");
            UpdateStatus();
            CPUPlay("black");
           


        }*/

    }

    public void UpdateThreats()
    {
        //clear all tiles of their "covered and threatened"
        foreach (TileType tile in TileList)
        {
            tile.covered = false;
            tile.threatened = "none";
        }
        //first, find available destinations for all pieces to updated "protected" and "covered" statuses
        foreach (Piece piece in PieceList)
        {
            piece.threatened = false;
            piece.protecting = 0;
            piece.guarding = false;
            piece.guarded = false;
            piece.GetComponent<TileType>().threatened = "none";
            piece.GetComponent<TileType>().covered = false;
            // piece.SetActive(true);

            // piece.SetActive(false);
        }
        foreach (Piece piece in PieceList)
        {
            
            piece.FindAvailableDestinations();
            // piece.SetActive(false);
        }
        foreach (Piece piece in PieceList)
        {

            piece.FindAvailableDestinations();
            // piece.SetActive(false);
        }
    }

    void  CPUPlay(string side)
    {
        //Debug.Log("Deciding move for " + side);

        List<Move> AllMoves = new List<Move>();

        //Collect moves from all pieces
        foreach(Piece piece in PieceList)
        {
            if (piece.PieceColor == side)
            {
                piece.DecideMove();
                foreach (Move move in piece.PossibleMoves)
                {
                    //Debug.Log("adding move of value " + move.Value);
                    AllMoves.Add(move);
                }
            }


        }
        if (side == "white")
        {
            Stats stats = GameObject.FindObjectOfType<Stats>();
            stats.Moves.Add(AllMoves);
        }

        AllMoves = AllMoves.OrderByDescending(move => move.Value).ToList();


        //Debug.Log(AllMoves.Count + " possibles moves and best move value is "+AllMoves[0].Value);
        if (AllMoves.Count == 0)
        {
            gameover = true;
            
        }


        int BestValue = AllMoves[0].Value;

        Move ChosenMove = AllMoves[0];

        //if best value is zero, sort by distance to enemy king
        List<Move> MoveList = new List<Move>();
        if (BestValue == 0)
        {
            //first find king
            Piece OtherKing = PieceList[0];
            foreach (Piece piece in PieceList)
            {
                if ((piece.PieceType == "king") && (piece.PieceColor != side))
                {
                    //Debug.Log("Found " + piece.PieceColor + " king");
                    OtherKing = piece;
                }
            }
            float lowestdistance = 10000000;
            foreach(Move move in AllMoves)
            {
                
                if (move.Value == 0)
                {
                    move.DestinationTile.GetDistanceTo(OtherKing.transform);
                    move.piece.CurrentTile.GetComponent<TileType>().GetDistanceTo(OtherKing.transform);
                    float currentdist = move.piece.CurrentTile.GetComponent<TileType>().DistanceToPiece;
                    //we don't want to backtrack
                    if(currentdist> move.DestinationTile.DistanceToPiece)
                    {
                        MoveList.Add(move);
                    }

                }
            }
        }
        else
        {
            //collect all moves of similar best value and choose randomly
            bool onlyking = true;
            foreach (Move move in AllMoves)
            {
                if (move.piece.PieceType != "king")
                {
                    onlyking = false;
                }
                if (move.Value == BestValue)
                {
                    MoveList.Add(move);
                }
            }
            if (onlyking)
            {
                //only the king remains, this is gameover in shatranj
                gameover = true;
                
            }

        }
        System.Random rnd = new System.Random();
        //one chance on 20 to do a completely random move

        if (AllMoves.Count == 0)
        {
            gameover = true;
        }

        else if ((rnd.Next(0, 20) < 1)||(MoveList.Count==0))
        {
            int RandomIndex = rnd.Next(0, AllMoves.Count);
            ChosenMove = AllMoves[RandomIndex];
        }
        else
        {
            int RandomIndex = rnd.Next(0, MoveList.Count);
            ChosenMove = MoveList[RandomIndex];
        }


        CurrentActivePiece = ChosenMove.piece;
        ChosenMove.piece.MakeMove(ChosenMove);

    }

    public void UpdateVisibility()
    {
        //Debug.Log("updating visibility of " + TileList.Count + "tiles");
        foreach (TileType tile in TileList)
        {
            tile.SetVisibility();
        }
        foreach (Piece piece in PieceList)
        {
            piece.SetVisibility();
        }
    }


    //make this a coroutine with actual movement, will collide with tiles and make them visible (maybe?!)
    public IEnumerator MoveToTile(TileType tile, bool nomessage = false)
    {
        //are we attacking the white king?
        if ((tile.GetComponent<Piece>() != null)&&(!CurrentActivePiece.human))
        {
           /* if (tile.GetComponent<Piece>().PieceType == "king"){
                Debug.Log("move to king");
                foreach (Piece piece in PieceList)
                {
                    if ((piece.PieceType == "king") && (piece.human))
                    {
                        Vector3 dest = new Vector3(piece.transform.position.x,piece.transform.position.y,Camera.main.transform.position.z);
                        Vector3 move = (dest - Camera.main.transform.position) / 200;
                        while (Vector3.Distance(dest, Camera.main.transform.position) > 1f)
                        {
                            Camera.main.transform.Translate((dest-Camera.main.transform.position)/50);
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                }
            }*/
        }
        
        //UpdateThreats();
        //Am I going to a tile that has a piece?
        if ((tile.CurrentPiece != null)&&(!nomessage))
        {
            Debug.Log("Found a piece at the tile, going there instead");
            tile = tile.CurrentPiece.GetComponent<TileType>();
        }

        //saving original position (for message)
        Transform OriginPosition = CurrentActivePiece.CurrentTile;
        Vector3 Origin = OriginPosition.position;
        
        //Debug.Log("destination tile type: "+tile.GetComponent<TileType>().Type);
        WaitingForMove = true;
        CurrentActivePiece.CurrentTile.GetComponent<Collider>().enabled = true;
        CurrentActivePiece.turn++;
        Vector3 destination = new Vector3(tile.transform.position.x, tile.transform.position.y, CurrentActivePiece.transform.position.z);
        CurrentActivePiece.HideDestinations();
        Vector3 movestep = (destination - CurrentActivePiece.transform.position) / (Vector3.Distance(destination, CurrentActivePiece.transform.position)*5);
        
        if(Vector3.Distance(destination, CurrentActivePiece.transform.position) > 4)
        {
            GetComponent<AudioSource>().clip = sliding;
            GetComponent<AudioSource>().Play();
        }
       // CurrentActivePiece.SetActive(true);
        //Debug.Log("About to get moving");
        while (( Vector3.Distance(destination, CurrentActivePiece.transform.position)>0.2f))
        {
            //Debug.Log("distance: "+Vector3.Distance(destination, CurrentActivePiece.transform.position));
            CurrentActivePiece.transform.position = CurrentActivePiece.transform.position + movestep;
            yield return new WaitForSeconds(0.01f);
            //UpdateVisibility();
        }
       //    Debug.Log("finished moving");
        //yield return new WaitForSeconds(0.01f);
        GetComponent<AudioSource>().clip = putdown;
        GetComponent<AudioSource>().Play();
        CurrentActivePiece.transform.position = destination;
        //did we land on stairs?
        
 

        

        //Start preparing the displayed message
        //Describe the move in chess notation


        string PieceType = CurrentActivePiece.PieceType;

        //string OriginCoordinate = OriginPosition.position.x.ToString() + OriginPosition.position.y.ToString();

        string PieceSymbol = CurrentActivePiece.PieceSymbol;
        string OriginCoordinate = OriginPosition.position.x.ToString() + OriginPosition.position.y.ToString();
        if (tile == null)
        {
            tile = TileList[0];
        }

        string DestinationCoordinate = tile.transform.position.x.ToString() + tile.transform.position.y.ToString();
        string ActionSymbol = ""; // will be set to x or * by EatPiece()
        string DestinationPiece = "";
        string InCheck = "";
        string Promotion = "";
        //check whether piece has enemy king in check

    


        TempMessage = ""; //reset message
                          // Calculate notation for the squares

        string startRank = Mathf.FloorToInt(OriginPosition.position.y).ToString();
        int startFileIndex = Mathf.FloorToInt(OriginPosition.position.x);
        string startFile = files[startFileIndex];

        string destRank = Mathf.FloorToInt(tile.transform.position.y).ToString();
        int destFileIndex = Mathf.FloorToInt(tile.transform.position.x);
        string destFile = files[destFileIndex];
        //Debug.Log("Moved piece's NewQueen is " + CurrentActivePiece.NewQueen + ", type is " + CurrentActivePiece.PieceType);



        // UpdateThreats();
        //was this a piece? then eat it!
        
        if (tile.transform.tag == "piece")
        {
            if (tile.GetComponent<Piece>().PieceColor != "red")
            {
                tile.GetComponent<Piece>().eaten = true;
            }
               
            ActionSymbol = EatPiece(tile.GetComponent<Piece>()); // SETS TempMessage with Attack Flavour

            //Debug.Log("eat piece at destination");
            DestinationPiece = tile.GetComponent<Piece>().PieceType;
        }
        WaitingForMove = false;
        foreach (Piece piece in PieceList)
        {
            piece.SetActive(true);
        }
        yield return new WaitForSeconds(0.05f);
        // MOVED Pawn promotion above check calculation between a P promoting to Q can put the K in check, so should
        // calculate its possible attacks based on that.
        //will not do the queening if the pawn was eaten
        //Debug.Log("is this new queen?");
        if ((CurrentActivePiece.NewQueen) && (!CurrentActivePiece.eaten) && (ActionSymbol != ".") && (ActionSymbol != "*"))
        {
           // Debug.Log("yes");
            CurrentActivePiece.Queen();
            CurrentActivePiece = PieceList[0];
            //CurrentActivePiece.SetActive(true);
            CurrentActivePiece.FindAvailableDestinations();
            OriginPosition = CurrentActivePiece.CurrentTile;
            if (CurrentActivePiece.human)
            {
                LastSelectedPiece = CurrentActivePiece = PieceList[0];
            }


            //OriginPosition.position = Origin;
            //ChangeTurn();


            Promotion = "=Q";
        }

        //waiting for all collisions to register
        if ((tile != null) && (tile.GetComponent<TileType>().Type == 3))
        {
            Debug.Log("landed on stairs");

            if (CurrentActivePiece.PieceColor == "white")
            {
                LastSelectedPiece = null;
                changinglevel = true;
                ChangeLevel();
            }
            else
            {
                //remove king
                PieceList.Remove(CurrentActivePiece);
                CurrentActivePiece.transform.position = new Vector3(10000, 10000, 10000);
                //CurrentActivePiece = PieceList[0];
            }
        }
        yield return new WaitForSeconds(0.1f);



        UpdateThreats();
       //yield return new WaitForSeconds(2.2f);
        //UpdateThreats();
        UpdateVisibility();
        foreach (Piece piece in PieceList)
        {
            piece.SetActive(false);
        }



        msgline.color = new Color(0.66f, 0.66f, 0.66f);

        if (CurrentActivePiece.check)
        {
            InCheck = "+";
            //msgline.color = new Color(255f / 255, 0 / 255, 0 / 255);
        }
        else
        {
        }
        // The eating/attacking message is defined in TempMessage by the EatPiece() function
        //Now building the full notation
        //string FullNotation = "(" + PieceType + OriginCoordinate + ActionSymbol + DestinationPiece + DestinationCoordinate + InCheck + ") ";




        if (ActionSymbol != "" && PieceType == "pawn")
        {
            // This is a pawn attack/capture, its symbol should be its current square
            PieceSymbol = startFile + startRank;
        }

        string MoveNotation = PieceSymbol + ActionSymbol + destFile + destRank + Promotion + InCheck;

        if (CurrentActivePiece.check)
        {
            MoveNotation = "<color=red>" + MoveNotation + "</color>";
         }

        //Now assembling complete message
        string FullMoveMessage;
        if (!nomessage)
        {
            if (CurrentActivePiece.human)
            {
                Turn++;

                FullMoveMessage = Turn.ToString() + ". " + MoveNotation;
                string AttackFlavour = TempMessage;
                if (AttackFlavour != "")
                {
                    FullMoveMessage += " (" + AttackFlavour + ")";
                }


                WhiteMoveMessage = FullMoveMessage; //keeping it for black's turn
            }
            else
            {
                
                FullMoveMessage = WhiteMoveMessage + " " + MoveNotation;
                string AttackFlavour = TempMessage;
                if (AttackFlavour != "")
                {
                    string TempFullMoveMessage = FullMoveMessage + " (" + AttackFlavour + ")";
                    while (TempFullMoveMessage.Length > statusLineLength) {
                        Debug.Log("Too long: " + TempFullMoveMessage + " (" + TempFullMoveMessage.Length + " > " + statusLineLength);
                        if (ActionSymbol == "*")
                        {
                            AttackFlavour = GenerateAttackMessage(tile.GetComponent<Piece>(), hits);
                        }
                        else if (ActionSymbol == ".")
                        {
                            AttackFlavour = GenerateAttackMessage(tile.GetComponent<Piece>(), misses);
                        }
                        TempFullMoveMessage = WhiteMoveMessage + " " + MoveNotation + " (" + AttackFlavour + ")";

                    }
                    FullMoveMessage = TempFullMoveMessage;
                }

            }

            //display message
            DisplayMsg(FullMoveMessage);
        }




        
        //if attack but not kill, send piece back to where it came from!
        if (WaitingForMoveBack)
        {
            WaitingForMove = true;
           // Debug.Log("going back to where I came from");

            WaitingForMoveBack = false;
            TileType temptile = FindTileByPosition(Origin);
            if (temptile == null)
            {
                temptile = OriginPosition.GetComponent<TileType>();
            }
            StartCoroutine(MoveToTile(temptile, true));
           // Debug.Log("waiting to get back");
            yield break;
           /* while (WaitingForMove==true)
            {
                //Debug.Log("WaitingForMove:" + WaitingForMove);
                yield return new WaitForSeconds(0.001f);
                //Debug.Log("WaitingForMove2:" + WaitingForMove);

            }*/

           // Debug.Log("got back, finish move function");
        }
        WaitingForMove = false;
        UpdateVisibility();
        UpdateThreats();
        yield return new WaitForSeconds(0.1f);
        if(!gameover)
        {
            ChangeTurn();
        }
        else
        {
            GameObject.FindObjectOfType<Stats>().NextGame();
        }
        






    }

    public string EatPiece(Piece piece)
    {


        //Calculate damage (0 to MaxHP)
        int DMG = (int)(Random.Range(0, CurrentActivePiece.MaxHP+1));
       // Debug.Log("damage:" + DMG);

        //If this is the clean take version, just add tons of damage
        if (!HitPointVersion)
        {
            DMG = 1000000000;
        }

        //apply damage
        piece.HP -= DMG;

        string attackerName = (CurrentActivePiece.PieceType == "king" && CurrentActivePiece.PieceColor == "black") ? "King of Yendor" : CurrentActivePiece.PieceType;
        string attackedName = (piece.PieceType == "king" && piece.PieceColor == "black") ? "King of Yendor" : piece.PieceType;



        //Dead?
        if ((piece.HP < 1)||(piece.PieceColor=="red"))
        {
            if (CurrentActivePiece.human && !piece.human && (piece.PieceType != "coin"))
            {
                if (HitPointVersion)
                {
                    TempMessage = "Your " + attackerName + " defeated the " + attackedName;
                }
                else
                {
                    TempMessage = "Your " + attackerName + " captured the " + attackedName;
                }
            }
            if (!CurrentActivePiece.human && piece.human)
            {
                if (HitPointVersion)
                {
                    TempMessage = "The " + attackerName + " defeated your " + attackedName;
                }
                else
                {
                    TempMessage = "The " + attackerName + " captured your " + attackedName;
                }
            }
            int humanpieces = 0;
            if (CurrentActivePiece.human && (piece.PieceColor == "red") && piece.PieceType != "coin")
            {
                
                foreach(Piece pie in PieceList)
                {
                    if (pie.human)
                    {
                        humanpieces++;
                    }
                }
                if (humanpieces < 16)
                {
                    TempMessage = "You now have a new " + piece.PieceType + "!";
                }
                else
                {
                    TempMessage = "Your party is full";
                }
                
            }


            //was it the player's king?
            if ((piece.PieceType == "king"))
            {
                 
                //Debug.Log("game over");
                gameover = true;

                //StartCoroutine(GameOver());
            }
            /*if ((!piece.human) && (piece.PieceType == "king") && (piece.PieceColor != "red"))
            {
               // Debug.Log("game over");
                gameover = true;
                //Popup.gameObject.SetActive(true);
                
            }*/
            //Debug.Log("I am human? : " + CurrentActivePiece.human);
            // Debug.Log("Eaten piece is human ?: " + piece.human);
            if (piece.PieceColor == "red")
            {
                if (piece.PieceType != "coin")
                {
                    
                    if (humanpieces < 16){
                        piece.human = true;
                        piece.PowerUp();
                    }
                    
                    WaitingForMoveBack = true;
                    return ".";

                }
                else
                {
                    int gold = (int)(Random.value * 9) + 1;
                    TempMessage = "You found " + gold + " gold!";
                    Debug.Log("choguelo : " + PlayerPrefs.GetInt("Choguelo"));
                    PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + gold);
                    PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + gold);
                    Debug.Log("choguelo : " + PlayerPrefs.GetInt("Choguelo"));
                    PieceList.Remove(piece);
                    Destroy(piece.gameObject);

                    GetComponent<AudioSource>().clip = gulp;
                    GetComponent<AudioSource>().Play();
                }


                

                // WaitingForCPUMove = true;

            }
            else if (piece.human != CurrentActivePiece.human)
            {
                if (CurrentActivePiece.human)
                {
                    PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + piece.MaxHP);
                   
                }
                    
                /*  int gold = (int)(Random.value * 50);
                  TempMessage = "You found " + gold + " gold !";
                  PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold")+gold);*/

                PieceList.Remove(piece);
                Destroy(piece.gameObject);
                GetComponent<AudioSource>().clip = gulp;
                GetComponent<AudioSource>().Play();
                SavePartyState();
            }
            if (CurrentActivePiece.human)
            {
                if (piece.PieceColor == "black")
                {
                    PlayerPrefs.SetInt("hptaken", PlayerPrefs.GetInt("hptaken") + piece.MaxHP);
                    PlayerPrefs.SetInt("taken", PlayerPrefs.GetInt("taken") + 1);
                    if (PlayerPrefs.GetInt("taken") > PlayerPrefs.GetInt("maxtaken"))
                    {
                        PlayerPrefs.SetInt("maxtaken", PlayerPrefs.GetInt("taken"));
                    }
                }


            }

            return "x"; //will return * if partial hit in the HP branch
        }
        //not dead!
        else
        {
            string verb = "**ERROR**";

            Debug.Log("Arrays are length " + hits.Length + " and " + misses.Length);

            if (DMG == 0)
            {
                verb = misses[Random.Range(0, misses.Length)];
            }
            else
            {
                verb = hits[Random.Range(0, hits.Length)];
            }

            Debug.Log("Chose verb: " + verb);

            if (CurrentActivePiece.human)
            {
                TempMessage = "Your " + CurrentActivePiece.PieceType + " " + verb + " the " + piece.PieceType;
            }
            else
            {
                TempMessage = "The " + CurrentActivePiece.PieceType + " " + verb + " your " + piece.PieceType; ;
            }

            // Hard coding the special case of the king of yendor
            if (attackerName == "King of Yendor" && DMG == 0)
            {
                TempMessage = "The King of Yendor misses your " + piece.PieceType;
            }
            else if (attackerName == "King of Yendor" && DMG > 0)
            {
                TempMessage = "The King of Yendor hits your " + piece.PieceType;
            }
            else if (attackedName == "King of Yendor" && DMG == 0)
            {
                TempMessage = "Your " + CurrentActivePiece.PieceType + " misses the King of Yendor";
            }
            else if (attackedName == "King of Yendor" && DMG > 0)
            {
                TempMessage = "Your " + CurrentActivePiece.PieceType + " hits the King of Yendor";
            }

            //TempMessage = "Some damage but no kill";
            //you have to go back to where you came from!
            WaitingForMoveBack = true;
            GetComponent<AudioSource>().clip = tic;
            GetComponent<AudioSource>().Play();

            if (DMG == 0)
            {
                return "."; // Symbol for missing
            }
            else {
                return "*"; // Symbol for hitting and damaging
            }
            
        }
        
    }


    private string GenerateAttackMessage(Piece piece, string[] verbArray)
    {
        string verb = verbArray[Random.Range(0, verbArray.Length)];

        if (CurrentActivePiece.human)
        {
            return "Your " + CurrentActivePiece.PieceType + " " + verb + " the " + piece.PieceType;

        }
        else
        {
            return "The " + CurrentActivePiece.PieceType + " " + verb + " your " + piece.PieceType; ;
        }
    }

    private void SavePartyState()
    {
        string outgoingpieces = "";
        foreach (Piece piece in PieceList)
        {
            if (piece.PieceColor == "white")
            {
                if (piece.PieceType == "pawn")
                {
                    outgoingpieces = outgoingpieces + "p";
                }
                else if (piece.PieceType == "bishop")
                {
                    outgoingpieces = outgoingpieces + "b";
                }
                else if (piece.PieceType == "knight")
                {
                    outgoingpieces = outgoingpieces + "c";
                }
                else if (piece.PieceType == "rook")
                {
                    outgoingpieces = outgoingpieces + "t";
                }
                else if (piece.PieceType == "king")
                {
                    outgoingpieces = outgoingpieces + "k";
                }
                else if (piece.PieceType == "queen")
                {
                    outgoingpieces = outgoingpieces + "q";
                }
            }

        }
        PlayerPrefs.SetString("IncomingPieces", outgoingpieces);
    }

    private void PrepareNextLevel()
    {
        SavePartyState();

        if (PlayerPrefs.GetInt("continued") != 1)
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") - 1);
        }
        if (PlayerPrefs.GetInt("level") > PlayerPrefs.GetInt("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
        }
    }

    private void ChangeLevel()
    {

        PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + 50);
        PrepareNextLevel();
        if (PlayerPrefs.GetInt("level") == 0)
        {
            //player actually won
            // Remember that
            //Debug.Log("MainManager.ChangeLevel() setting grandmaster to 1");
            //PlayerPrefs.SetInt("grandmaster", 1);
            //// Update their rating
            //PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + 500);
            //// Submit their achievement
            //GameServices.Instance.SubmitAchievement(allAchievements[1], AchievementSUbmitted);
            //// Load the victory scene (which will submit the score itself)
            /// 
            PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + 500);
            if (PlayerPrefs.GetInt("grandmaster") == 0)
            {
                PlayerPrefs.SetInt("newgrandmaster", 1);
            }
            PlayerPrefs.SetInt("grandmaster", 1);
            SceneManager.LoadScene("LastLevel");
        }
        else
        {
            SceneManager.LoadScene("LevelGen");
        }
    }

    public IEnumerator GameOver()
    {
        
        
        /*yield return new WaitForSeconds(0.2f);
        GetComponent<AudioSource>().clip = endchord;
        GetComponent<AudioSource>().Play();*/
        //GameObject.Find("MainCamera").GetComponent<CameraMove>().CenterOnKing();
        yield return new WaitForSeconds(4f);
        PlayerPrefs.SetString("Executor", CurrentActivePiece.PieceType);
        SceneManager.LoadScene("GameOver");

    }
    public void  Win()
    {
        //DisplayMsg("You captured the king of Yendor. Will you make it back to the light of day?");
        PlayerPrefs.SetInt("Choguelo", PlayerPrefs.GetInt("Choguelo") + 500);
        PlayerPrefs.SetInt("continued", 1); //this means we got the king?
        PlayerPrefs.SetInt("kinglevel", PlayerPrefs.GetInt("level"));


        //Popup.gameObject.SetActive(true);

       // DisplayMsg("You captured the king of Yendor. Will you make it back to the light of day? ");
       /* WaitingForPlayerMove = false;
        WaitingForCPUMove = true;
        float endtime = Time.time+5;*/
        
        /*while (Time.time<endtime)
        {
            DisplayMsg("You captured the king of Yendor. Will you make it back to the light of day?");
            yield return new WaitForSeconds(0.01f);
        }*/
        gameover = false;
        
        
       // msgline.color = new Color(0xaa / 255, 0xaa / 255, 0xaa / 255);
        // DisplayMsg("");
      /*  WaitingForPlayerMove = true;
        WaitingForCPUMove = false;*/
        ChangeTurn();
        //PrepareNextLevel();
        //PlayerPrefs.SetString("Executor", CurrentActivePiece.PieceType);
        //SceneManager.LoadScene("Victory");

    }
    public void UpdateStatus()
    {
        int CurrentPieces = 0;
        foreach(Piece piece in PieceList)
        {
            if (piece.human)
            {
                CurrentPieces++;
            }
        }
        if (statusline != null)
        {
            statusline.text = " Level:" + PlayerPrefs.GetInt("level")+"     Hits:"+CurrentActivePiece.HP+"("+CurrentActivePiece.MaxHP+")"+"     Pieces:" + CurrentPieces + "     Captured:" + PlayerPrefs.GetInt("taken") + "     Gold:"+ PlayerPrefs.GetInt("gold");
        }

        
    }
    public void DisplayMsg(string msg)
    {
        if (msgline != null)
        {
            msgline.text = msg;
        }
    }
    TileType FindTileByPosition(Vector3 tilepos)

    {
       // Debug.Log("find tile by position");
        foreach(TileType tile in TileList)
        {
            if (tile.transform.position == tilepos)
            {
              //  Debug.Log("found tile");
                return tile;
            }
        }
       // Debug.Log("tile not found");
        return null;
    }
    void ChangeTurn()
    {
        //Debug.Log("changing turn");
        if (WaitingForCPUMove)
        {
           // Debug.Log("Player's turn");
            WaitingForCPUMove = false;
            PieceSelected = false;
            WaitingForPlayerMove = true;
        }
        else
        {
           // Debug.Log("CPU's turn");
            WaitingForCPUMove = true;
           
            WaitingForPlayerMove = false;
            CPUPlay("black");
        }


    }
    IEnumerator InitialStuff()
    {
        
        foreach (Piece piece in PieceList)
        {
            piece.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);


        UpdateThreats();

        UpdateVisibility();
        foreach (Piece piece in PieceList)
        {
            piece.SetActive(false);
        }



    }
    public void Resign()
    {
        if (resignbutton.text == "[Resign]")
        {
            resignbutton.text = "[Tap to confirm resignation]";
            StartCoroutine(ResetResignation());
        }
        else
        {
            PlayerPrefs.SetString("Executor", "resigned");
            SceneManager.LoadScene("GameOver");
        }
    }

    private IEnumerator ResetResignation()
    {
        yield return new WaitForSeconds(3f);
        resignbutton.text = "[Resign]";
    }

 

}
