## Hot takes on Chogue from what's done now (2017-08-17 12:22)

1. So hot take one is that since our playtest of 13 June we've not worked together on the game at all (10 weeks) and since I wrote the notes below as I thought about the current situation I haven't thought about the game at all (6 weeks).

2. Hot take two is that much of the obsession when I've thought about the game has focused heavily on the nature of the AI, think that's maybe true of J as well. A lot of thought going into how the AI should play in the space - which would be a non-issue in human-versus-human for example, BUT human-versus-human is non-roguelike.

3. Hot take three is that this game is a _hybridisation_ (rogue+chess=?) rather than a _remediation_ (e.g. sibilant snakelikes, breakout indies) and this affects the nature of design. Specifically on multiple occasions we've found ourselves pressed to answer the question of which ruleset/conventions we should follow for a specific type of event/interaction.

  - For example when thinking about turn taking, our first idea was to follow a Roguelike tradition of all moves effectively happening simultaneously. Part of the reason that works in Rogue is that the player is a singular entity and all the monsters are controlled by the AI. As such, the player's calculations only need to worry about how they figure the AI will move relative to their singular position. But in Chogue you're controlling a group of pieces with interrelationships and it turns out (perhaps obviously) that this makes (Roguelike-like) simultaneous moves unpalatable. Notably it leads to 'chess situations' that seem undesirable, like both revealing and making a capture on the same turn. This is also an outcome of chess moves and the ability of pieces to capture at an arbitrary distance. As such we fairly quickly 'retreated' to using standard chess turn-taking (any single piece on your turn). This quickly revealed much more interesting tactics and strategy. (Is a fear raised here that chess will in some sense be too dominant as a design paradigm? That it's been around for so long and is such a refined concept that the various things that arise from it are simply more interesting than those in Rogue, a much younger format?)

  - Another example of this is the AI planning algorithm quickly being very much based around standard chess tactics - again a dominance effect. It makes its plans based on traditional chess concepts - capture value, swaps, etc. We've shied away from thinking, for instance, about the more spatial concepts that a Roguelike probably encourages - notably since the AI is hypothetically preventing the player from reaching the exit, there are strategies and tactics more about restriction that could be important - such as a concept of controlling the board in key locations (though note this is also chesslike, just with different intentions?)

4. So hot take four is probably this question of the dominance of chess in the hybridisation. One way to look at that is negatively - as in are we ending up 'just' making a game of chess in a different permutation of the board, and does that modification turn out to not really alter the chessy nature of things all that much? Another way to look at this, though, is to see it as an design-experimental examination of different designs - and the discovery that chess is a dominant design? Does that make it a 'failed' game though? Like, do you actually bother to make the digital version? What will the full digital version illustrate?

5. Hot take five concerns the reason for making this game in the first place and the question of what's interesting about it. The opening idea was the game would be comic because of various forms of movement from chess being recontextualised into a new setting. Original idea focused on the idea of selecting a single chess piece as a 'class' and trying to beat the dungeon with it. e.g. playing as a horse and thus getting a feeling for the horse's abilities (jumping through walls was the quintessential example here), or e.g. playing as a pawn for the comedy value of just plunging into a wall and coming to a stop until a monster comes to kill you. None of these comic elements seem to come through in the version of the game we've pursued (again, effectively straight-up chess in a dungeon). Perhaps by heavily embracing the chess-ness of the game (in part perhaps because it's such an impressive and complete system?) we end up with a less interesting game? Or at least a much more 'serious' game that is intellectually competitive/serious in ways that are antithetical to the comedy we'd been imagining to begin with and are, perhaps, outside our wheelhouse as designers?

6. Hot take six might be the idea of reverting back to what was funny/entertaining about the idea in the first place - the positioning of chess movement in a traditional rogue context. To get that rogue context it might be necessary, for example, to lay over the top a more traditional narrative and setting? Treasure chests? Rooms? Secret doors? Narrative interstitials? NPCs? Shops? Weapons? Potions? Spells? I can already feel, as I list more and more qualities of Roguelikes, the chessy nature being drowned out though - the opposite problem?

