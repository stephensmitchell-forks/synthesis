using BulletUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetworkMesh : MonoBehaviour
{
    private const float CorrectionThreshold = 0.9f;

    private BRigidBody bRigidBody;

    private Vector3 deltaPosition;
    private Quaternion deltaRotation;

    private float interpolationFactor;

    /// <summary>
    /// Updates the NetworkMesh offset from the given new position and rotations.
    /// </summary>
    /// <param name="newPosition"></param>
    /// <param name="newRotation"></param>
    public void UpdateMeshTransform(Vector3 newPosition, Quaternion newRotation)
    {
        deltaPosition = newPosition - transform.position;
        deltaRotation = newRotation * transform.rotation;

        interpolationFactor = 1.0f;
    }

    /// <summary>
    /// Initializes the NetworkMesh
    /// </summary>
    private void Start()
    {
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;

        interpolationFactor = 0.0f;

        bRigidBody = GetComponent<BRigidBody>();
    }

    /// <summary>
    /// Updates the interpolation factor, which slowly moves the visible mesh to the position of the robot.
    /// </summary>
    private void FixedUpdate()
    {
        interpolationFactor *= CorrectionThreshold;
    }

    /// <summary>
    /// Updates the transform of the visible mesh.
    /// </summary>
    private void Update()
    {
        transform.position = bRigidBody.GetCollisionObject().WorldTransform.Origin.ToUnity() - deltaPosition * interpolationFactor;

        Quaternion currentRotation = bRigidBody.GetCollisionObject().WorldTransform.Orientation.ToUnity();
        transform.rotation = Quaternion.Inverse(Quaternion.Lerp(currentRotation, currentRotation * Quaternion.Inverse(deltaRotation), interpolationFactor));
    }
}
