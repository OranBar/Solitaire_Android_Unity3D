using UnityEngine;

public class TouchInputHelper{


    public static bool DoubleTapDetected(){
        bool result = false;
        float maxTimeWait = 1;
        float variancePosition = 1;

        if( Input.touchCount == 1  && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float deltaTime = Input.GetTouch (0).deltaTime;
            float deltaPositionLenght=Input.GetTouch (0).deltaPosition.magnitude;

            if ( deltaTime> 0 && deltaTime < maxTimeWait && deltaPositionLenght < variancePosition)
                result = true;                
        }
        return result;
    }
}