7. So another big hot take: is the original comic idea even funny writ large? Writ interactive?


## Notes on the basic setup and initial thoughts on early code (Pippin, 2017-07-07 15:04)

I just set up the GitHub repo for the thousandth time (fourth or fifth time) since we have decided at least for now that this game is called _Chogue_. It's a pretty powerful word... good phonetics... strong and ridiculous... unapologetic... but also kind of lowers expectations? I think it's helpful right at the moment.

After we ran our first playtest we ended up establishing a lot of what we _think_ the game is going to be like at a very basic mechanical level. Namely:

- Some kind of single-screen dungeon shaped board (possible rooms and corridors, maybe just irregular space)
- An entry point and an exit door (to go deeper into the dungeons)
- White (the player) has some set number of points to spend on their 'team' (I believe we went for 11 plus the king maybe?) in any way they choose
- Black (the computer) has an escalating number of points to spend on their 'team' the deeper into the dungeon you go
  - It may go up one point per level to avoid terrifying leaps in power
  - Selection will most likely just be randomised completely - could be all pawns etc (this is our 'algorithmic' solution to many things at this point)
- Turn-taking is based on moving one piece each in alternation (e.g. chess)
  - Can choose any piece to move each turn
  - AI will prioritise specific outcomes (capturing, avoiding capture, etc.)
- Objective for white (player) is to get the King to the exit
  - Team respawns on each level, so you can sacrifice them (no permadeath)
