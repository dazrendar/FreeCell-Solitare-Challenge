# FreeCell-Solitare-Challenge

This build is the main game screen, without any splash pages. It deals the deck randomly and allows the user to click the card on to put it into the free cell spaces, or click and drag around the screen. 

I am not particularly stuck on any one feature; just ran out of time. As for the JSON i/o, I suppose it could be useful for exporting and importing user statistics (e.g., "current global top score"). But, it is not (yet!) in my implementation.

Edit: one thing I would change is that I used two data structs to keep track of where each card is: a list and the actual card object itself (a c# script treated as an object). I used this because I initially relied on the list to deal the cards out, based on data used to generate the card GameObjects. The difficulty is that, for instance, to click a card (so as to move it into a free cell), I have to get the RayCast hit from the card GameObject, retrieve which column that card is in (from the card c# script), then consult the list (containing the suit and number of the cards in that column). And then update *both* accordingly. Amalgamating these distinct structures would make implementation easier, and less prone to error. This could be done by replacing the list of <Suit, value> with the actual card (at worst by simply by looping through the list and creating a new one with the card objects, then destroying the old one). This might also make it easier to keep track of the card positions on the field (otherwise, math must be used to calculate the transform.position values of each card, which, when moving cards around, is difficult to keep track of and more prone to error).

Overall I had fun with this and look forward to finishing it over the next few days, when I have some spare time.

Thanks!
