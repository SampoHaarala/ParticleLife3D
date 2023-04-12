
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysics : MonoBehaviour
{
    public ParticlePhysicsControl control;
    public List<float> position = new List<float>();
    public List<float> velocity = new List<float>();
    private float velocityMultip = 1f; // controls physical move speed.
    private int timeIndex = -1;
    public List<float> zeroVector = new List<float>();

    // TODO: Tee switch funktio dimensionSettings setupille.

    // Start is called before the first frame update
    void Start()
    {
        velocityMultip = control.updatePhysicsScale;
        
        print("Velocity: ");
        if (control)
        {
            for (int i = 0; i < control.renderedDimensionCount + control.extraDimensionCount; i++)
            {
                velocity.Add(0);
                print(velocity[i]);
            }
        }
        position = velocity;
        switch (position.Count)
        {
            case 1:
                position[0] = gameObject.transform.position.x;
            break;
            case 2:
                position[0] = gameObject.transform.position.x;
                position[1] = gameObject.transform.position.y;
            break;
            case 3:
                position[0] = gameObject.transform.position.x;
                position[1] = gameObject.transform.position.y;
                position[2] = gameObject.transform.position.z;
            break;
            default:
            break;
        }
        zeroVector = velocity;
        if (timeIndex != -1) velocity[timeIndex] = control.moveTimePerTick;
    }

    public void UpdateVelocityMultip(float update)
    {
        velocityMultip = update * velocityMultip;
    }

    public void AddVelocity(List<float> update)
    {
        for (int i = 0; i < velocity.Count; i++)
        {
            velocity[i] += update[i];
        }
    }

    public Vector3 GetRenderedVelocity()
    {
        switch (control.renderedDimensionCount)
        {
            case 1: return new Vector3 (velocity[0],0,0);
            case 2: return new Vector3 (velocity[0],velocity[1],0);
            case 3: return new Vector3 (velocity[0],velocity[1], velocity[3]);
            default: return Vector3.zero;
        }
    }

    public List<float> ClosestPointOnSphere()
    {
        float lenght = GetVectorLenght(position);
        List<float> dir = DivideBy(lenght, position);
        return MultiplyVectorWithScalar(dir, lenght);
    }

    public List<float> GetDirTo(ParticlePhysics particle2)
    {
        float lenghtVector = 0f;
        List<float> dirVector = new List<float>();
        for (int i = 0; i < particle2.position.Count; i++)
        {
            float x = particle2.position[i] - position[i];
            dirVector.Add(x);
            lenghtVector += Mathf.Pow(x, 2);
        }
        lenghtVector = Mathf.Sqrt(lenghtVector);

        float lenghtPoint = 0f;
        List<float> dirPoint = new List<float>();
        List<float> pointOnSphere = GetInteractionPointOnSphere(position, particle2.position);
        dirPoint = SubtractVectors(pointOnSphere, position);

        lenghtPoint = GetVectorLenght(dirPoint);

        if (lenghtPoint < lenghtVector)
        {
            for (int i = 0; i < dirPoint.Count; i++)
            {
                dirPoint[i] = dirPoint[i] / lenghtPoint;
            }
            return dirPoint;
        }
        else
        {
            for (int i = 0; i < dirVector.Count; i++)
            {
                dirVector[i] = dirVector[i] / lenghtVector;
            }
            return dirVector;
        }
    }

    public List<float> GetInteractionPointOnSphere(List<float> point1, List<float> point2)
    {
        List<float> pointsVector = new List<float>();
        List<float> dirVector = NormalizeVector(pointsVector);
        float a = 0f;
        float b = 0f;
        float c = 0f;
        for (int i = 0; i < dirVector.Count; i++)
        {
            pointsVector.Add(point1[i] - point2[i]);
            a += Mathf.Pow(pointsVector[i], 2);
            b += point2[i] * pointsVector[i];
            c += Mathf.Pow(point2[i], 2);
        }
        b = b * 2;
        c -= control.simulation.worldSize;
        float t1 = -b + Mathf.Sqrt(Mathf.Pow(b,2) - 4 * a * c)/2 * a;
        float t2 = -b + Mathf.Sqrt(Mathf.Pow(b,2) - 4 * a * c)/2 * a;

        List<float> newPoint1 = new List<float>();
        List<float> newPoint2 = new List<float>();
        for (int i = 0; i < dirVector.Count; i++)
        {
            newPoint1.Add(point2[i] + t1 * pointsVector[i]);
            newPoint2.Add(point2[i] + t2 * pointsVector[i]);
        }
        float d1 = GetDistanceToPoint(SubtractVectors(newPoint1, point1));
        float d2 = GetDistanceToPoint(SubtractVectors(newPoint2, point1));
        if (d1 < d2) return newPoint1;
        else return newPoint2;
    }

    public List<float> NormalizeVector(List<float> vector)
    {
        float d = GetVectorLenght(vector);
        for (int i = 0; i < vector.Count; i++)
        {
            vector[i] = vector[i] / d;
        }
        return vector;
    }

    public float GetVectorLenght(List<float> vector)
    {
        float lenght = 0f;
        foreach (float f in vector)
        {
            lenght += f;
        }
        lenght = Mathf.Sqrt(lenght);
        return lenght;
    }

    public float GetDistanceToPoint(List<float> point)
    {
        List<float> dir = SubtractVectors(point, position);
        return GetVectorLenght(dir);
    }

    public List<float> DivideBy(float divider, List<float> vector)
    {
        for (int i = 0; i < vector.Count; i++)
        {
            vector[i] = vector[i] / divider;
        }
        return vector;
    }

    public List<float> MultiplyVectorWithScalar(List<float> vector, float scalar)
    {
        for (int i = 0; i < velocity.Count; i++)
        {
            print(i);
            vector[i] = vector[i] * scalar;
        }
        return vector;
    }

    public List<float> SubtractVectors(List<float> vector1, List<float> vector2)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < vector1.Count; i++)
        {
            result.Add(vector1[i] - vector2[i]);
        }
        return result;
    }

    public List<float> SumVectors(List<float> vector1, List<float> vector2)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < vector1.Count; i++)
        {
            result.Add(vector1[i] + vector2[i]);
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        velocityMultip = control.simulation.worldForce;
        gameObject.transform.position += GetRenderedVelocity();
    }
}
