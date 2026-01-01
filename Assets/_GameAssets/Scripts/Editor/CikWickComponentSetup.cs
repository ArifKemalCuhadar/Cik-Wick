using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Editor tool to automatically setup required Unity components for CikWick game
/// This ensures all GameObjects have the necessary Rigidbodies, Colliders, NavMesh components, etc.
/// </summary>
public class CikWickComponentSetup : EditorWindow
{
    private GameObject selectedObject;
    private Vector2 scrollPosition;

    [MenuItem("CikWick/Component Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<CikWickComponentSetup>("CikWick Component Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("CikWick Component Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This tool automatically adds and configures required Unity components for CikWick game objects.", MessageType.Info);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Player Setup Section
        DrawSectionHeader("Player Setup");
        if (GUILayout.Button("Setup Player Components", GUILayout.Height(30)))
        {
            SetupPlayerComponents();
        }
        EditorGUILayout.Space();

        // Cat AI Setup Section
        DrawSectionHeader("Cat AI Setup");
        if (GUILayout.Button("Setup Cat Components", GUILayout.Height(30)))
        {
            SetupCatComponents();
        }
        EditorGUILayout.Space();

        // Collectibles Setup Section
        DrawSectionHeader("Collectibles Setup");
        if (GUILayout.Button("Setup All Collectibles", GUILayout.Height(30)))
        {
            SetupAllCollectibles();
        }
        EditorGUILayout.Space();

        // Damageables Setup Section
        DrawSectionHeader("Damageables Setup");
        if (GUILayout.Button("Setup All Damageables", GUILayout.Height(30)))
        {
            SetupAllDamageables();
        }
        EditorGUILayout.Space();

        // Booster Setup Section
        DrawSectionHeader("Booster Setup");
        if (GUILayout.Button("Setup Spatula Booster", GUILayout.Height(30)))
        {
            SetupSpatulaBooster();
        }
        EditorGUILayout.Space();

        // Counter Triggers Setup Section
        DrawSectionHeader("Counter Triggers Setup");
        if (GUILayout.Button("Setup Counter Triggers", GUILayout.Height(30)))
        {
            SetupCounterTriggers();
        }
        EditorGUILayout.Space();

        // Complete Setup Section
        DrawSectionHeader("Complete Setup");
        EditorGUILayout.HelpBox("This will configure ALL components in the scene automatically.", MessageType.Warning);
        if (GUILayout.Button("Setup ALL Components (Complete Game)", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Complete Setup",
                "This will setup all components for the entire game. Continue?",
                "Yes", "Cancel"))
            {
                SetupAllComponents();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSectionHeader(string title)
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 12;
        headerStyle.normal.textColor = new Color(0.2f, 0.6f, 1f);
        EditorGUILayout.LabelField(title, headerStyle);
        EditorGUILayout.Space(5);
    }

    #region Player Setup

    private void SetupPlayerComponents()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Player GameObject not found! Make sure your player is tagged as 'Player' or named 'Player'.", "OK");
            return;
        }

        Undo.RecordObject(player, "Setup Player Components");

        // Add Rigidbody (REQUIRED by PlayerController)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = player.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody to Player");
        }

        // Configure Rigidbody
        rb.mass = 1f;
        rb.linearDamping = 0f; // Controlled by script
        rb.angularDamping = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true; // CRITICAL - prevents player from tipping over

        Debug.Log("Configured Player Rigidbody: mass=1, freezeRotation=true, interpolation=Interpolate");

