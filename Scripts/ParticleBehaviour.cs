using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    public SimulationBehaviour simulation;
    public List<ParticleBehaviour> otherParticles = new List<ParticleBehaviour>();
    public TypeSettings typeSetting;
    private float force;
    private List<float> attractions = new List<float>();
    private int type = -1;
    private Rigidbody rb;
    public bool useMyPhysics = true;
    private ParticlePhysics physics;
    // Start is called before the first frame update

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        physics = GetComponent<ParticlePhysics>();

        if (typeSetting)
        {
            force = typeSetting.force;
            attractions = typeSetting.attractions;
            type = typeSetting.type;
            GetComponent<Renderer>().material.color = typeSetting.color;
        }

        if (otherParticles.Count > 0)
        {
            otherParticles = simulation.particles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (attractions != simulation.types[type].attractions) attractions = simulation.types[type].attractions;
        if (useMyPhysics) UpdateVelocity();
        else ApplyForce();
        if (simulation.worldSize < physics.GetDistanceToPoint(physics.zeroVector)) BorderMovement(simulation.worldSize);
    }

    private void ApplyForce()
    {
        foreach (ParticleBehaviour other in otherParticles)
        {
            if (other != this)
            {
                Vector3 vector = other.transform.position - gameObject.transform.position;
                float attraction = attractions[other.type];
                vector = ApplyDistanceFormula(vector.magnitude) * force * (vector / vector.magnitude) * attraction;
                rb.AddForce(vector);
            }
        }
    }

    private void UpdateVelocity()
    {
       foreach (ParticleBehaviour other in otherParticles)
        {
            if (other != this)
            {
                ParticlePhysics physics2 = other.GetComponent<ParticlePhysics>();
                List<float> vector = physics.GetDirTo(physics2);
                float attraction = attractions[other.type];
                float lenght = physics.GetVectorLenght(vector);
                vector = physics.MultiplyVectorWithScalar(vector, ApplyDistanceFormula(lenght) * force * attraction);
                physics.AddVelocity(vector);
            }
        } 
    }

    private float ApplyDistanceFormula(float x)
    {
        return (simulation.worldForce / x) - (simulation.worldForce / Mathf.Pow(x,2));
    }

    private void BorderMovement(float r)
    {
        List<float> vector = physics.MultiplyVectorWithScalar(physics.position, -0.9f);
        physics.position = vector;
    }
}
