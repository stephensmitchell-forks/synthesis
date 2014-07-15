﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Threading;

/// <summary>
/// Handles all the variables for a thread to find the radius of a component.
/// </summary>
class FindRadiusThread
{
    Thread findRadius;
    ComponentOccurrence component;
    BXDVector3 rotationAxis;
    static double fullRadius;
    static ComponentOccurrence treadPart;

    public FindRadiusThread(ComponentOccurrence passComponent, BXDVector3 passRotationAxis)
    {
        findRadius = new Thread(() => FindMaxRadius());
        component = passComponent;
        rotationAxis = passRotationAxis;
    }

    static public void Reset()
    {
        fullRadius = 0;
        treadPart = null;
    }

    static public double GetRadius()
    {
        return fullRadius;
    }

    static public ComponentOccurrence GetWidthComponent()
    {
        return treadPart;
    }

    public void Start()
    {
        findRadius.Start();
    }

    public void Join()
    {
        findRadius.Join();
    }

    /// <summary>
    /// Calculates the radius of component by creating a mesh and checking their distance from the origin.
    /// </summary>
    public void FindMaxRadius()
    {
        const double MESH_TOLERANCE = 0.5;
        Inventor.Point tmp = ((Inventor.Application)System.Runtime.InteropServices.Marshal.
            GetActiveObject("Inventor.Application")).TransientGeometry.CreatePoint();
        int vertexCount;
        int segmentCount;
        //TODO: Figure out if arrays are right for c#.
        double[] verticeCoords = new double[10000];
        int[] verticeIndicies = new int[10000];
        double newRadius;
        Vector vertex = ((Inventor.Application)System.Runtime.InteropServices.Marshal.
            GetActiveObject("Inventor.Application")).TransientGeometry.CreateVector();
        Vector projectedVector = ((Inventor.Application)System.Runtime.InteropServices.Marshal.
            GetActiveObject("Inventor.Application")).TransientGeometry.CreateVector();
        treadPart = null;
        FindRadiusThread newThread;
        List<FindRadiusThread> radiusThreadList = new List<FindRadiusThread>();
        double minRadius = 0.0;
        double maxRadius = 0.0;
        double localFullRadius = 0.0;
        Vector myRotationAxis = Program.INVENTOR_APPLICATION.TransientGeometry.CreateVector();
        Inventor.Point origin = Program.INVENTOR_APPLICATION.TransientGeometry.CreatePoint();
        Vector partXAxis;
        Vector partYAxis;
        Vector partZAxis;
        Vector asmXAxis = Program.INVENTOR_APPLICATION.TransientGeometry.CreateVector(1, 0, 0);
        Vector asmYAxis = Program.INVENTOR_APPLICATION.TransientGeometry.CreateVector(0, 1, 0);
        Vector asmZAxis = Program.INVENTOR_APPLICATION.TransientGeometry.CreateVector(0, 0, 1);
        Matrix asmToPart = Program.INVENTOR_APPLICATION.TransientGeometry.CreateMatrix();
        Matrix transformedVector = Program.INVENTOR_APPLICATION.TransientGeometry.CreateMatrix();

        Console.WriteLine("Finding radius of " + component.Name + ".");

        foreach (ComponentOccurrence sub in component.SubOccurrences)
        {
            newThread = new FindRadiusThread(sub, rotationAxis);
            radiusThreadList.Add(newThread);
            newThread.Start();
        }

        //Takes the part axes and the assembly axes and creates a transformation from one to the other.
        component.Transformation.GetCoordinateSystem(out origin, out partXAxis, out partYAxis, out partZAxis);

        asmToPart.SetToAlignCoordinateSystems(origin, partXAxis, partYAxis, partZAxis, origin, asmXAxis, asmYAxis, asmZAxis);

        //The joint normal is changed from being relative to assembly to relative to the part axes.
        transformedVector.Cell[1, 1] = rotationAxis.x;
        transformedVector.Cell[2, 1] = rotationAxis.y;
        transformedVector.Cell[3, 1] = rotationAxis.z;

        Console.Write("Changing vector from " + transformedVector.Cell[1, 1] + ", " + transformedVector.Cell[2, 1] + ", " + transformedVector.Cell[3, 1]);

        transformedVector.TransformBy(asmToPart);

        myRotationAxis.X = transformedVector.Cell[1, 1];
        myRotationAxis.Y = transformedVector.Cell[2, 1];
        myRotationAxis.Z = transformedVector.Cell[3, 1];

        Console.Write(" to " + transformedVector.Cell[1, 1] + ", " + transformedVector.Cell[2, 1] + ", " + transformedVector.Cell[3, 1] + ".\n");

        foreach (SurfaceBody surface in component.Definition.SurfaceBodies)
        {


            surface.CalculateStrokes(MESH_TOLERANCE, out vertexCount, out segmentCount, out verticeCoords, out verticeIndicies);

            Console.WriteLine(Convert.ToString(vertexCount) + " vertices in mesh of " + component.Name + ".");

            for (int i = 0; i < verticeCoords.Length; i += 3)
            {
                vertex.X = verticeCoords[i];
                vertex.Y = verticeCoords[i + 1];
                vertex.Z = verticeCoords[i + 2];

                projectedVector = myRotationAxis.CrossProduct(vertex);

                newRadius = Math.Sqrt(Math.Pow(projectedVector.X, 2) + Math.Pow(projectedVector.Y, 2) + Math.Pow(projectedVector.Z, 2));

                if (newRadius > maxRadius)
                {
                    maxRadius = newRadius;

                    //Sets the starting point if this is the first vertex.
                    if (minRadius == 0.0)
                    {
                        minRadius = newRadius;
                    }

                    localFullRadius = maxRadius - minRadius;
                }
                else if (newRadius < minRadius)
                {
                    minRadius = newRadius;

                    //Sets the starting point if this is the first vertex.
                    if (maxRadius == 0.0)
                    {
                        maxRadius = newRadius;
                    }

                    localFullRadius = maxRadius - minRadius;
                }

                lock (Program.INVENTOR_APPLICATION)
                {
                    if (localFullRadius > fullRadius)
                    {
                        fullRadius = localFullRadius;

                        treadPart = component;
                    }
                }
            }
        }

        foreach (FindRadiusThread thread in radiusThreadList)
        {
            thread.Join();
        }

        Console.WriteLine("Found radius of " + component.Name + ".");
    }   
}

