using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet planet;
    private Editor shapeEditor;
    private Editor colourEditor;

    [Header("Radius Settings")]
    public float minRadius = 1f;
    public float maxRadius = 20f;

    [Header("Noise Layer Settings")]
    public int minNoiseLayers = 1;
    public int maxNoiseLayers = 10;

    [Header("Biome Layer Settings")]
    public int minBiomeLayers = 1;
    public int maxBiomeLayers = 8;

    [Header("Tint Percent Settings")]
    public float minTintPercent = 0.0f;
    public float maxTintPercent = 1.0f;

    [Header("Noise Strength Settings")]
    public float minNoiseStrength = 0.001f;
    public float maxNoiseStrength = 1f;

    [Header("Simple Noise Settings")]
    public int minSimpleNoiseLayers = 1;
    public int maxSimpleNoiseLayers = 5;
    public float minPersistence = 0.9f;
    public float maxPersistence = 1.1f;
    public float minRoughness = 0.9f;
    public float maxRoughness = 1.1f;
    public float minBaseRoughness = 0.9f;
    public float maxBaseRoughness = 1.1f;
    public float minStrength = 0.01f;
    public float maxStrength = 1.05f;
    public float minValue = 0.01f;
    public float maxValue = 1.0f;

    [Header("Ridgid Noise Settings")]
    public float minWeightMultiplier = 0.01f;
    public float maxWeightMultiplier = 1.0f;

    private const string FolderPath = "Assets/Planets/";
    private const string DefaultFolderPath = "Assets/Defaults/";
    private const string DefaultShapeSettingsFile = "ShapeSettingsDefault.asset";
    private const string DefaultColourSettingsFile = "ColorSettingsDefault.asset";
    private bool start = false;
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            planet = (Planet)target;
            if (!start)
            {
                if (planet.shapeSettings != null && planet.colourSettings != null)
                {
                    planet.GeneratePlanet();
                    start = true;
                }
            }

            if (planet.shapeSettings == null || planet.colourSettings == null)
            {
                EnsureDefaultsExist();
                AssignDefaultSettings();
            }

            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        DrawConfigurableParameters();

        if (GUILayout.Button("Generate Planet Mesh"))
        {
            planet.GeneratePlanet();
        }

        if (GUILayout.Button("Random Planet"))
        {
            RandomizePlanet();
        }

        if (GUILayout.Button("Save Planet"))
        {
            SavePlanet();
        }

        if (GUILayout.Button("Load Planet"))
        {
            LoadPlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
    }

    private void DrawConfigurableParameters()
    {
        EditorGUILayout.LabelField("Randomization Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        minRadius = EditorGUILayout.FloatField("Min Radius", minRadius);
        maxRadius = EditorGUILayout.FloatField("Max Radius", maxRadius);
        minNoiseLayers = EditorGUILayout.IntField("Min Noise Layers", minNoiseLayers);
        maxNoiseLayers = EditorGUILayout.IntField("Max Noise Layers", maxNoiseLayers);
        minBiomeLayers = EditorGUILayout.IntField("Min Biome Layers", minBiomeLayers);
        maxBiomeLayers = EditorGUILayout.IntField("Max Biome Layers", maxBiomeLayers);
        minTintPercent = EditorGUILayout.FloatField("Min Tint Percent", minTintPercent);
        maxTintPercent = EditorGUILayout.FloatField("Max Tint Percent", maxTintPercent);

        minNoiseStrength = EditorGUILayout.FloatField("Min Noise Strength", minNoiseStrength);
        maxNoiseStrength = EditorGUILayout.FloatField("Max Noise Strength", maxNoiseStrength);

        minPersistence = EditorGUILayout.FloatField("Min Persistence", minPersistence);
        maxPersistence = EditorGUILayout.FloatField("Max Persistence", maxPersistence);
        minRoughness = EditorGUILayout.FloatField("Min Roughness", minRoughness);
        maxRoughness = EditorGUILayout.FloatField("Max Roughness", maxRoughness);
        minStrength = EditorGUILayout.FloatField("Min Strength", minStrength);
        maxStrength = EditorGUILayout.FloatField("Max Strength", maxStrength);
    }

    private void RandomizePlanet()
    {
        if (planet == null)
        {
            Debug.LogError("Planet is not initialized.");
            return;
        }

        planet.shapeSettings.planetRadius = Random.Range(minRadius, maxRadius);
        planet.shapeSettings.noiseLayers = GenerateRandomNoiseLayers();
        planet.colourSettings.biomeColourSettings.biomes = GenerateRandomBiomes();

        planet.GeneratePlanet();
    }

    private void LoadPlanet()
    {
        string shapePath = EditorUtility.OpenFilePanel("Load Shape Settings", FolderPath, "asset");
        string colourPath = EditorUtility.OpenFilePanel("Load Colour Settings", FolderPath, "asset");

        if (string.IsNullOrEmpty(shapePath) || string.IsNullOrEmpty(colourPath)) return;

        string relativeShapePath = GetRelativeAssetPath(shapePath);
        string relativeColourPath = GetRelativeAssetPath(colourPath);

        planet.shapeSettings = AssetDatabase.LoadAssetAtPath<ShapeSettings>(relativeShapePath);
        planet.colourSettings = AssetDatabase.LoadAssetAtPath<ColourSettings>(relativeColourPath);

        planet.GeneratePlanet();
        EditorUtility.SetDirty(planet);
        EditorUtility.SetDirty(planet.shapeSettings);
        EditorUtility.SetDirty(planet.colourSettings);

        Debug.Log("Planet settings loaded and applied.");
        planet.GeneratePlanet();
    }

    private string GetUniqueAssetPath(string baseName)
    {
        string path = Path.Combine(FolderPath, baseName);
        return AssetDatabase.GenerateUniqueAssetPath(path);
    }

    private string GetRelativeAssetPath(string absolutePath)
    {
        return "Assets" + absolutePath.Replace(Application.dataPath, "").Replace("\\", "/");
    }

    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        onSettingsUpdated?.Invoke();
                    }
                }
            }
        }
    }
    private ShapeSettings.NoiseLayer[] GenerateRandomNoiseLayers()
    {
        int noiseLayerCount = Random.Range(minNoiseLayers, maxNoiseLayers);
        var noiseLayers = new ShapeSettings.NoiseLayer[noiseLayerCount];

        for (int i = 0; i < noiseLayerCount; i++)
        {
            var noiseSettings = new NoiseSettings
            {
                filterType = Random.Range(0, 2) == 0 ? NoiseSettings.FilterType.Simple : NoiseSettings.FilterType.Ridgid
            };

            if (noiseSettings.filterType == NoiseSettings.FilterType.Simple)
            {
                noiseSettings.simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings
                {
                    numLayers = Random.Range(minSimpleNoiseLayers, maxSimpleNoiseLayers),
                    persistence = Random.Range(minPersistence, maxPersistence),
                    roughness = Random.Range(minRoughness, maxRoughness),
                    baseRoughness = Random.Range(minBaseRoughness, maxBaseRoughness),
                    strength = Random.Range(minStrength, maxStrength),
                    minValue = Random.Range(minValue, maxValue)
                };
            }
            else
            {
                noiseSettings.ridgidNoiseSettings = new NoiseSettings.RidgidNoiseSettings
                {
                    numLayers = Random.Range(minSimpleNoiseLayers, maxSimpleNoiseLayers),
                    persistence = Random.Range(minPersistence, maxPersistence),
                    roughness = Random.Range(minRoughness, maxRoughness),
                    baseRoughness = Random.Range(minBaseRoughness, maxBaseRoughness),
                    strength = Random.Range(minStrength, maxStrength),
                    minValue = Random.Range(minValue, maxValue),
                    weightMultiplier = Random.Range(minWeightMultiplier, maxWeightMultiplier)
                };
            }

            noiseLayers[i] = new ShapeSettings.NoiseLayer
            {
                useFirstLayerAsMask = Random.value > 0.5f,
                noiseSettings = noiseSettings
            };
        }

        return noiseLayers;
    }

    private ColourSettings.BiomeColourSettings.Biome[] GenerateRandomBiomes()
    {
        var biomeLayerCount = Random.Range(minBiomeLayers, maxBiomeLayers);
        var biomes = new ColourSettings.BiomeColourSettings.Biome[biomeLayerCount];

        for (var i = 0; i < biomeLayerCount; i++)
        {
            biomes[i] = new ColourSettings.BiomeColourSettings.Biome
            {
                tintPercent = Random.Range(minTintPercent, maxTintPercent),
                gradient = GenerateRandomGradient(),
                tint = Random.ColorHSV(),
                startHeight = Random.Range(0f, 1f)
            };
        }

        return biomes.OrderBy(b => b.startHeight).ToArray();
    }

    private Gradient GenerateRandomGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Random.ColorHSV(), 0f),
                new GradientColorKey(Random.ColorHSV(), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            });

        return gradient;
    }
    
    private void EnsureDefaultsExist()
    {
        // Crée le dossier par défaut s'il n'existe pas
        if (!Directory.Exists(DefaultFolderPath))
        {
            Directory.CreateDirectory(DefaultFolderPath);
        }

        // Crée ShapeSettingsDefault si nécessaire
        string shapePath = Path.Combine(DefaultFolderPath, DefaultShapeSettingsFile);
        if (!File.Exists(shapePath))
        {
            var defaultShapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
            AssetDatabase.CreateAsset(defaultShapeSettings, shapePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Default ShapeSettings created.");
        }

        // Crée ColorSettingsDefault si nécessaire
        string colourPath = Path.Combine(DefaultFolderPath, DefaultColourSettingsFile);
        if (!File.Exists(colourPath))
        {
            var defaultColourSettings = ScriptableObject.CreateInstance<ColourSettings>();
            AssetDatabase.CreateAsset(defaultColourSettings, colourPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Default ColourSettings created.");
        }
    }

    private void AssignDefaultSettings()
    {
        string shapePath = Path.Combine(DefaultFolderPath, DefaultShapeSettingsFile);
        string colourPath = Path.Combine(DefaultFolderPath, DefaultColourSettingsFile);

        planet.shapeSettings = AssetDatabase.LoadAssetAtPath<ShapeSettings>(shapePath);
        planet.colourSettings = AssetDatabase.LoadAssetAtPath<ColourSettings>(colourPath);

        EditorUtility.SetDirty(planet);
        Debug.Log("Default settings assigned to the planet.");
    }
    private void SavePlanet()
    {
        string folderPath = Path.Combine(Application.dataPath, "Planets");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string shapePath = GetUniqueAssetPath("ShapeSettings.asset");
        string colourPath = GetUniqueAssetPath("ColourSettings.asset");

        AssetDatabase.CreateAsset(Instantiate(planet.shapeSettings), shapePath);
        AssetDatabase.CreateAsset(Instantiate(planet.colourSettings), colourPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Planet settings saved to {shapePath} and {colourPath}");
        planet.GeneratePlanet();
    }
}
