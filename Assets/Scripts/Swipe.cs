using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe
{
    private Vector2 swipeDirection;
    private float swipeTime;

    public Swipe() {
        swipeDirection = Vector2.zero;
        swipeTime = 0f;
    }

    public Swipe(Vector2 swipeDirection, float swipeTime) {
        this.swipeDirection = swipeDirection;
        this.swipeTime = swipeTime;
    }

    public Swipe(Vector2 swipeDirection, float fingerDownTime, float fingerUpTime) {
        this.swipeDirection = swipeDirection;
        swipeTime = CalculateSwipeTime(fingerDownTime, fingerUpTime);
    }

    public Swipe(float swipeTime, Vector2 fingerDownPos, Vector2 fingerUpPos) {
        this.swipeTime = swipeTime;
        swipeDirection = CalculateSwipeDirection(fingerDownPos, fingerUpPos);
    }

    public static float CalculateSwipeTime(float fingerDownTime, float fingerUpTime) {
        return fingerUpTime - fingerDownTime;
    }

    public static Vector2 CalculateSwipeDirection(Vector2 fingerDownPosition, Vector2 fingerUpPosition) {
        return fingerUpPosition - fingerDownPosition;
    }
}
