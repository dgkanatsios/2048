using System;
using UnityEngine;

public static class Utilities
{
    public static string[,] GetMatrixFromResourcesData()
    {
        string[,] shapes = new string[Globals.Rows, Globals.Columns];

        TextAsset txt = Resources.Load("debugLevel") as TextAsset;
        string level = txt.text;

        string[] lines = level.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int row = 0; row < Globals.Rows; row++)
        {
            string[] items = lines[row].Split('|');
            for (int column = 0; column < Globals.Columns; column++)
            {
                shapes[row, column] = items[column];
            }
        }
        return shapes;

    }

    public static string ShowMatrixOnConsole(ItemArray matrix)
    {
        string x = string.Empty;
        for (int row = Globals.Rows - 1; row >= 0; row--)
        {
            for (int column = 0; column < Globals.Columns; column++)
            {
                if (matrix[row, column] != null)
                {
                    x += matrix[row, column].Value + "|";
                }
                else
                {
                    x += "X" + "|";
                }
            }
            x += Environment.NewLine;
        }
        Debug.Log(x);
        return x;
    }
}