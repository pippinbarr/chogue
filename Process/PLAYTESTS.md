

# 2017-06-16 09:37

- Jonathan as the player

* White moves all pieces at once

* Exits need to be on two colors if you have a rule of getting all pieces to the exit (for bishop)
* Pawns as more of an obstruction than a threat?
* Q: Do opponent pieces that hit the exit go through to the next level

* Q: Move order - who chooses order of pieces to move on their turn? (Has tactical importance because of captures...

* _Swaps_ are not actually a valuable strategy for the player - more important to maintain the party
* Move order strikes: a revealed capture that can then take place because you get to move all pieces (e.g. move one piece, reveals a capture, then make the capture)
* Need to think about how you can be tripped up by combinations of moves at once
* _Stalemate_? If the king can't move then he just doesn't move for that turn
* _Space_ - king backed up in a corner and can't get out
* Forks not as powerful because _both_ pieces can move
* Black will always capture... can't really sacrifice pieces as white?
* King generally speaking wants to move forward
* Not necessarily getting a sense it's easy to get captures...
* Turning into more of an avoidance based on capturing squares rather than much in the way of tactics?
* Is it always good to get a piece through the exit when you can?
* Bishops not on same colour as opponent is a specific dynamic


# 2017-06-16 09:58

* One piece moves at a time and alternating
* Same setup

* Being cramped is a major issue
* A lot more chessy - enables more chess tactics
* Try different ways of indicating who the next move


# 2017-06-16 10:06

* One piece at a time alternating sides
* Black moves in rank order from pawn up, and by proximity to the king within that
* White moves any piece

* Plays a lot faster (only thinking about a single piece at a time)
* Black lost almost instantly


# 2017-06-16 10:09

* One piece at a time alternating side -- e.g. chess

* AI: don't get eaten, eat if you can, threaten to eat, and protect
* AI: possible to move and break one of those rules for another piece... how would you check these things? (I guess you can propose a move and then check that the board doesn't contain a bad condition for one of your pieces?
* AI: go for check if you won't lose your piece

* Could have queening pawns - it seemed okay once
* AI should be trying to _kill_ the king not prevent it from doing things

# 2017-06-16 10:24

- Pippin as the player now

* New layout and pieces (see photo)
* Same rules as previous version (basically chess rules and AI with prioritised objectives)
  - Eat
  - Don't get eaten / protect
  - Threaten
  - Queen
  - Random

  * Thought: novelty levels: lots of pawns moving to queen, have to get them first
  * There is a feeling of opening - need to open a passage for pieces and the king to move
  * This thing of 'move the king' is very distinct
* Do you need to get the other pieces out?
  - Could be a score thing when you
* King has been fairly easy to move to the exit with a swap at the end

* Could build up a budget to buy pieces each level

* Maybe try black has 2x points of white

* Easy after you've thinned out black enough to just move the king through knowing it's not possible for the AI to prevent you (not least because that's not in its rules)
* Black shouldn't accept bad swaps because it's too easy to fool it into moves that remove its most powerful pieces

# 2017-06-16 10:43

* Same layout and power ratio, black will no longer make bad swaps
* Rules
  - Capture if it's equal or a win for you
  - Don't get captured or protect your piece
  - Threaten a capture (if it's valuable to you)
  - Random

* SHould you threaten captures that you wouldn't make?
* White has a vested interest in clearing the board enough to get out...
* Could have probabilistic thing for straight swaps for AI so they don't always do it? Might move away instead


# 2017-06-16 10:53

* Same game with +3 for black
* Random choice for protection

* This one has been much more tactical
* Pippin sacrificed his pieces to end up against a queen and a bishop thinking he'll be able to get by
* In the end Pippin was able to get by because the queen protected a pawn stupidly (if it hadn't been there it likely would have been a stalemate?)


# 2017-06-16 11:08

- Jonathan as white

* Black at 28 points
* Same rules

* Do we want a heuristic that random moves should take a piece toward the king?
* Trapping the queen into guarding/attacking some piece is interesting... king can slip by, a distraction

* Game lasted quite a while and ended in checkmate

* Being able to replay a level might be a nice - could be a chess puzzle as a side-line where you try out a level with different sets of characters

* Going up +3 is too much per level, +1 is probably better...?

* Choose your party at the very start of the dungeon and stick with them
* Generate names for your party
* Pawns as hostages < can threaten them and then move past while you occupy a bigger piece protecting them
