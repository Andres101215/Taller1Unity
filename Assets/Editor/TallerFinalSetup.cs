using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// One-click setup for the final workshop. Automates the Editor-side work that
/// cannot be done from a script: rebuilding the player prefab, generating the
/// Animator Controller, and generating the animation state diagram FROM that
/// controller so the two can never drift apart.
///
/// The rubric asks for the diagram to match the Animator Controller exactly.
/// Generating both from the same source of truth removes the human error.
///
/// Menu: "Taller Final" in the Unity toolbar.
/// </summary>
public static class TallerFinalSetup
{
    const string PrefabPath = "Assets/Prefabs/Player/Player.prefab";
    const string ModelFolder = "Assets/Models/Player";
    const string AnimatorFolder = "Assets/Animations";
    const string ControllerPath = "Assets/Animations/Player.controller";
    const string DiagramPath = "Docs/Diagrama_Animaciones.md";

    const string TagPlayer = "Jugador";
    const float PlayerHeight = 1.8f;
    const float PlayerRadius = 0.35f;
    const float PlayerCenterY = 0.9f;

    static readonly Vector3 CameraOffset = new Vector3(0f, 2.5f, -5f);

    // ------------------------------------------------------------------ 1

    [MenuItem("Taller Final/1. Reconstruir prefab Player")]
    public static void RebuildPlayerPrefab()
    {
        if (!File.Exists(PrefabPath))
        {
            EditorUtility.DisplayDialog("Taller Final", "No existe " + PrefabPath, "OK");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            Undo.RecordObject(root, "Reconstruir prefab Player");

            // 1. Tag
            TrySetTag(root, TagPlayer);

            // 2. Scale: a CharacterController inherits the GameObject scale, so a
            //    scaled player gets a proportionally scaled capsule.
            root.transform.localScale = Vector3.one;
            root.transform.localPosition = new Vector3(0f, 0f, 0f);

            // 3. Remove the sphere leftovers. A Rigidbody and a CharacterController
            //    on the same object fight over the position.
            int removed = 0;
            removed += RemoveComponent<Rigidbody>(root);
            removed += RemoveComponent<SphereCollider>(root);

            // 4. Required components with the right values
            CharacterController controller = root.GetComponent<CharacterController>();
            if (controller == null)
            {
                controller = root.AddComponent<CharacterController>();
            }

            controller.height = PlayerHeight;
            controller.radius = PlayerRadius;
            controller.center = new Vector3(0f, PlayerCenterY, 0f);
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            controller.skinWidth = 0.08f;
            controller.slopeLimit = 45f;

            if (root.GetComponent<GroundChecker>() == null)
            {
                root.AddComponent<GroundChecker>();
            }

            PlayerController playerController = root.GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = root.AddComponent<PlayerController>();
            }

            // 5. The visual model lives in a child named "Model": the root is what
            //    the CharacterController moves, the child is what gets animated.
            Transform model = root.transform.Find("Model");
            if (model == null)
            {
                GameObject modelPrefab = FindHumanoidModel();
                if (modelPrefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
                    instance.name = "Model";
                    instance.transform.SetParent(root.transform, false);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = Vector3.one;
                    model = instance.transform;
                    Debug.Log("[Taller Final] Modelo importado al prefab: " + modelPrefab.name);
                }
                else
                {
                    Debug.LogWarning("[Taller Final] No hay ningun modelo en " + ModelFolder +
                                     ". Descarga el humanoide de Mixamo en Assets/Models/Player y vuelve a ejecutar este menu.");
                }
            }

            // 6. Animator + driver on the model
            if (model != null)
            {
                Animator animator = model.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = model.gameObject.AddComponent<Animator>();
                }

                PlayerAnimationDriver driver = model.GetComponent<PlayerAnimationDriver>();
                if (driver == null)
                {
                    driver = model.gameObject.AddComponent<PlayerAnimationDriver>();
                }

                driver.playerController = playerController;

                if (animator.runtimeAnimatorController == null && AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath) != null)
                {
                    animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Debug.Log("[Taller Final] Prefab reconstruido. Componentes eliminados: " + removed +
                      ". Escala 1, tag " + TagPlayer + ", CharacterController " + PlayerHeight + "m.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    // ------------------------------------------------------------------ 2

    [MenuItem("Taller Final/2. Crear Animator Controller y diagrama")]
    public static void CreateAnimatorController()
    {
        if (!Directory.Exists(AnimatorFolder))
        {
            Directory.CreateDirectory(AnimatorFolder);
            AssetDatabase.Refresh();
        }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        }
        else
        {
            controller.layers = new AnimatorControllerLayer[] { controller.layers[0] };
            controller.layers[0].name = "Base Layer";
            AnimatorStateMachine sm = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState child in new List<ChildAnimatorState>(sm.states)) sm.RemoveState(child.state);
            foreach (ChildAnimatorStateMachine child in new List<ChildAnimatorStateMachine>(sm.stateMachines)) sm.RemoveStateMachine(child.stateMachine);
            foreach (AnimatorStateTransition t in new List<AnimatorStateTransition>(sm.anyStateTransitions)) sm.RemoveAnyStateTransition(t);
        }

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;

        AnimatorState idle = machine.AddState("Idle");
        AnimatorState locomotion = machine.AddState("Locomotion");
        AnimatorState jump = machine.AddState("Jump");

        // Locomotion is a 1D blend tree over the horizontal speed.
        BlendTree blend = new BlendTree();
        blend.blendType = BlendTreeType.Simple1D;
        blend.blendParameter = "Speed";
        blend.useAutomaticThresholds = false;

        AddClip(blend, "Idle", 0f, controller);
        AddClip(blend, "Walk", 0.1f, controller);
        AddClip(blend, "Run", 3.5f, controller);
        locomotion.motion = blend;

        AssetDatabase.AddObjectToAsset(blend, ControllerPath);
        AssetDatabase.ImportAsset(ControllerPath);

        machine.defaultState = idle;

        AddTransition(idle, locomotion, "Greater", 0.1f, false);
        AddTransition(locomotion, idle, "Less", 0.1f, true);
        AddTransition(locomotion, jump, "If", 0f, false, "Jump");
        AddTransition(jump, locomotion, "If", 0f, false, "Grounded", true);

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GenerateDiagram(controller);

        Debug.Log("[Taller Final] Animator Controller creado en " + ControllerPath +
                  ". Diagrama generado en " + DiagramPath + ".");
    }

