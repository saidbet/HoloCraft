using UnityEngine;

public static class Utility
{
    public static float Round(float nbr)
    {
        if (nbr > 0.5)
        {
            return 1;
        }
        else if (nbr <= 0.5 && nbr >= -0.5)
        {
            return 0;
        }
        else if (nbr < -0.5)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public static Vector3[] FindAdjacentPos(Vector3 position)
    {
        Vector3[] res = new Vector3[6];
        res[0] = new Vector3(position.x + 1, position.y, position.z);
        res[1] = new Vector3(position.x - 1, position.y, position.z);
        res[2] = new Vector3(position.x, position.y + 1, position.z);
        res[3] = new Vector3(position.x, position.y - 1, position.z);
        res[4] = new Vector3(position.x, position.y, position.z + 1);
        res[5] = new Vector3(position.x, position.y, position.z - 1);

        return res;
    }

    public static bool ToBool(float nbr)
    {
        if (nbr == 0)
            return false;
        else
            return true;
    }

    public static float GetPropValue(BlockPropertiesValues props, Properties property)
    {
        return props.properties.Find(prop => prop.property == property).value;
    }

    public static bool GetBoolValue(BlockPropertiesValues props, Properties property)
    {
        return ToBool(props.properties.Find(prop => prop.property == property).value);
    }
}
