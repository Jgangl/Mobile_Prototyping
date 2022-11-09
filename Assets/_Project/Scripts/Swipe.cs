using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe
{
    private Vector2 swipeDirection;
    private float swipeDuration;
    private float swipeLength;

    public Swipe() {
        swipeDirection = Vector2.zero;
        swipeDuration = 0f;
        swipeLength = 0f;
    }

    public Swipe(float swipeTime, Vector2 fingerDownPos, Vector2 fingerUpPos) {
        this.swipeDuration = swipeTime;
        swipeDirection = CalculateSwipeDirection(fingerDownPos, fingerUpPos);
        swipeLength = CalculateSwipeLength(fingerDownPos, fingerUpPos);
    }

    public static float CalculateSwipeDuration(float fingerDownTime, float fingerUpTime) {
        return fingerUpTime - fingerDownTime;
    }

    public static Vector2 CalculateSwipeDirection(Vector2 fingerDownPosition, Vector2 fingerUpPosition) {
        return fingerUpPosition - fingerDownPosition;
    }

    public static float CalculateSwipeLength(Vector2 fingerDownPosition, Vector2 fingerUpPosition) {
        return Vector2.Distance(fingerUpPosition, fingerDownPosition);
    }

    public Vector2 GetDirection() {
        return swipeDirection;
    }

    public float GetDuration() {
        return swipeDuration;
    }

    public float GetLength() {
        return swipeLength;
    }
}
