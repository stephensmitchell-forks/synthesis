using Inventor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static partial class SurfaceExporter
{
    public static void ClearAssets()
    {
        assets.Clear();
    }

    /// <summary>
    /// Exports all the components in this group to the in-RAM mesh.
    /// </summary>
    /// <param name="group">Group to export from</param>
    /// <param name="reporter">Progress reporter</param>
    public static BXDAMesh ExportAll(CustomRigidGroup group, Guid guid, Progress progressReporter)
    {
        // Create output mesh
        MeshController outputMesh = new MeshController(guid);

        // Collect surfaces to export
        List<SurfaceBody> plannedSurfaces = GenerateExportList(group, outputMesh.Mesh.physics);

        // Export faces, multithreaded
        if (plannedSurfaces.Count > 0)
        {
            // Reset progress bar
            progressReporter.Status = 0;

            // Start jobs
            int totalJobsFinished = 0;
            object finishLock = new object(); // Used to prevent multiple threads from updating progress bar at the same time.

            Parallel.ForEach(plannedSurfaces, (SurfaceBody surface) =>
            {
                CalculateSurfaceFacets(surface, outputMesh, SynthesisGUI.PluginSettings.GeneralUseFancyColors);

                lock (finishLock)
                {
                    totalJobsFinished++;
                    progressReporter.Status = (double)totalJobsFinished / plannedSurfaces.Count;
                }
            });

            outputMesh.DumpOutput();
        }

        return outputMesh.Mesh;
    }
}