using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePosition
{
    public Zone zone;
    public int index;

    public TablePosition(Zone zone, int index)
    {
        this.zone = zone;
        this.index = index;
    }
}
