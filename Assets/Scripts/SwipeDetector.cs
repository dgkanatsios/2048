using UnityEngine;
using System;
using Assets.Scripts;

public enum State
{
    SwipeNotStarted,
    SwipeStarted
}

public class SwipeDetector : MonoBehaviour, IInputDetector
{

    private State state = State.SwipeNotStarted;
    private Vector2 startPoint;
    private DateTime timeSwipeStarted;
    private TimeSpan maxSwipeDuration = TimeSpan.FromSeconds(1);
    private TimeSpan minSwipeDuration = TimeSpan.FromMilliseconds(100);

    public InputDirection? DetectInputDirection()
    {
        if (state == State.SwipeNotStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                timeSwipeStarted = DateTime.Now;
                state = State.SwipeStarted;
                startPoint = Input.mousePosition;
            }
        }
        else if (state == State.SwipeStarted)
        {
            if (Input.GetMouseButtonUp(0))
            {
                TimeSpan timeDifference = DateTime.Now - timeSwipeStarted;
                if (timeDifference <= maxSwipeDuration && timeDifference >= minSwipeDuration)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    Vector2 differenceVector = mousePosition - startPoint;
                    float angle = Vector2.Angle(differenceVector, Vector2.right);
                    Vector3 cross = Vector3.Cross(differenceVector, Vector2.right);

                    if (cross.z > 0)
                        angle = 360 - angle;

                    state = State.SwipeNotStarted;

                    if ((angle >= 315 && angle < 360) || (angle >= 0 && angle <= 45))
                        return InputDirection.Right;
                    else if (angle > 45 && angle <= 135)
                        return InputDirection.Top;
                    else if (angle > 135 && angle <= 225)
                        return InputDirection.Left;
                    else
                        return InputDirection.Bottom;
                }
            }
        }
        return null;
    }

}
