using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

/// <summary>
/// Simple one-click NavMesh baker for CikWick game
/// </summary>
public class NavMeshBaker : EditorWindow
{
    [MenuItem("CikWick/Bake NavMesh Now!")]
    public static void BakeNavMesh()
    {
        if (EditorUtility.DisplayDialog("Bake NavMesh",
            "This will bake the NavMesh for the Cat AI to navigate.\n\nContinue?",
            "Yes, Bake It!", "Cancel"))
        {
            Debug.Log("Starting NavMesh bake...");

            // Bake the NavMesh using Unity's built-in method
            UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
            UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

            Debug.Log("✓ NavMesh bake complete! Check the Scene view for blue overlay on walkable surfaces.");

            EditorUtility.DisplayDialog("Success!",
                "NavMesh has been baked!\n\n" +
                "You should now see a BLUE overlay on your floor surfaces in the Scene view.\n\n" +
                "Next steps:\n" +
                "1. Check Scene view for blue NavMesh overlay\n" +
                "2. Press Play to test your game!\n" +
                "3. The Cat should now patrol and chase!",
                "Awesome!");
        }
    }

    [MenuItem("CikWick/Mark All Floors as Walkable")]
    public static void MarkFloorsWalkable()
    {
        int count = 0;

        // Find all GameObjects with "Floor" or "Ground" in their name or tag
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("floor") ||
                obj.name.ToLower().Contains("ground") ||
                obj.name.ToLower().Contains("environment") ||
                obj.tag == "Ground" ||
                obj.tag == "Floor")
            {
                // Mark as Navigation Static
                GameObjectUtility.SetStaticEditorFlags(obj, StaticEditorFlags.NavigationStatic);

                count++;
                Debug.Log($"Marked {obj.name} as Navigation Static");
            }
        }

        if (count > 0)
        {
            EditorUtility.DisplayDialog("Floors Marked",
                $"Marked {count} floor objects as Navigation Static and Walkable.\n\n" +
                "Now use 'CikWick > Bake NavMesh Now!' to bake.",
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("No Floors Found",
                "Couldn't automatically find floor objects.\n\n" +
                "Please manually select your floor/ground objects and mark them as Navigation Static in the Inspector.",
                "OK");
        }
    }
}
