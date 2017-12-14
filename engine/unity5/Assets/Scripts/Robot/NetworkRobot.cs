using Assets.Scripts.BUExtensions;
using Assets.Scripts.FSM;
using BulletSharp;
using BulletUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.025f)]
public class NetworkRobot : RobotBase
{
    BRigidBody[] rigidBodies;
    bool correctionEnabled = true;
    bool canSendUpdate = true;
    bool updateTransform = false;

    MultiplayerState state;

    private void Start()
    {
        string directory = PlayerPrefs.GetString("simSelectedRobot");

        if (!string.IsNullOrEmpty(directory))
        {
            state = StateMachine.Instance.FindState<MultiplayerState>();
            state.LoadRobot(this, directory, isLocalPlayer);
            rigidBodies = GetComponentsInChildren<BRigidBody>();

            if (!isServer)
                foreach (BRigidBody rb in rigidBodies)
                    rb.gameObject.AddComponent<NetworkMesh>();

            UpdateRobotInfo();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
            correctionEnabled = true;
        else if (Input.GetKey(KeyCode.D))
            correctionEnabled = false;

        BRigidBody rigidBody = GetComponentInChildren<BRigidBody>();

        if (rigidBody == null)
            return;

        UpdateRobotPhysics();

        canSendUpdate = true;
    }

    private void FixedUpdate()
    {
        BRigidBody rigidBody = GetComponentInChildren<BRigidBody>();

        if (rigidBody == null)
            return;

        if (isLocalPlayer)
        {
            float[] pwm = DriveJoints.GetPwmValues(Packet == null ? emptyDIO : Packet.dio, ControlIndex, IsMecanum);

            if (!isServer)
                UpdateRobotInfo(pwm);

            CmdUpdateRobotInfo(pwm);
        }

        if (isServer && canSendUpdate)
        {
            float[] transforms = new float[rigidBodies.Length * 13];

            int i = 0;
            foreach (BRigidBody rb in rigidBodies)
            {
                float[] currentTransform = SerializeTransform(rb.GetCollisionObject().WorldTransform);

                for (int j = 0; j < currentTransform.Length; j++)
                    transforms[i * 13 + j] = currentTransform[j];

                float[] currentLinearVelocity = rb.GetCollisionObject().InterpolationLinearVelocity.ToArray();

                for (int j = 0; j < currentLinearVelocity.Length; j++)
                    transforms[i * 13 + currentTransform.Length + j] = currentLinearVelocity[j];

                float[] currentAngularVelocity = rb.GetCollisionObject().InterpolationAngularVelocity.ToArray();

                for (int j = 0; j < currentAngularVelocity.Length; j++)
                    transforms[i * 13 + currentTransform.Length + currentLinearVelocity.Length + j] = currentAngularVelocity[j];

                i++;
            }

            RpcUpdateTransforms(transforms);
        }

        canSendUpdate = false;
    }

    [Command]
    private void CmdUpdateRobotInfo(float[] pwm)
    {
        if (rootNode != null && ControlsEnabled)
            DriveJoints.UpdateAllMotors(rootNode, pwm);

        UpdateStats();
    }

    [ClientRpc]
    void RpcUpdateTransforms(float[] transforms)
    {
        if (isServer || !correctionEnabled)
            return;

        bool updateTransformNextFrame = false;
        int i = 0;
        foreach (BRigidBody rb in rigidBodies)
        {
            float[] rawTransform = new float[7];

            for (int j = 0; j < rawTransform.Length; j++)
                rawTransform[j] = transforms[i * 13 + j];

            BulletSharp.Math.Matrix currentTransform = DeserializeTransform(rawTransform);

            float[] rawLinearVelocity = new float[3];

            for (int j = 0; j < rawLinearVelocity.Length; j++)
                rawLinearVelocity[j] = transforms[i * 13 + rawTransform.Length + j];

            float[] rawAngularVelocity = new float[3];

            for (int j = 0; j < rawAngularVelocity.Length; j++)
                rawAngularVelocity[j] = transforms[i * 13 + rawTransform.Length + rawLinearVelocity.Length + j];

            BulletSharp.Math.Vector3 linearVelocity = new BulletSharp.Math.Vector3(rawLinearVelocity);
            BulletSharp.Math.Vector3 angularVelocity = new BulletSharp.Math.Vector3(rawAngularVelocity);

            int rtt = state.Network.client.GetRTT();

            currentTransform.Origin += linearVelocity * rtt * 0.001f;
            currentTransform.Orientation.Rotate(angularVelocity * rtt * 0.001f);

            if (!updateTransform && (rb.GetCollisionObject().WorldTransform.Origin - currentTransform.Origin).Length > 0.025f)
                updateTransformNextFrame = true;

            if (updateTransform)
            {
                rb.GetComponent<NetworkMesh>().UpdateMeshTransform(currentTransform.Origin.ToUnity(), currentTransform.Orientation.ToUnity());

                rb.GetCollisionObject().WorldTransform = currentTransform;
                rb.GetCollisionObject().InterpolationLinearVelocity = linearVelocity;
                rb.GetCollisionObject().InterpolationAngularVelocity = angularVelocity;
            }

            i++;
        }

        updateTransform = updateTransformNextFrame;
    }

    /// <summary>
    /// Serializes the given Matrix into an array of floats.
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    private float[] SerializeTransform(BulletSharp.Math.Matrix matrix)
    {
        return new float[]
        {
            matrix.Origin.X,
            matrix.Origin.Y,
            matrix.Origin.Z,
            matrix.Orientation.X,
            matrix.Orientation.Y,
            matrix.Orientation.Z,
            matrix.Orientation.W
        };
    }

    /// <summary>
    /// Deserializes the given array of floats in to a Matrix.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    private BulletSharp.Math.Matrix DeserializeTransform(float[] transform)
    {
        if (transform.Length != 7)
            return BulletSharp.Math.Matrix.Identity;

        return new BulletSharp.Math.Matrix
        {
            Origin = new BulletSharp.Math.Vector3(transform[0], transform[1], transform[2]),
            Orientation = new BulletSharp.Math.Quaternion(transform[3], transform[4], transform[5], transform[6])
        };
    }

    /// <summary>
    /// Serializes the given RigidBody linear velocity into an array of floats.
    /// </summary>
    /// <param name="rigidBody"></param>
    /// <returns></returns>
    private float[] SerializeVector3(BulletSharp.Math.Vector3 vec)
    {
        return new float[]
        {
            vec.X,
            vec.Y,
            vec.Z
        };
    }
}
