using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;

#endif


[ExecuteAlways]
public class SplineRibbonMeshCollider : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;

    [Header("Wall Geometry")]
    [Min(0.1f)] public float height = 3f;          // vertical size
    [Min(0.005f)] public float thickness = 0.06f;  // lateral thickness (binormal direction)
    [Tooltip("Approx target segment length along the spline (world).")]
    [Min(0.05f)] public float targetSegmentLength = 0.6f;

    [Header("Sampling")]
    [Tooltip("Keep per-segment tangent turn below this (degrees). Lower = smoother around bends.")]
    [Range(2f, 30f)] public float maxDegreesPerSegment = 8f;
    [Tooltip("Force world up (prevents twist if your spline rolls).")]
    public bool useFixedWorldUp = true;

    [Header("Build")]
    public bool autoRebuild = true;

    MeshFilter mf;
    MeshRenderer mr;
    MeshCollider mc;
    Mesh mesh;


    void OnEnable()
    {
        if (!splineContainer) splineContainer = GetComponent<SplineContainer>();
        EnsureComponents();
#if UNITY_EDITOR
                if (autoRebuild) EditorApplication.update += EditorTick;
#endif
    }
    void OnDisable()
    {
#if UNITY_EDITOR
                EditorApplication.update -= EditorTick;
#endif
    }

#if UNITY_EDITOR
    void EditorTick()
    {
        if (autoRebuild) Rebuild();
    }