- AI decision making is just a prioritised list based on the possible moves for all pieces:
  0. Capture when you win the exchange (could have a 10% chance of not doing this?)
  0. Random chance capture on swaps (to avoid being too predictable)
  0. Move or protect threatened pieces (prioritise on value if necessary)
  0. Threaten to capture a piece (without being captured or unprotecting your pieces)
  0. Make a random move (possible a move toward the King that doesn't get you threatened?)
- Game ending condition possibilities
  - Resignation (get to keep the points you have for leaderboard/hiscore)
  - Death (checkmate, stalemate) (maybe you lose the points? Or at least a penalty.)
  - Some actual ending at a specific dungeon depth where you find the opponent's King (maybe alone!) and checkmate it

---

Okay, so what about actually managing to implement this? Let me take a quick look at how the JavaScript chess engine I used for Best Chess handles making moves and checking for legality. Well here is the [README](https://github.com/jhlywa/chess.js/blob/master/README.md) for that engine. Pretty helpful in terms of the kinds of functions it uses to be able to run a game of chess, and we'll have many of the same concerns when we're creating the underlying engine in terms of what we'd need to check out. And of course the actual code for this engine is available on GitHub so one could use it to figure out representations its using... so let's see...

- You obviously need a way of representing colours and pieces (text for instance)
- You might need a string-based way of representing a position (FEN is used here, but it won't necessarily make sense for an irregular board? Though that said you could have a FEN equivalent for the maximum area possible for a dungeon, with the understanding there will be no pieces in illegal locations for that, OR with another indicator for a wall, legal entry squares, and the exit)
- There are some AMAZING looking 2D arrays of PIECE_OFFSETS, ATTACKS, RAYS, ... don't know what they mean
- The board itself is an array of size 128 (why is this, given a chess board has 64 locations?)
- Track the current turn
- Maintain a history of... moves? board positions?

Sadly chess.js uses bitwise operations that immediately confuse the shit out of me in order to calculate legal moves and captures for pieces. It looks very sophisticated. It could be that it's necessary for performance, or it could be that it's elegant and we don't need it? Could each of our pieces just brute force search all legal moves based on their movement style?

Note that you have to think about issues about moves that put you in check? Or would we have it such that you leave your king unprotected at your own risk? e.g. you move a piece out of the way and the your king just gets captured? That's not a legal move in chess, but it could be in Chogue?

chess.js is kind of 'simple' in part because of course no AI is required - it's all about determining and making legal moves. We need the additional property of an AI (simple though it may be) evaluating the resulting board positions of potential moves and deciding on a specific move to make, which means that a move cannot just reduced to its legality but also to an evaluation of consequent board positions.

So would a protocol for the AI be

- Collect all legal moves for all pieces
- Evaluate resulting positions
- Choose the most valuable move

This would require an evaluation function for board positions that is reflective of our basic AI behaviour. e.g. A board position where the AI is up points is preferred (e.g. it captured a piece), but there should be a penalty of course if in that position an AI piece of threatened without protection (it can/will be captured). One would need to work out the numbers to account for these different elements such that they reflected the behaviour. We'd also need to be able to evaluate board positions to the extent we can track things like - total value of pieces in play but also positional elements (attackers versus defenders for each piece).

The alternative is something like the AI choosing a random piece, evaluating each of its moves (is it a capture, can it be captured back), and doing this for each piece and, again, choosing the best move. STILL need to be able to calculate attackers and defenders in that case. So I guess a board position (for AI) is evaluated such that for each SQUARE you know

- Value of the piece on it (if any) and for which side
- Total number of pieces attacking this square (and value)
- Total number of pieces defending this square (and value)

(Would cumulative value of all pieces attacking and defending tell us the total story? At least enough to make a decision?)

To build up an evaluation of the board like this you'd need to basically evaluate every possible move for every piece in place and cumulatively build up a board with evaluations for each square? OR you build that once at the beginning, and then modify it as pieces move? (It feels like there could be performance stuff here, but maybe modern day computer shit is so fast it's no big deal?)

I guess you could do it backwards? For each square you could cast 'rays' outward in all legal directions (e.g. horizontal, diagonal, and horsagonal) and see if you hit a piece that can make that move. If so, the piece attacks/defends that square. That's a nested loop, but shouldn't be worse than roughly n^2? (E.g. per square you loop on squares - the subset this square can see... including walls etc.)

After than a square in this evaluation board would know:
- value and colour of the piece on the square if any
- set of attacking pieces and their value (including a total score)
- set of defending pieces and their value (including a total score)

And from that our AI could, I think, make its decisions? Or ... not? Fuck. I don't actually know again. All that typing!

Time to take a break.


## Notes from meeting of 2017-06-13 at The Warehouse

### What is it 'about'?

* The poetics/aesthetics of chess
* Narrative/drama/tension of chess
* The vertigo feeling of chess (possibilities)
* Movement and space

### Who are you?

* Play as the king? Play as any piece ('classes')?
* Play as a party with the king as the player's "real avatar" (the one whose death triggers loss)
  - Some number of points to create your party with (could take all pawns, two queens, etc.)

### Turn taking

* How do you work out the turn order?
* All pieces of one colour move, then the other color
  - All move at once? What about conflicts?
  - Move in random order? Rank order? Proximity to enemy?
* When it's the player's turn simplify their life by determining which piece they need to move 'now' and cycle through their pieces for them?
  - This reduces a certain level of strategy as it matters which piece you move when (only if players alternate moving one piece each as in chess)

### Space

* What happens when you reach an 'exit'
  - Go down in levels of dungeon
* Will have potential issues like corridor widths for pieces like bishops and knights
* Is the whole space visible the whole time? Fog of war? Spaces that extend beyond the screen or smaller 'puzzle-like' levels (as in Hoplite)?

### Game structure

* Is there an ending or infinite dungeons?
* There could be the enemy king on a specific level, like the 10th or 20th, and if you capture him you win
* Think about things like Michael Brough's 868-HACK which solves the replayability thing by encouraging people to make multiple runs without dying (all this only really matters if we're getting serious about leaderboards though)
