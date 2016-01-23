using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public interface IInputDetector
    {
        InputDirection? DetectInputDirection();
    }

    public enum InputDirection
    {
        Left, Right, Top, Bottom
    }
}
