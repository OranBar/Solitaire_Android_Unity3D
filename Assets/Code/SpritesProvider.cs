using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesProvider 
{
    private static Sprite Load_Blank_Sprite(){
        return Resources.Load<Sprite>("Blank_Square");
    }

    public static Sprite LoadSuitSprite(Suit suit){
        switch(suit){
            case Suit.Hearts:
                return Resources.Load<Sprite>("suits/cuori");
            case Suit.Diamonds:
                return Resources.Load<Sprite>("suits/quadri");
            case Suit.Clubs:
                return Resources.Load<Sprite>("suits/fiori");
            case Suit.Spades:
                return Resources.Load<Sprite>("suits/picche");
            default:
                return Load_Blank_Sprite();
        }
    }

    public static Sprite LoadValueSprite(int value){
        Debug.Assert(value>=-1 && value<14);
        string spriteName;

        switch(value){
            case -1: return Load_Blank_Sprite();

            case 1: spriteName = "A"; break;
            case 11: spriteName = "J"; break;
            case 12: spriteName = "Q"; break;
            case 13: spriteName = "K"; break;
            default: spriteName = value.ToString(); break;
        }
        
        return Resources.Load<Sprite>("values/"+spriteName);
    }
}
