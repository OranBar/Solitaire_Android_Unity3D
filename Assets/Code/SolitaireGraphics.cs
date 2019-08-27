using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolitaireGraphics
{
    public Vector2[] ComputePortraitPositions(int noOfColumns, int x_padding, int y_padding){
        Vector2[] tableuPortraitPositions = new Vector2[noOfColumns];
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        int spaceNeededForMargins = (noOfColumns+1)*x_padding;
        float cardWidth = (screenWidth - spaceNeededForMargins)/noOfColumns;
        float cardHeight = 0;

        for (int i = 0; i < noOfColumns; i++)
        {
            float x = (x_padding*i) + (cardWidth/2);
            float y =  (y_padding*2) + cardHeight;
            Vector2 cardPosition = new Vector2(x,y);
            tableuPortraitPositions[i] = cardPosition;
        }
        return tableuPortraitPositions;
    }
}
