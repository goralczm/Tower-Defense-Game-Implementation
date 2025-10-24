using UnityEngine;

public class Spinner
{
    private int _pointsCount;
    private float _speed;
    private float _radius;
    private float _angle;
    private bool _inverted;

    public Spinner()
    {

    }

    public Spinner(int pointsCount, float speed, float radius, bool inverted)
    {
        SetPointsCount(pointsCount);
        SetSpeed(speed);
        SetRadius(radius);
        SetInverted(inverted);
    }

    public int GetPointsCount()
    {
        return _pointsCount;
    }

    public void SetPointsCount(int pointsCount)
    {
        if (pointsCount < 0)
            pointsCount = 0;

        _pointsCount = pointsCount;
    }

    public void SetSpeed(float speed)
    {
        if (speed < 0)
            speed = 0;

        _speed = speed;
    }

    public void SetRadius(float radius)
    {
        if (radius < 0)
            radius = 0;

        _radius = radius;
    }

    public void SetInverted(bool inverted)
    {
        _inverted = inverted;
    }

    public void Update(float deltaTime)
    {
        _angle += _speed * deltaTime * (_inverted ? -1 : 1);
    }

    public Vector2[] GetAllPointsPositions(Vector2 center)
    {
        Vector2[] output = new Vector2[_pointsCount];

        float angleBetweenObjects = 2f * Mathf.PI / _pointsCount;
        for (int i = 0; i < _pointsCount; i++)
        {
            float objectAngle = i * angleBetweenObjects + _angle;
            Vector2 position = new Vector2(Mathf.Cos(objectAngle), Mathf.Sin(objectAngle)) * _radius;
            position += center;
            output[i] = position;
        }

        return output;
    }
}
