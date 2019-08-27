using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolitaireGraphics
{
    public Vector2 ComputeCardSize_ScreenSpace(int noOfColumns, int x_padding, int y_padding){
        int screenWidth = Camera.main.pixelWidth;
        int screenHeight = Camera.main.pixelHeight;

        int spaceNeededForMargins = (noOfColumns+1)*x_padding;

        float cardWidth = (screenWidth - spaceNeededForMargins) / noOfColumns;
        // float cardRatio = (2.5f / 3.5f); 
        // float cardHeight = cardWidth * cardRatio;
        // 2.5 x 3.5 inches is the standard size of poker cards
        float cardHeight = (cardWidth * 3.5f) / 2.5f;

        Vector2 suggestedCardSize = new Vector2(cardWidth, cardHeight);
        
        return suggestedCardSize;
    }

    public Vector2 ComputeCardSize_WorldSpace(int noOfColumns, int x_padding, int y_padding){
        var tmp = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        Vector2 suggestedCardSize = Camera.main.ScreenToWorldPoint(tmp) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        return suggestedCardSize;
    }

     public Vector2[] ComputePortraitPositions(int noOfColumns, int x_padding, int y_padding){
        Vector2[] tableuPortraitPositions = new Vector2[noOfColumns];
        Vector2 suggestedCardSize_sceenSpace = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        float cardWidth = suggestedCardSize_sceenSpace.x;
        float cardHeight = suggestedCardSize_sceenSpace.y;


        for (int i = 0; i < noOfColumns; i++)
        {
            float x = (x_padding*(i+1)) + (cardWidth*i) + (cardWidth/2);
            float y = ((y_padding*2) + cardHeight/2 + cardHeight);
            Vector2 cardPosition_screen = new Vector2(x, Camera.main.pixelHeight - y);
            Debug.Log(x+" "+y);

            Vector2 cardPosition_world = Camera.main.ScreenToWorldPoint(cardPosition_screen);
            tableuPortraitPositions[i] = cardPosition_world;
        }
        return tableuPortraitPositions;
    }
}
