using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBehaviour : MonoBehaviour
{
    public Camera sceneCamera;
    public float worldForce = 1f;
    public float worldSize = 0;
    public int particleCount = 0;
    public List<ParticleBehaviour> particles = new List<ParticleBehaviour> ();
    public List<TypeSettings> types = new List<TypeSettings>();
    public List<int> particleAmounts = new List<int>();
    public GameObject particleTemplate;
    public ParticlePhysicsControl physics;
    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<ParticlePhysicsControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        if (particles.Count == 0) SpawnSquareFormation();
        else SetSquareFormation();
        
    }

    public void SetSquareFormation()
    {
        int i = 0;
        float pow = Mathf.Pow((float)particleCount, 1f / 3f);
        float distanceBetweenParticles = worldSize / pow;
        float spaceInEachDistance = worldSize / 2;
        float x = -spaceInEachDistance;
        float y = -spaceInEachDistance;
        float z = -spaceInEachDistance;
        
        foreach (ParticleBehaviour particle in particles)
        {
            particle.transform.position = new Vector3(x, y, z);
            
            x += distanceBetweenParticles;
            if (x >= spaceInEachDistance) {
                x = -spaceInEachDistance;;
                y += distanceBetweenParticles;
                if (y >= spaceInEachDistance) {
                    y = -spaceInEachDistance;
                    z += distanceBetweenParticles;
                }
            }
            i++;
        }
        UpdateParticleInteractions();
    }

    public void SpawnSquareFormation()
    {
        List<int> typeCount = new List<int>();
        for (int p = 0; p < particleAmounts.Count; p++)
        {
            typeCount.Add(0);
        }

        int i = 0;
        int currentType = 0;
        int maxType = particleAmounts.Count;
        float pow = Mathf.Pow((float)particleCount, 1f / 3f);
        float distanceBetweenParticles = worldSize / pow;
        float spaceInEachDistance = worldSize / 2;
        float x = -spaceInEachDistance;
        float y = -spaceInEachDistance;
        float z = -spaceInEachDistance;
        
        while (i < particleCount)
        {
            currentType = CheckIfTypeLimitHasBeenMet(typeCount, currentType);
            GameObject newParticle = Instantiate(particleTemplate, new Vector3(x, y, z), new Quaternion(0,0,0,0), gameObject.transform);
            ParticleBehaviour behaviour = newParticle.GetComponent<ParticleBehaviour>();
            behaviour.typeSetting = types[currentType];
            behaviour.simulation = this;
            particles.Add(behaviour);
            ParticlePhysics physics = newParticle.GetComponent<ParticlePhysics>();
            physics.control = this.physics;
            typeCount[currentType] += 1;
            currentType++;
            
            x += distanceBetweenParticles;
            if (x >= spaceInEachDistance) {
                x = -spaceInEachDistance;;
                y += distanceBetweenParticles;
                if (y >= spaceInEachDistance) {
                    y = -spaceInEachDistance;
                    z += distanceBetweenParticles;
                }
            }
            i++;
        }
        UpdateParticleInteractions();
    }

    public void UpdateTypeAttractions()
    {
        foreach (TypeSettings type in types)
        {
            if (type.attractions.Count < types.Count) while (type.attractions.Count < types.Count) type.attractions.Add(0);
            else if (type.attractions.Count > types.Count) while (type.attractions.Count > types.Count) type.attractions.RemoveAt(type.attractions.Count - 1);
        }
    }

    public void UpdateParticleCount()
    {
        int sum = 0;
        foreach (int count in particleAmounts)
        {
            sum += count;
        }
        particleCount = sum;
    }

    private void UpdateParticleInteractions()
    {
        foreach (ParticleBehaviour particle in particles)
        {
            particle.otherParticles = particles;
        }
    }

    int CheckIfTypeLimitHasBeenMet(List<int> typeCount, int currentType)
    {
        if (currentType > types.Count - 1) currentType = 0;
        if (typeCount[currentType] < particleAmounts[currentType]) return currentType;
        else
        {
            currentType++;
            return CheckIfTypeLimitHasBeenMet(typeCount, currentType);
        }
    }

    public void SetCameraPositionTop()
    {
        sceneCamera.transform.position = new Vector3(0,(3 / 2) * worldSize, 0);
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(90,0,0);
        sceneCamera.transform.rotation = rotation;
    }

    public void SetCameraPositionRight()
    {
        sceneCamera.transform.position = new Vector3((3 / 2) * worldSize, 0, 0);
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0,-90,0);
        sceneCamera.transform.rotation = rotation;
    }

    public void SetCameraPositionFront()
    {
        sceneCamera.transform.position = new Vector3(0, 0, (3 / 2) * worldSize);
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(180,0,0);
        sceneCamera.transform.rotation = rotation;
    }
}