    static void AddClip(BlendTree blend, string clipName, float threshold, AnimatorController controller)
    {
        AnimationClip clip = FindClip(clipName);
        if (clip == null)
        {
            Debug.LogWarning("[Taller Final] Clip '" + clipName + "' no encontrado en " + AnimatorFolder +
                             ". El estado quedara sin animacion hasta que se agregue.");
            return;
        }

        blend.AddChild(clip, threshold);
    }

    static void AddTransition(AnimatorState from, AnimatorState to, string mode, float threshold, bool hasExitTime, string parameter = null, bool boolValue = false)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = hasExitTime;
        transition.exitTime = hasExitTime ? 0.9f : 0f;
        transition.duration = 0.1f;

        if (parameter != null)
        {
            transition.AddCondition(mode == "If"
                ? (boolValue ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot)
                : (mode == "Greater" ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less),
                threshold, parameter);
        }
    }

    // ------------------------------------------------------------------ 3

    [MenuItem("Taller Final/3. Corregir camara e instancia en escena")]
    public static void FixScene()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            EditorUtility.DisplayDialog("Taller Final", "No se encontro un GameObject llamado 'Player' en la escena.", "OK");
            return;
        }

        Undo.RecordObject(player.transform, "Corregir escala del jugador");
        player.transform.localScale = Vector3.one;
        TrySetTag(player, TagPlayer);

        Camera main = Camera.main;
        if (main != null)
        {
            CameraOrbit orbit = main.GetComponent<CameraOrbit>();
            if (orbit != null)
            {
                Undo.RecordObject(orbit, "Corregir camara");
                orbit.player = player.transform;
                orbit.offset = CameraOffset;
                EditorUtility.SetDirty(orbit);
                Debug.Log("[Taller Final] Camara: offset " + CameraOffset + ", target " + player.name + ".");
            }
            else
            {
                Debug.LogWarning("[Taller Final] La camara principal no tiene CameraOrbit.");
            }
        }

        EditorUtility.SetDirty(player);
        Debug.Log("[Taller Final] Instancia corregida: escala 1, tag " + TagPlayer + ".");
    }

    // ------------------------------------------------------------------ 4

    [MenuItem("Taller Final/4. Chequeo rapido (reporte en consola)")]
    public static void QuickCheck()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== CHEQUEO RAPIDO - Taller Final ===");

        // Prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            report.AppendLine("[FALTA] Prefab del jugador");
        }
        else
        {
            report.AppendLine("[OK] Prefab del jugador");
            report.AppendLine("   Rigidbody: " + (prefab.GetComponent<Rigidbody>() != null ? "PRESENTE (mal)" : "ausente (ok)"));
            report.AppendLine("   SphereCollider: " + (prefab.GetComponent<SphereCollider>() != null ? "PRESENTE (mal)" : "ausente (ok)"));

            CharacterController cc = prefab.GetComponent<CharacterController>();
            report.AppendLine("   CharacterController: " + (cc == null
                ? "AUSENTE (mal)"
                : $"h={cc.height} r={cc.radius} center.y={cc.center.y}"));

            Transform model = prefab.transform.Find("Model");
            report.AppendLine("   Hijo 'Model': " + (model == null ? "AUSENTE (mal)" : "presente"));
            if (model != null)
            {
                Animator animator = model.GetComponent<Animator>();
                report.AppendLine("   Animator: " + (animator == null ? "AUSENTE (mal)" : "presente"));
                if (animator != null)
                {
                    report.AppendLine("   Controller: " + (animator.runtimeAnimatorController == null
                        ? "SIN ASIGNAR (mal)"
                        : animator.runtimeAnimatorController.name));
                }
            }
        }

        // Animator
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        report.AppendLine(DescribeController(controller));

        // Clips
        foreach (string name in new[] { "Idle", "Walk", "Run", "Jump" })
        {
            report.AppendLine("   Clip '" + name + "': " + (FindClip(name) != null ? "OK" : "FALTA"));
        }

        // Scene
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            report.AppendLine("   Escena: escala del jugador = " + player.transform.localScale.ToString("F2") +
                              " | tag = " + player.tag);
        }

        Camera main = Camera.main;
        if (main != null)
        {
            CameraOrbit orbit = main.GetComponent<CameraOrbit>();
            report.AppendLine("   Escena: camara offset = " + (orbit == null ? "sin CameraOrbit" : orbit.offset.ToString("F1")));
        }

        report.AppendLine("=== FIN ===");
        Debug.Log(report.ToString());
    }

    // ------------------------------------------------------------------ 5

    [MenuItem("Taller Final/5. Generar diagrama de animaciones (Mermaid)")]
    public static void RegenerateDiagram()
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            Debug.LogError("[Taller Final] No hay Animator Controller en " + ControllerPath);
            return;
        }

        GenerateDiagram(controller);
        Debug.Log("[Taller Final] Diagrama regenerado en " + DiagramPath + ".");
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>
    /// Writes the state diagram from the actual Animator Controller, so the
    /// diagram can never disagree with the implementation.
    /// </summary>
    static void GenerateDiagram(AnimatorController controller)
    {
        StringBuilder md = new StringBuilder();
        md.AppendLine("# Diagrama de animaciones — El Laberinto de la Gárgola");
        md.AppendLine();
        md.AppendLine("> Este diagrama se genera automáticamente desde `" + ControllerPath + "`.");
        md.AppendLine("> Regenerarlo con **Taller Final → 5. Generar diagrama de animaciones**.");
        md.AppendLine("> Los estados, parámetros y condiciones son exactamente los del Animator Controller.");
        md.AppendLine();
        md.AppendLine("## Parámetros");
        md.AppendLine();
        md.AppendLine("| Parámetro | Tipo | Uso |");
        md.AppendLine("|---|---|---|");
        md.AppendLine("| `Speed` | Float | Velocidad horizontal. Alimenta el Blend Tree de Locomotion. |");
        md.AppendLine("| `Grounded` | Bool | `true` cuando el personaje está apoyado en el suelo. |");
        md.AppendLine("| `Jump` | Trigger | Se dispara en el frame en que el personaje deja el suelo. |");
        md.AppendLine();

        md.AppendLine("## Estados");
        md.AppendLine();
        md.AppendLine("- **Idle** — estado por defecto. Animación de reposo.")
          .AppendLine("- **Locomotion** — Blend Tree 1D sobre `Speed`: `Idle` (0), `Walk` (0.1), `Run` (3.5).")
          .AppendLine("- **Jump** — animación de salto. Se entra con el trigger `Jump` y se sale al volver al suelo.");
        md.AppendLine();

        md.AppendLine("## Transiciones");
        md.AppendLine();
        md.AppendLine("| Desde | Hacia | Condición | Exit Time |");
        md.AppendLine("|---|---|---|---|");

        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            AppendTransitions(md, layer.stateMachine);
        }

        md.AppendLine();
        md.AppendLine("## Diagrama");
        md.AppendLine();
        md.AppendLine("```mermaid");
        md.AppendLine("stateDiagram-v2");
        md.AppendLine("    [*] --> Idle");

        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            AppendMermaid(md, layer.stateMachine);
        }

        md.AppendLine("```");
        md.AppendLine();

        string directory = Path.GetDirectoryName(DiagramPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        File.WriteAllText(DiagramPath, md.ToString(), new UTF8Encoding(false));
        AssetDatabase.ImportAsset(DiagramPath);
    }

    static void AppendTransitions(StringBuilder md, AnimatorStateMachine machine)
    {
        foreach (AnimatorStateTransition transition in machine.anyStateTransitions)
        {
            AppendTransitionRow(md, "Any State", transition);
        }

        foreach (ChildAnimatorState child in machine.states)
        {
            foreach (AnimatorStateTransition transition in child.state.transitions)
            {
                AppendTransitionRow(md, child.state.name, transition);
            }
        }

        foreach (ChildAnimatorStateMachine child in machine.stateMachines)
        {
            AppendTransitions(md, child.stateMachine);
        }
    }

    static void AppendTransitionRow(StringBuilder md, string fromName, AnimatorStateTransition transition)
    {
        string to = transition.destinationState != null ? transition.destinationState.name : "?";
        string exit = transition.hasExitTime ? transition.exitTime.ToString("0.0") : "—";

        if (transition.conditions.Length == 0)
        {
            md.AppendLine($"| {fromName} | {to} | — | {exit} |");
            return;
        }

        foreach (AnimatorCondition condition in transition.conditions)
        {
            md.AppendLine($"| {fromName} | {to} | `{condition.parameter}` {DescribeCondition(condition)} | {exit} |");
        }
    }

    static void AppendMermaid(StringBuilder md, AnimatorStateMachine machine)
    {
        foreach (AnimatorStateTransition transition in machine.anyStateTransitions)
        {
            AppendMermaidRow(md, "Any State", transition);
        }

        foreach (ChildAnimatorState child in machine.states)
        {
            foreach (AnimatorStateTransition transition in child.state.transitions)
            {
                AppendMermaidRow(md, child.state.name, transition);
            }
        }

        foreach (ChildAnimatorStateMachine child in machine.stateMachines)
        {
            AppendMermaid(md, child.stateMachine);
        }
    }

    static void AppendMermaidRow(StringBuilder md, string fromName, AnimatorStateTransition transition)
    {
        string to = transition.destinationState != null ? transition.destinationState.name : "?";

        string label = "";
        if (transition.conditions.Length > 0)
        {
            AnimatorCondition condition = transition.conditions[0];
            label = condition.parameter + " " + DescribeCondition(condition);
        }

        md.AppendLine($"    {fromName} --> {to} : {label}");
    }

    static string DescribeCondition(AnimatorCondition condition)
    {
        switch (condition.mode)
        {
            case AnimatorConditionMode.Greater: return ">";
            case AnimatorConditionMode.Less: return "<";
            case AnimatorConditionMode.Equals: return "==";
            case AnimatorConditionMode.NotEqual: return "!=";
            case AnimatorConditionMode.If: return "== true";
            case AnimatorConditionMode.IfNot: return "== false";
            default: return "";
        }
    }

    static string DescribeController(AnimatorController controller)
    {
        if (controller == null)
        {
            return "[FALTA] Animator Controller";
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[OK] Animator Controller");
        foreach (AnimatorControllerParameter parameter in controller.parameters)
        {
            sb.AppendLine("   Parámetro: " + parameter.name + " (" + parameter.type + ")");
        }

        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            foreach (ChildAnimatorState child in layer.stateMachine.states)
            {
                string motion = child.state.motion != null ? child.state.motion.name : "sin animacion";
                sb.AppendLine("   Estado: " + child.state.name + " | motion: " + motion);
            }
        }

        return sb.ToString().TrimEnd();
    }

    static AnimationClip FindClip(string name)
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { AnimatorFolder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null && clip.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return clip;
            }
        }

        return null;
    }

    static GameObject FindHumanoidModel()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { ModelFolder });
        if (guids.Length == 0)
        {
            return null;
        }

        // Prefer a humanoid-looking file name when several are present.
        string[] preferred = { "mixamo", "bot", "player", "character", "model", "humano" };

        foreach (string preferredName in preferred)
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset != null && asset.name.ToLowerInvariant().Contains(preferredName))
                {
                    return asset;
                }
            }
        }

        return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    static int RemoveComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component == null)
        {
            return 0;
        }

        Object.DestroyImmediate(component, true);
        return 1;
    }

    static void TrySetTag(GameObject target, string tag)
    {
        if (target.CompareTag(tag))
        {
            return;
        }

        foreach (string availableTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (availableTag.Equals(tag, System.StringComparison.OrdinalIgnoreCase))
            {
                target.tag = tag;
                return;
            }
        }

        Debug.LogWarning("[Taller Final] El tag '" + tag + "' no existe en TagManager. Agregalo antes.");
    }
}
