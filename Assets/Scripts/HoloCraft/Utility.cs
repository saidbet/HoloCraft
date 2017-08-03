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
}
