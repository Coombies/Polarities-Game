using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSizeController : MonoBehaviour
{
    // Define specific sizes for pixel-perfect stars
    public float[] starSizes = { 1f, 2f, 3f }; // Add your specific sizes here

    private new ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Hook into the Particle System's event when particles are emitted
        var main = particleSystem.main;
        main.startSize = 1;  // Set a default, doesn't matter, script will override
    }

    void Update()
    {
        // Modify particle size when emitted
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (particles[i].remainingLifetime == particles[i].startLifetime)  // This ensures it runs once when the particle spawns
            {
                Debug.Log("Do stuff");
                particles[i].startSize = starSizes[Random.Range(0, starSizes.Length)];
            }
        }

        // Apply the modified particles back to the system
        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}
