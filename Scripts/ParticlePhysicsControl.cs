using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysicsControl : MonoBehaviour
{
    public SimulationBehaviour simulation;
    public int simulationSetting = 0;
    public int renderedDimensionCount = 3;
    public int extraDimensionCount = 0;
    public float worldForce = 1f;
    public float updatePhysicsScale = 1f;
    public float moveTimePerTick = 1f;
    // Start is called before the first frame update

    void Start()
    {
        simulation = GetComponent<SimulationBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
                                                                                                               
    }
}
