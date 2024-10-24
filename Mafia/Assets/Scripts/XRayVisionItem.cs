using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class XRayVisionItem : MonoBehaviour
{
    public float duration = 10f;
    public Color xrayColor = Color.red;
    public float xrayIntensity = 0.5f;
    public int minimumPlayerCount = 2;
    public float searchInterval = 0.5f;
    public string xrayShaderName = "Custom/XRayShader";

    private bool isActive = false;
    private List<PlayerXRay> playerXRays = new List<PlayerXRay>();

    void Start()
    {
        Debug.Log("XRayVisionItem: Start method called");
        StartCoroutine(InitializeWhenPlayersFound());
    }

    IEnumerator InitializeWhenPlayersFound()
    {
        while (true)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log($"XRayVisionItem: Found {players.Length} players with 'Player' tag");

            if (players.Length >= minimumPlayerCount)
            {
                InitializeXRayEffects(players);
                break;
            }

            yield return new WaitForSeconds(searchInterval);
        }

    }

    void InitializeXRayEffects(GameObject[] players)
    {
        Shader xrayShader = Shader.Find(xrayShaderName);
        if (xrayShader == null)
        {
            Debug.LogError($"XRayVisionItem: X-ray shader '{xrayShaderName}' not found!");
            return;
        }

        foreach (GameObject player in players)
        {
            Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length > 0)
            {
                PlayerXRay xray = new PlayerXRay(renderers, xrayShader, xrayColor, xrayIntensity);
                playerXRays.Add(xray);
                Debug.Log($"XRayVisionItem: Added X-ray effect for player {player.name}");
            }
            else
            {
                Debug.LogWarning($"XRayVisionItem: Player {player.name} does not have any Renderer components");
            }
        }

        Debug.Log($"XRayVisionItem: Created {playerXRays.Count} player X-ray effects");
    }

    void OnActivationButtonClick()
    {
        Debug.Log("XRayVisionItem: Activation button clicked");
        if (!isActive)
        {
            StartCoroutine(ActivateXRayVision());
        }
        else
        {
            DeactivateXRayVision();
        }
    }

    IEnumerator ActivateXRayVision()
    {
        isActive = true;
        Debug.Log("XRayVisionItem: Activating X-Ray vision");

        foreach (PlayerXRay xray in playerXRays)
        {
            xray.EnableXRayEffect(true);
        }
        Debug.Log($"XRayVisionItem: Enabled {playerXRays.Count} X-ray effects");

        yield return new WaitForSeconds(duration);

        DeactivateXRayVision();
    }

    void DeactivateXRayVision()
    {
        foreach (PlayerXRay xray in playerXRays)
        {
            xray.EnableXRayEffect(false);
        }
        Debug.Log($"XRayVisionItem: Disabled {playerXRays.Count} X-ray effects");

        isActive = false;
        Debug.Log("XRayVisionItem: X-Ray vision deactivated");
    }
}

public class PlayerXRay
{
    private Renderer[] renderers;
    private Material xrayMaterial;
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    public PlayerXRay(Renderer[] playerRenderers, Shader xrayShader, Color xrayColor, float xrayIntensity)
    {
        renderers = playerRenderers;
        xrayMaterial = new Material(xrayShader);
        xrayMaterial.SetColor("_XRayColor", xrayColor);
        xrayMaterial.SetFloat("_XRayIntensity", xrayIntensity);

        foreach (Renderer renderer in renderers)
        {
            originalMaterials[renderer] = renderer.materials;
        }
    }

    public void EnableXRayEffect(bool enable)
    {
        foreach (Renderer renderer in renderers)
        {
            if (enable)
            {
                Material[] materials = renderer.materials;
                System.Array.Resize(ref materials, materials.Length + 1);
                materials[materials.Length - 1] = xrayMaterial;
                renderer.materials = materials;
            }
            else
            {
                renderer.materials = originalMaterials[renderer];
            }
        }
    }
}