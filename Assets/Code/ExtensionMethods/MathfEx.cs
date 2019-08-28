using UnityEngine;

public static class MathfEx
{
    //Rounds all the floats in the Vector3 at the decimal parameter
    public static Vector3 Round(Vector3 v, float multiply = 1)
    {
        v.x = Mathf.Round(v.x / multiply) * multiply;
        v.y = Mathf.Round(v.y / multiply) * multiply;
        v.z = Mathf.Round(v.z / multiply) * multiply;
        return v;
    }
    //Floors all the floats in the Vector3 at the decimal parameter
    public static Vector3 Floor(Vector3 v, float multiply = 1)
    {
        v.x = Mathf.Floor(v.x / multiply) * multiply;
        v.y = Mathf.Floor(v.y / multiply) * multiply;
        v.z = Mathf.Floor(v.z / multiply) * multiply;
        return v;
    }
    //Ceils all the floats in the Vector3 at the decimal parameter
    public static Vector3 Ceil(Vector3 v, float multiply = 1)
    {
        v.x = Mathf.Ceil(v.x / multiply) * multiply;
        v.y = Mathf.Ceil(v.y / multiply) * multiply;
        v.z = Mathf.Ceil(v.z / multiply) * multiply;
        return v;
    }
    //Clamps all the floats in the Vector2 with min-max vectors
    public static Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max)
    {
        v.x = Mathf.Clamp(v.x, min.x, max.x);
        v.y = Mathf.Clamp(v.y, min.y, max.y);
        return v;
    }
    //Clamps all the floats in the Vector3 with min-max vectors
    public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
    {
        v.x = Mathf.Clamp(v.x, min.x, max.x);
        v.y = Mathf.Clamp(v.y, min.y, max.y);
        v.z = Mathf.Clamp(v.z, min.z, max.z);
        return v;
    }
    //Returns the Abs version of Vector2 v
    public static Vector2 Abs(Vector2 v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        return v;
    }
    //Returns the Abs version of Vector3 v
    public static Vector3 Abs(Vector3 v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        v.z = Mathf.Abs(v.z);
        return v;
    }
    //Rounds the float in at the decimal parameter
    public static float Round(float f, float multiply = 1)
    {
        return Mathf.Round(f / multiply) * multiply;
    }
    //Floors the float in at the decimal parameter
    public static float Floor(float f, float multiply = 1)
    {
        return Mathf.Floor(f / multiply) * multiply;
    }
    //Ceils the float in at the decimal parameter
    public static float Ceil(float f, float multiply = 1)
    {
        return Mathf.Ceil(f / multiply) * multiply;
    }

    //Rounds the float at the passed step multiplier
    public static float RoundOff(this float i, float multiply = 1)
    {
        return ((float)Mathf.Round(i / multiply)) * multiply;
    }
    //Floors the float at the passed step multiplier
    public static float FloorOff(this float i, float multiply = 1)
    {
        return ((float)Mathf.Floor(i / multiply)) * multiply;
    }
    //Ceil the float at the passed step multiplier
    public static float CeilOff(this float i, float multiply = 1)
    {
        return ((float)Mathf.Ceil(i / multiply)) * multiply;
    }

}