        // Add Capsule Collider
        CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = player.AddComponent<CapsuleCollider>();
            capsule.radius = 0.5f;
            capsule.height = 2f;
            capsule.center = new Vector3(0, 1f, 0);
            Debug.Log("Added CapsuleCollider to Player");
        }

        // Check for child transforms
        Transform orientation = player.transform.Find("Orientation");
        if (orientation == null)
        {
            GameObject orientationObj = new GameObject("Orientation");
            orientationObj.transform.SetParent(player.transform);
            orientationObj.transform.localPosition = Vector3.zero;
            orientationObj.transform.localRotation = Quaternion.identity;
            Debug.Log("Created Orientation child object");
        }

        Transform playerVisual = player.transform.Find("PlayerVisual");
        if (playerVisual == null)
        {
            Debug.LogWarning("PlayerVisual child not found - you may need to create this manually with your player model");
        }

        EditorUtility.SetDirty(player);
        Debug.Log("✓ Player setup complete!");
        EditorUtility.DisplayDialog("Success", "Player components configured successfully!", "OK");
    }

    #endregion

    #region Cat Setup

    private void SetupCatComponents()
    {
        GameObject cat = GameObject.Find("Cat");
        if (cat == null)
        {
            EditorUtility.DisplayDialog("Error", "Cat GameObject not found! Make sure your cat is named 'Cat'.", "OK");
            return;
        }

        Undo.RecordObject(cat, "Setup Cat Components");

        // Add NavMeshAgent (REQUIRED by CatController)
        NavMeshAgent agent = cat.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = cat.AddComponent<NavMeshAgent>();
            Debug.Log("Added NavMeshAgent to Cat");
        }

        // Configure NavMeshAgent
        agent.speed = 5f; // Default speed (changed to 7 during chase)
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;
        agent.autoBraking = true;
        agent.radius = 0.5f;
        agent.height = 1f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        Debug.Log("Configured Cat NavMeshAgent: speed=5, radius=0.5, height=1");

        // Add Capsule Collider
        CapsuleCollider capsule = cat.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = cat.AddComponent<CapsuleCollider>();
            capsule.radius = 0.5f;
            capsule.height = 1f;
            capsule.center = new Vector3(0, 0.5f, 0);
            Debug.Log("Added CapsuleCollider to Cat");
        }

        // Add Animator if not present
        Animator animator = cat.GetComponent<Animator>();
        if (animator == null)
        {
            animator = cat.AddComponent<Animator>();
            Debug.LogWarning("Added Animator to Cat - you need to assign the AnimatorController!");
        }

        EditorUtility.SetDirty(cat);
        Debug.Log("✓ Cat setup complete!");
        EditorUtility.DisplayDialog("Success", "Cat components configured successfully!\n\nREMINDER: You need to bake NavMesh for the cat to work (Window > AI > Navigation)", "OK");
    }

    #endregion

    #region Collectibles Setup

    private void SetupAllCollectibles()
    {
        int count = 0;

        // Setup Eggs
        count += SetupCollectibleType<EggCollectible>("EggCollectible");

        // Setup Wheat variants
        count += SetupCollectibleType<GoldWheatCollectible>("GoldWheatCollectible");
        count += SetupCollectibleType<HolyWheatCollectible>("HolyWheatCollectible");
        count += SetupCollectibleType<RottenWheatCollectible>("RottenWheatCollectible");

        Debug.Log($"✓ Setup {count} collectibles!");
        EditorUtility.DisplayDialog("Success", $"Configured {count} collectible objects!", "OK");
    }

    private int SetupCollectibleType<T>(string prefabName) where T : MonoBehaviour
    {
        T[] collectibles = FindObjectsOfType<T>();
        int count = 0;

        foreach (T collectible in collectibles)
        {
            GameObject obj = collectible.gameObject;
            Undo.RecordObject(obj, $"Setup {prefabName}");

            // Add SphereCollider as trigger
            SphereCollider sphere = obj.GetComponent<SphereCollider>();
            if (sphere == null)
            {
                sphere = obj.AddComponent<SphereCollider>();
                Debug.Log($"Added SphereCollider to {obj.name}");
            }

            sphere.isTrigger = true; // CRITICAL - must be trigger for OnTriggerEnter
            sphere.radius = 0.5f;

            // Collectibles do NOT need Rigidbody (they are static triggers)

            EditorUtility.SetDirty(obj);
            count++;
        }

        return count;
    }

    #endregion

    #region Damageables Setup

    private void SetupAllDamageables()
    {
        int count = 0;

        // Setup Knives
        KnifeDamageable[] knives = FindObjectsOfType<KnifeDamageable>();
        foreach (KnifeDamageable knife in knives)
        {
            SetupDamageable(knife.gameObject, "Knife");
            count++;
        }

        // Setup Fire/Stoves
        FireDamageable[] fires = FindObjectsOfType<FireDamageable>();
        foreach (FireDamageable fire in fires)
        {
            SetupDamageable(fire.gameObject, "Fire");
            count++;
        }

        Debug.Log($"✓ Setup {count} damageables!");
        EditorUtility.DisplayDialog("Success", $"Configured {count} damageable objects!", "OK");
    }

    private void SetupDamageable(GameObject obj, string type)
    {
        Undo.RecordObject(obj, $"Setup {type} Damageable");

        // Add Rigidbody (needed for collision detection and knockback)
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
            Debug.Log($"Added Rigidbody to {obj.name}");
        }

        // Configure Rigidbody
        rb.mass = 0.5f;
        rb.useGravity = true;
        rb.isKinematic = false; // Can be moved by physics

        // Add BoxCollider
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = obj.AddComponent<BoxCollider>();
            box.size = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log($"Added BoxCollider to {obj.name}");
        }

        box.isTrigger = false; // NOT a trigger - uses OnCollisionEnter

        EditorUtility.SetDirty(obj);
    }

    #endregion

    #region Booster Setup

    private void SetupSpatulaBooster()
    {
        SpatulaBooster[] spatulas = FindObjectsOfType<SpatulaBooster>();
        int count = 0;

        foreach (SpatulaBooster spatula in spatulas)
        {
            GameObject obj = spatula.gameObject;
            Undo.RecordObject(obj, "Setup Spatula Booster");

            // Add BoxCollider
            BoxCollider box = obj.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = obj.AddComponent<BoxCollider>();
                box.size = new Vector3(1f, 0.5f, 1f);
                Debug.Log($"Added BoxCollider to {obj.name}");
            }

            box.isTrigger = false; // Uses OnCollisionEnter

            // Add Rigidbody (can be kinematic since it doesn't move)
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody>();
                Debug.Log($"Added Rigidbody to {obj.name}");
            }

            rb.isKinematic = true; // Doesn't move
            rb.useGravity = false;

            // Add Animator
            Animator animator = obj.GetComponent<Animator>();
            if (animator == null)
            {
                animator = obj.AddComponent<Animator>();
                Debug.LogWarning($"Added Animator to {obj.name} - assign AnimatorController!");
            }

            EditorUtility.SetDirty(obj);
            count++;
        }

        Debug.Log($"✓ Setup {count} spatula boosters!");
        if (count > 0)
        {
            EditorUtility.DisplayDialog("Success", $"Configured {count} spatula booster(s)!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No spatula boosters found in scene.", "OK");
        }
    }

    #endregion

    #region Counter Triggers Setup

    private void SetupCounterTriggers()
    {
        CounterTriggerController[] counters = FindObjectsOfType<CounterTriggerController>();
        int count = 0;

        foreach (CounterTriggerController counter in counters)
        {
            GameObject obj = counter.gameObject;
            Undo.RecordObject(obj, "Setup Counter Trigger");

            // Add BoxCollider as trigger
            BoxCollider box = obj.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = obj.AddComponent<BoxCollider>();
                box.size = new Vector3(2f, 2f, 2f);
                Debug.Log($"Added BoxCollider to {obj.name}");
            }

            box.isTrigger = true; // MUST be trigger for OnTriggerEnter/Exit

            // Counters do NOT need Rigidbody (static triggers)

            EditorUtility.SetDirty(obj);
            count++;
        }

        Debug.Log($"✓ Setup {count} counter triggers!");
        if (count > 0)
        {
            EditorUtility.DisplayDialog("Success", $"Configured {count} counter trigger(s)!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No counter triggers found in scene.", "OK");
        }
    }

    #endregion

    #region Complete Setup

    private void SetupAllComponents()
    {
        Debug.Log("Starting complete CikWick component setup...");

        SetupPlayerComponents();
        SetupCatComponents();
        SetupAllCollectibles();
        SetupAllDamageables();
        SetupSpatulaBooster();
        SetupCounterTriggers();

        Debug.Log("✓✓✓ Complete CikWick setup finished!");
        EditorUtility.DisplayDialog("Complete Setup Finished",
            "All components have been configured!\n\n" +
            "IMPORTANT NEXT STEPS:\n" +
            "1. Bake NavMesh (Window > AI > Navigation)\n" +
            "2. Assign animator controllers to Cat and Spatula\n" +
            "3. Verify layer assignments (Ground vs Floor)\n" +
            "4. Test the game!",
            "OK");
    }

    #endregion
}
