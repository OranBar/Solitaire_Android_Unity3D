public enum Suit{
    Hearts=0,
    Diamonds=1,
    Clubs=2,
    Spades=3,
    None=4
}

static class SuitMethods
{
    public static SuitColor GetSuitColor(this Suit s1)
    {
        if(s1 == Suit.Hearts || s1 == Suit.Diamonds){
            return SuitColor.Red;
        }
        else if(s1 == Suit.Clubs || s1 == Suit.Spades){
            return SuitColor.Black;
        }

        throw new System.Exception("Can't identify suit color. Did you add Jollys?");
    }
    
}