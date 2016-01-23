using Assets.Scripts;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemArray
{
    //the array, exposed only internally in the class
    private Item[,] matrix = new Item[Globals.Rows, Globals.Columns];

    //indexer for the class - https://msdn.microsoft.com/en-us/library/6x16t2tx.aspx
    public Item this[int row, int column]
    {
        get
        {
            return matrix[row, column];
        }
        set
        {
            matrix[row, column] = value;
        }
    }


    //searches for a random null column and returns it via output variables - https://msdn.microsoft.com/en-us/library/t3c3bfhx.aspx
    public void GetRandomRowColumn(out int row, out int column)
    {
        do
        {
            row = random.Next(0, Globals.Rows);
            column = random.Next(0, Globals.Columns);
        } while (matrix[row, column] != null);
    }

    public List<ItemMovementDetails> MoveHorizontal(HorizontalMovement horizontalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        //the relative column we will compare with
        //if swipe is left, we will compare with the previous one (the -1 position)
        int relativeColumn = horizontalMovement == HorizontalMovement.Left ? -1 : 1;
        //to get the column indexes, to do the loop below
        var columnNumbers = Enumerable.Range(0, Globals.Columns);

        //for left swipe, we will traverse the columns in the order 0,1,2,3
        //for right swipe, we want the reverse order
        if (horizontalMovement == HorizontalMovement.Right)
        {
            columnNumbers = columnNumbers.Reverse();
        }

        for (int row = Globals.Rows - 1; row >= 0; row--)
        {   //we're doing foreach instead of for in order to traverse the columns
            //in the appropriate order
            foreach (int column in columnNumbers)
            {
                //if the item is null, continue checking for non-null items
                if (matrix[row, column] == null) continue;

                //since we arrived here, we have a non-null item
                //first we check if this item has the same value as the previous one
                //previous one's position depends on whether the relativeColumn variable is -1 or 1, depending on the swipe
                ItemMovementDetails imd = AreTheseTwoItemsSame(row, column, row, column + relativeColumn);
                if (imd != null)
                {
                    //items have the same value, so they will be "merged"
                    movementDetails.Add(imd);
                    //continue the loop
                    //the new duplicated item may be moved on a subsequent loop
                    continue;
                }

                //matrix[row,column] is the first not null item
                //move it to the first null item space
                int columnFirstNullItem = -1;

                //again, this is to help on the foreach loop that follows
                //for a left swipe, we want to check the columns 0 to [column-1]
                //for a right swipe, we want to check columns [Globals.Columns-1] to column
                int numberOfItemsToTake = horizontalMovement == HorizontalMovement.Left
                ? column : Globals.Columns - column;

                bool emptyItemFound = false;

                //keeping it for documentation/clarity
                //this for loop would run for a left swipe ;)
                //for (columnFirstNullItem = 0; columnFirstNullItem < column; columnFirstNullItem++)
                foreach (var tempColumnFirstNullItem in columnNumbers.Take(numberOfItemsToTake))
                {
                    //keep a copy of the index on the potential null item position
                    columnFirstNullItem = tempColumnFirstNullItem;
                    if (matrix[row, columnFirstNullItem] == null)
                    {
                        emptyItemFound = true;
                        break;//exit the loop
                    }
                }

                //we did not find an empty/null item, so we cannot move current item
                if (!emptyItemFound)
                {
                    continue;
                }


                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne
                (row, row, row, column, columnFirstNullItem, columnFirstNullItem + relativeColumn);

                movementDetails.Add(newImd);


            }
        }
        return movementDetails;
    }



    public List<ItemMovementDetails> MoveVertical(VerticalMovement verticalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        int relativeRow = verticalMovement == VerticalMovement.Bottom ? -1 : 1;
        var rowNumbers = Enumerable.Range(0, Globals.Rows);

        if (verticalMovement == VerticalMovement.Top)
        {
            rowNumbers = rowNumbers.Reverse();
        }

        for (int column = 0; column < Globals.Columns; column++)
        {
            foreach (int row in rowNumbers)
            {
                //if the item is null, continue checking for non-null items
                if (matrix[row, column] == null) continue;

                //we have a non-null item
                //first we check if this item has the same value as the next one
                ItemMovementDetails imd = AreTheseTwoItemsSame(row, column, row + relativeRow, column);
                if (imd != null)
                {
                    movementDetails.Add(imd);

                    continue;
                }

                //matrix[row,column] is the first not null item
                //move it to the first null item
                int rowFirstNullItem = -1;

                int numberOfItemsToTake = verticalMovement == VerticalMovement.Bottom
                ? row : Globals.Rows - row;


                bool emptyItemFound = false;

                foreach (var tempRowFirstNullItem in rowNumbers.Take(numberOfItemsToTake))
                {
                    rowFirstNullItem = tempRowFirstNullItem;
                    if (matrix[rowFirstNullItem, column] == null)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

                if (!emptyItemFound)
                {
                    continue;
                }

                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne(row, rowFirstNullItem, rowFirstNullItem + relativeRow, column, column, column);

                movementDetails.Add(newImd);
            }
        }
        return movementDetails;
    }

    private ItemMovementDetails MoveItemToNullPositionAndCheckIfSameWithNextOne
(int oldRow, int newRow, int itemToCheckRow, int oldColumn, int newColumn, int itemToCheckColumn)
    {
        //we found a null item, so we attempt the switch ;)
        //bring the first not null item to the position of the first null one
        matrix[newRow, newColumn] = matrix[oldRow, oldColumn];
        matrix[oldRow, oldColumn] = null;

        //check if we have the same value as the left one
        ItemMovementDetails imd2 = AreTheseTwoItemsSame(newRow, newColumn, itemToCheckRow,
            itemToCheckColumn);
        if (imd2 != null)//we have, so add the item returned by the method
        {
            return imd2;
        }
        else//they are not the same, so we'll just animate the current item to its new position
        {
            return
                new ItemMovementDetails(newRow, newColumn, matrix[newRow, newColumn].GO, null);

        }
    }

    private ItemMovementDetails AreTheseTwoItemsSame(
        int originalRow, int originalColumn, int toCheckRow, int toCheckColumn)
    {
        if (toCheckRow < 0 || toCheckColumn < 0 || toCheckRow >= Globals.Rows || toCheckColumn >= Globals.Columns)
            return null;


        if (matrix[originalRow, originalColumn] != null && matrix[toCheckRow, toCheckColumn] != null
                && matrix[originalRow, originalColumn].Value == matrix[toCheckRow, toCheckColumn].Value
                && !matrix[toCheckRow, toCheckColumn].WasJustDuplicated)
        {
            //double the value, since the item is duplicated
            matrix[toCheckRow, toCheckColumn].Value *= 2;
            matrix[toCheckRow, toCheckColumn].WasJustDuplicated = true;
            //make a copy of the gameobject to be moved and then disappear
            var GOToAnimateScaleCopy = matrix[originalRow, originalColumn].GO;
            //remove this item from the array
            matrix[originalRow, originalColumn] = null;
            return new ItemMovementDetails(toCheckRow, toCheckColumn, matrix[toCheckRow, toCheckColumn].GO, GOToAnimateScaleCopy);

        }
        else
        {
            return null;
        }
    }

    private void ResetWasJustDuplicatedValues()
    {
        for (int row = 0; row < Globals.Rows; row++)
            for (int column = 0; column < Globals.Columns; column++)
            {
                if (matrix[row, column] != null && matrix[row, column].WasJustDuplicated)
                    matrix[row, column].WasJustDuplicated = false;
            }
    }

    private System.Random random = new System.Random();
}

public enum HorizontalMovement { Left, Right };
public enum VerticalMovement { Top, Bottom };
