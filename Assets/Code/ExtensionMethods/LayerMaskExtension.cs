using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
	public static IEnumerable<string> GetAllLayerNames()
	{
		var myLayerMask = new LayerMask();
		myLayerMask = -1;
		return myLayerMask.MaskToNames();
	}

	public static IEnumerable<int> GetAllLayersIds()
	{
		List<int> result = new List<int>();
		int layer1 = 1;
		result.Add(0);
		result.Add(1);
		for (int i = 2; i < 32; i++)
		{
			layer1 = layer1 << 1;
			if (LayerMask.LayerToName(i).IsNullOrEmpty() == false)
			{
				result.Add(i);
			}
		}
		return result;
	}

	public static LayerMask Create(params string[] layerNames)
    {
        return NamesToMask(layerNames);
    }

    public static LayerMask Create(params int[] layerNumbers)
    {
        return LayerNumbersToMask(layerNumbers);
    }

    public static LayerMask NamesToMask(params string[] layerNames)
    {
        LayerMask ret = (LayerMask)0;
        foreach (var name in layerNames)
        {
            ret |= (1 << LayerMask.NameToLayer(name));
        }
        return ret;
    }

    public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
    {
        LayerMask ret = (LayerMask)0;
        foreach (var layer in layerNumbers)
        {
            ret |= (1 << layer);
        }
        return ret;
    }

    public static LayerMask Inverse(this LayerMask original)
    {
        return ~original;
    }

    public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
    {
        return original | NamesToMask(layerNames);
    }

    public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
    {
        LayerMask invertedOriginal = ~original;
        return ~(invertedOriginal | NamesToMask(layerNames));
    }

    public static string[] MaskToNames(this LayerMask original)
    {
        var output = new List<string>();

        for (int i = 0; i < 32; ++i)
        {
            int shifted = 1 << i;
            if ((original & shifted) == shifted)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    output.Add(layerName);
                }
            }
        }
        return output.ToArray();
    }

    public static string MaskToString(this LayerMask original)
    {
        return MaskToString(original, ", ");
    }

    public static string MaskToString(this LayerMask original, string delimiter)
    {
        return string.Join(delimiter, MaskToNames(original));
    }

    public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
    {
        return ((mask.value & (1 << obj.layer)) > 0);
    }

	public static bool IsInLayerMask(this string layer, LayerMask mask)
	{
		return ((mask.value & (1 << (LayerMask.NameToLayer(layer)) )) > 0);
	}
}