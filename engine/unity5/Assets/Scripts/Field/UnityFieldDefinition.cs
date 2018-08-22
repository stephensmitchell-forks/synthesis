using BulletUnity;
using Synthesis.FEA;
using Synthesis.NetworkMultiplayer;
using Synthesis.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Synthesis.Field
{
    public class UnityFieldDefinition : FieldDefinition
    {
        public GameObject unityObject;
        private const float CollisionMargin = 0.01f;
        private const float FrictionScale = 0.02f;
        private const float RollingFrictionScale = 0.0025f;

        public UnityFieldDefinition(Guid guid, string name)
            : base(guid, name)
        {
        }

        public void CreateTransform(Transform root)
        {
            unityObject = root.gameObject;
        }

        public bool CreateMesh(string filePath, bool multiplayer = false, bool host = false)
        {
            BXDAMesh mesh = new BXDAMesh();
            mesh.ReadFromFile(filePath, null);

            if (!mesh.GUID.Equals(GUID))
                return false;

            List<FieldNode> remainingNodes = new List<FieldNode>(NodeGroup.EnumerateAllLeafFieldNodes());

            List<KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>> submeshes = new List<KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>>();
            List<KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>> colliders = new List<KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>>();
            Dictionary<string, GameObject> joinedPropertySets = new Dictionary<string, GameObject>();

            // Create all submesh objects
            Auxiliary.ReadMeshSet(mesh.meshes, delegate (int id, BXDAMesh.BXDASubMesh sub, Mesh meshu)
            {
                submeshes.Add(new KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>(sub, meshu));
            });

            // Create all collider objects
            Auxiliary.ReadMeshSet(mesh.colliders, delegate (int id, BXDAMesh.BXDASubMesh sub, Mesh meshu)
            {
                colliders.Add(new KeyValuePair<BXDAMesh.BXDASubMesh, Mesh>(sub, meshu));
            });

            Dictionary<string, NetworkElement> networkElements = new Dictionary<string, NetworkElement>();

            foreach (NetworkElement ne in Resources.FindObjectsOfTypeAll<NetworkElement>())
                networkElements[ne.NodeID] = ne;

            foreach (FieldNode node in NodeGroup.EnumerateAllLeafFieldNodes())
            {
                PropertySet? propertySet = null;

                if (GetPropertySets().ContainsKey(node.PropertySetID))
                    propertySet = GetPropertySets()[node.PropertySetID];

                GameObject subObject;

                if (multiplayer && propertySet.HasValue && propertySet.Value.Mass != 0)
                {
                    if (host)
                    {
                        subObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("prefabs/NetworkElement"), unityObject.transform);
                        subObject.GetComponent<NetworkElement>().NodeID = node.NodeID;
                        subObject.name = node.NodeID;
                        NetworkServer.Spawn(subObject);
                    }
                    else
                    {
                        subObject = networkElements[node.NodeID].gameObject;
                        subObject.name = node.NodeID;
                    }
                }
                else
                {
                    subObject = new GameObject(node.NodeID);
                }

                subObject.transform.parent = unityObject.transform;

                GameObject meshObject = new GameObject(node.NodeID + "-mesh");

                if (node.SubMeshID != -1)
                {
                    KeyValuePair<BXDAMesh.BXDASubMesh, Mesh> currentSubMesh = submeshes[node.SubMeshID];

                    BXDAMesh.BXDASubMesh sub = currentSubMesh.Key;
                    Mesh meshu = currentSubMesh.Value;

                    meshObject.AddComponent<MeshFilter>().mesh = meshu;
                    meshObject.AddComponent<MeshRenderer>();
                    Material[] matls = new Material[meshu.subMeshCount];

                    for (int i = 0; i < matls.Length; i++)
                    {
                        matls[i] = sub.surfaces[i].AsMaterial();
                    }

                    meshObject.GetComponent<MeshRenderer>().materials = matls;
                }

                // Invert the x-axis to compensate for Unity's inverted coordinate system.
                meshObject.transform.localScale = new Vector3(-1f, 1f, 1f);

                // Set the rotation of the object (the x and w properties are inverted to once again compensate for Unity's differences).
                meshObject.transform.localRotation = new Quaternion(-node.Rotation.X, node.Rotation.Y, node.Rotation.Z, -node.Rotation.W);

                // Set the position of the object (scaled by 1/100 to match Unity's scaling correctly).
                meshObject.transform.position = new Vector3(-node.Position.x * 0.01f, node.Position.y * 0.01f, node.Position.z * 0.01f);

                if (GetPropertySets().ContainsKey(node.PropertySetID))
                {
                    PropertySet currentPropertySet = GetPropertySets()[node.PropertySetID];

                    BCollisionShape collider = CreateCollider(currentPropertySet.Collider, meshObject, subObject);

                    // Give this subobject a collider if the property set specifies that members should be separated.
                    if (currentPropertySet.Separated)
                    {
                        BRigidBody rb = subObject.AddComponent<BRigidBody>();
                        rb.friction = currentPropertySet.Friction * FrictionScale;
                        rb.rollingFriction = currentPropertySet.Friction * RollingFrictionScale;
                        rb.mass = currentPropertySet.Mass;

                        if (currentPropertySet.Mass == 0)
                            rb.collisionFlags = BulletSharp.CollisionFlags.StaticObject;
                        else
                        {
                            subObject.AddComponent<Tracker>();
                            subObject.name = currentPropertySet.PropertySetID;
                        }
                    }
                    // Give property set a this object's collider if not separated.
                    else
                    {
                        GameObject propObject;

                        // Get existing property set parent if it has been created
                        if (joinedPropertySets.ContainsKey(currentPropertySet.PropertySetID))
                        {
                            propObject = joinedPropertySets[currentPropertySet.PropertySetID];
                        }
                        else
                        {
                            if (multiplayer && currentPropertySet.Mass != 0)
                            {
                                if (host)
                                {
                                    propObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("prefabs/NetworkElement"), unityObject.transform);
                                    propObject.GetComponent<NetworkElement>().NodeID = currentPropertySet.PropertySetID;
                                    propObject.name = currentPropertySet.PropertySetID;
                                    NetworkServer.Spawn(propObject);
                                }
                                else
                                {
                                    propObject = networkElements[currentPropertySet.PropertySetID].gameObject;
                                    propObject.name = currentPropertySet.PropertySetID;
                                }
                            }
                            else
                            {
                                propObject = new GameObject(currentPropertySet.PropertySetID);
                                propObject.transform.parent = unityObject.transform;
                            }

                            // Create multi-collider for property set
                            propObject.AddComponent<BUExtensions.BMultiShape>();

                            // Create rigidbody for property set
                            BRigidBody rb = propObject.AddComponent<BRigidBody>();
                            rb.friction = currentPropertySet.Friction * FrictionScale;
                            rb.rollingFriction = currentPropertySet.Friction * RollingFrictionScale;
                            rb.mass = currentPropertySet.Mass;

                            if (currentPropertySet.Mass == 0)
                                rb.collisionFlags = BulletSharp.CollisionFlags.StaticObject;
                            else
                                propObject.AddComponent<Tracker>();

                            joinedPropertySets[currentPropertySet.PropertySetID] = propObject;
                        }

                        subObject.transform.parent = propObject.transform;

                        // Create offset based on the subObject's local transform
                        BulletSharp.Math.Quaternion bRot = subObject.transform.localRotation.ToBullet();
                        BulletSharp.Math.Vector3 bPos = subObject.transform.localPosition.ToBullet();
                        BulletSharp.Math.Matrix offset;
                        BulletSharp.Math.Matrix.AffineTransformation(1.0f, ref bRot, ref bPos, out offset);

                        propObject.GetComponent<BUExtensions.BMultiShape>().AddShape(collider.CopyCollisionShape(), offset);
                        GameObject.DestroyImmediate(collider);
                    }
                }

                meshObject.transform.parent = subObject.transform;
            }

            #region Free mesh
            foreach (var list in new List<BXDAMesh.BXDASubMesh>[] { mesh.meshes, mesh.colliders })
            {
                foreach (BXDAMesh.BXDASubMesh sub in list)
                {
                    sub.verts = null;
                    sub.norms = null;
                    foreach (BXDAMesh.BXDASurface surf in sub.surfaces)
                    {
                        surf.indicies = null;
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = null;
                }
            }
            mesh = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            #endregion

            return true;
        }

        BCollisionShape CreateCollider(PropertySet.PropertySetCollider psCollider, GameObject meshObject, GameObject colliderObject)
        {
            switch (psCollider.CollisionType)
            {
                case PropertySet.PropertySetCollider.PropertySetCollisionType.BOX:
                    PropertySet.BoxCollider psBoxCollider = (PropertySet.BoxCollider)psCollider;
                    BoxCollider dummyBoxCollider = meshObject.AddComponent<BoxCollider>();

                    colliderObject.transform.localRotation = meshObject.transform.localRotation;
                    colliderObject.transform.position = meshObject.transform.TransformPoint(dummyBoxCollider.center);

                    BBoxShape boxShape = colliderObject.AddComponent<BBoxShape>();
                    boxShape.Extents = new Vector3(
                        dummyBoxCollider.size.x * 0.5f * psBoxCollider.Scale.x,
                        dummyBoxCollider.size.y * 0.5f * psBoxCollider.Scale.y,
                        dummyBoxCollider.size.z * 0.5f * psBoxCollider.Scale.z);

                    //meshObject.AddComponent<MouseListener>();
                    UnityEngine.Object.Destroy(dummyBoxCollider);

                    return boxShape;

                case PropertySet.PropertySetCollider.PropertySetCollisionType.SPHERE:
                    PropertySet.SphereCollider psSphereCollider = (PropertySet.SphereCollider)psCollider;
                    SphereCollider dummySphereCollider = meshObject.AddComponent<SphereCollider>();

                    colliderObject.transform.position = meshObject.transform.TransformPoint(dummySphereCollider.center);

                    BSphereShape sphereShape = colliderObject.AddComponent<BSphereShape>();
                    sphereShape.Radius = dummySphereCollider.radius * psSphereCollider.Scale;

                    //meshObject.AddComponent<MouseListener>();
                    UnityEngine.Object.Destroy(dummySphereCollider);

                    return sphereShape;

                case PropertySet.PropertySetCollider.PropertySetCollisionType.MESH:
                    PropertySet.MeshCollider psMeshCollider = (PropertySet.MeshCollider)psCollider;

                    if (psMeshCollider.Convex)
                    {
                        MeshCollider dummyMeshCollider = colliderObject.AddComponent<MeshCollider>();
                        dummyMeshCollider.sharedMesh = meshObject.GetComponent<MeshFilter>().mesh;

                        colliderObject.transform.position = meshObject.transform.TransformPoint(dummyMeshCollider.bounds.center);
                        colliderObject.transform.rotation = meshObject.transform.rotation;

                        BConvexHullShape hullshape = colliderObject.AddComponent<BConvexHullShape>();
                        hullshape.HullMesh = Auxiliary.GenerateCollisionMesh(meshObject.GetComponent<MeshFilter>().mesh, Vector3.zero, 0f/*CollisionMargin*/);
                        hullshape.GetCollisionShape().Margin = CollisionMargin;

                        //subObject.AddComponent<MouseListener>();
                        UnityEngine.Object.Destroy(dummyMeshCollider);

                        return hullshape;
                    }
                    else
                    {
                        colliderObject.transform.position = meshObject.transform.position;
                        colliderObject.transform.rotation = meshObject.transform.rotation;

                        BBvhTriangleMeshShape meshShape = colliderObject.AddComponent<BBvhTriangleMeshShape>();
                        meshShape.HullMesh = meshObject.GetComponent<MeshFilter>().mesh.GetScaledCopy(-1f, 1f, 1f);
                        meshShape.GetCollisionShape().Margin = CollisionMargin;

                        return meshShape;
                    }
            }

            return null;
        }
    }
}