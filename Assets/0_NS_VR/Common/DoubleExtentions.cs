using System;

public static class DoubleExtentions
{
    public static string ToBigNumString(this double current, int minTani = 1)
    {
        if (double.IsInfinity(current) || double.IsNaN(current))
        {
            return "Infinity";
        }
        var baseCurrent = current;
        //0～1未満の場合は1とする
        if (current > 0 && current < 1) return "1";


        string[] str = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td" };
        int tani = 0;

        while (current >= 1000)
        {
            current /= 1000;
            tani++;
        }
        if (tani < str.Length)
        {
            if (tani < minTani)
            {
                baseCurrent = Math.Floor(baseCurrent);
                return baseCurrent.ToString("F0"); // 整数の場合、小数点以下を表示しない
            }
            else
            {
                current = Math.Floor(current * 10) / 10;
                return current.ToString("F1") + str[tani]; // 小数点以下1桁までを表示
            }
        }
        else
        {
            return current.ToString("F1") + "e+" + (3 * tani); // 指数表記
        }
    }
}