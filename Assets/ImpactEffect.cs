using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    private static Texture2D _particleTexture;
    private static Material _particleMaterial;

    public static void Spawn(Vector3 position)
    {
        var effectObject = new GameObject("explosion");
        effectObject.transform.position = position;

        var particleSystem = effectObject.AddComponent<ParticleSystem>();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = particleSystem.main;
        main.duration = 0.3f;
        main.loop = false;
        main.startLifetime = 0.25f;
        main.startSpeed = 2.5f;
        main.startSize = 0.2f;
        main.startColor = new Color(1f, 0.75f, 0.2f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 14) });

        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.02f;

        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(new Color(0.8f, 0.1f, 0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.1f));

        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial();

        particleSystem.Play();

        Destroy(effectObject, main.duration + main.startLifetime.constant);
    }

    private static Material GetParticleMaterial()
    {
        if (_particleMaterial != null)
        {
            return _particleMaterial;
        }

        _particleMaterial = new Material(Shader.Find("Sprites/Default"));
        _particleMaterial.mainTexture = GetParticleTexture();
        return _particleMaterial;
    }

    private static Texture2D GetParticleTexture()
    {
        if (_particleTexture != null)
        {
            return _particleTexture;
        }

        const int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1f - dist / maxDist);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        _particleTexture = texture;
        return texture;
    }
}
