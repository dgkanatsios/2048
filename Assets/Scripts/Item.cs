using UnityEngine;
using System.Collections;

public class Item
{
    public int Value { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public GameObject GO { get; set; }
    public bool WasJustDuplicated { get; set; }
}