#endif

    void EnsureComponents()
    {
        if (!mf) mf = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        if (!mr) mr = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
        if (!mc) mc = GetComponent<MeshCollider>() ?? gameObject.AddComponent<MeshCollider>();
        if (mesh == null)
        {
            mesh = new Mesh { name = "SplineRibbonMesh" };
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
    }

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        if (!splineContainer || splineContainer.Splines.Count == 0) return;
        EnsureComponents();

        var world = splineContainer.transform;

        // Aggregate all splines (supports multiple)
        var verts = new List<Vector3>();
        var norms = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        foreach (var spline in splineContainer.Splines)
        {
            BuildSplineSection(spline, world, verts, norms, uvs, tris);
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();

        mf.sharedMesh = mesh;

        // MeshCollider: non-convex, sharedMesh must be set AFTER triangles
        mc.sharedMesh = null;
        mc.convex = false;
        mc.sharedMesh = mesh;
        mc.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning
                          | MeshColliderCookingOptions.WeldColocatedVertices
                          | MeshColliderCookingOptions.CookForFasterSimulation;
    }

    void BuildSplineSection(
       Spline spline, Transform world,
       List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris)
    {
        // Sample along spline with adaptive stepping
        float approxLen = SplineUtility.CalculateLength(spline, (float4x4)world.localToWorldMatrix);
        int minSegments = Mathf.Max(4, Mathf.CeilToInt(approxLen / targetSegmentLength));
        float t = 0f;

        var frames = new List<(Vector3 pos, Vector3 tan, Vector3 up, Vector3 binorm)>();

        // Build frames
        Vector3 prevTanW = Vector3.forward;
        bool havePrev = false;
        int safety = 0;
        while (t < 1f - 1e-5f && safety++ < 20000)
        {
            SplineUtility.Evaluate(spline, Mathf.Clamp01(t), out var pL, out var tL, out var upL);
            var pW = world.TransformPoint(pL);
            var tanW = world.TransformDirection(tL).normalized;
            var upW = useFixedWorldUp ? Vector3.up : world.TransformDirection(upL).normalized;

            // Compute binormal (right) as tangent x up
            var bin = Vector3.Cross(tanW, upW).normalized;
            // Re-orthogonalize up to avoid drift
            upW = Vector3.Cross(bin, tanW).normalized;

            if (!havePrev)
            {
                frames.Add((pW, tanW, upW, bin));
                prevTanW = tanW;
                havePrev = true;
                t += 1f / (minSegments * 2f); // seed step
                continue;
            }

            float angle = Vector3.Angle(prevTanW, tanW);
            float step = Mathf.Max(0.0005f, targetSegmentLength / Mathf.Max(approxLen, 0.001f));

            if (angle > maxDegreesPerSegment) step *= 0.35f; // shrink step on tight bends

            frames.Add((pW, tanW, upW, bin));
            prevTanW = tanW;
            t += step;
        }

        // Last sample at t=1
        {
            SplineUtility.Evaluate(spline, 1f, out var pL, out var tL, out var upL);
            var pW = world.TransformPoint(pL);
            var tanW = world.TransformDirection(tL).normalized;
            var upW = useFixedWorldUp ? Vector3.up : world.TransformDirection(upL).normalized;
            var bin = Vector3.Cross(tanW, upW).normalized;
            upW = Vector3.Cross(bin, tanW).normalized;
            frames.Add((pW, tanW, upW, bin));
        }

        bool closed = spline.Closed;
        int n = frames.Count;
        if (n < 2) return;

        // Build a thin, capped box strip:
        // For each sample, we create 4 rim vertices:
        //  bottom-left (BL), bottom-right (BR), top-left (TL), top-right (TR)
        // Left/right are along ±binormal by half thickness.
        int baseVert = verts.Count;

        for (int i = 0; i < n; i++)
        {
            var (p, _tan, up, bin) = frames[i];
            Vector3 left = p - bin * (thickness * 0.5f);
            Vector3 right = p + bin * (thickness * 0.5f);
            Vector3 bl = left;
            Vector3 br = right;
            Vector3 tl = left + up * height;
            Vector3 tr = right + up * height;

            verts.Add(bl); verts.Add(br); verts.Add(tl); verts.Add(tr);

            // Normals (approx): outwards per face; for strip use up for top, -up for bottom,
            // and +/-bin for sides. Here we’ll store something reasonable; MeshCollider
            // doesn’t need them, but if you ever render the mesh they help.
            norms.Add(-Vector3.up);  // BL (bottom face normal-ish)
            norms.Add(-Vector3.up);  // BR
            norms.Add(Vector3.up);   // TL (top)
            norms.Add(Vector3.up);   // TR

            // UVs (optional but nice)
            float v = (float)i / (n - 1);
            uvs.Add(new Vector2(0, v));
            uvs.Add(new Vector2(1, v));
            uvs.Add(new Vector2(0, v));
            uvs.Add(new Vector2(1, v));
        }

        // Helper to append a quad strip between rings i and i+1 given local offsets
        void AddStrip(int a0, int a1, int b0, int b1)
        {
            for (int i = 0; i < n - 1; i++)
            {
                int i0 = baseVert + i * 4;
                int i1 = baseVert + (i + 1) * 4;

                int A0 = i0 + a0, A1 = i0 + a1;
                int B0 = i1 + b0, B1 = i1 + b1;

                tris.Add(A0); tris.Add(B0); tris.Add(B1);
                tris.Add(A0); tris.Add(B1); tris.Add(A1);
            }

            // Close the loop if the spline is closed
            if (closed)
            {
                int i0 = baseVert + (n - 1) * 4;
                int i1 = baseVert + 0 * 4;

                int A0 = i0 + a0, A1 = i0 + a1;
                int B0 = i1 + b0, B1 = i1 + b1;

                tris.Add(A0); tris.Add(B0); tris.Add(B1);
                tris.Add(A0); tris.Add(B1); tris.Add(A1);
            }
        }

        // Vertex layout per ring: 0=BL, 1=BR, 2=TL, 3=TR
        // Build four faces: bottom, top, left side, right side
        AddStrip(0, 1, 0, 1); // bottom
        AddStrip(2, 3, 2, 3); // top
        AddStrip(0, 2, 0, 2); // left side
        AddStrip(1, 3, 1, 3); // right side

        // Cap the ends if open
        if (!closed)
        {
            int head = baseVert + 0;
            int tail = baseVert + (n - 1) * 4;

            // front cap (at start): BL(0), BR(1), TR(3), TL(2)
            tris.Add(head + 0); tris.Add(head + 3); tris.Add(head + 1);
            tris.Add(head + 0); tris.Add(head + 2); tris.Add(head + 3);

            // back cap (at end)
            tris.Add(tail + 0); tris.Add(tail + 1); tris.Add(tail + 3);
            tris.Add(tail + 0); tris.Add(tail + 3); tris.Add(tail + 2);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SplineRibbonMeshCollider))]
    public class SplineRibbonMeshColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var b = (SplineRibbonMeshCollider)target;

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Rebuild")) b.Rebuild();
            }

            EditorGUILayout.HelpBox(
                "Use a non-bouncy PhysicMaterial on this MeshCollider for smooth stops. " +
                "Keep this object static (no Rigidbody) so the non-convex MeshCollider is valid at runtime.",
                MessageType.Info);
        }
    }
#endif



}
