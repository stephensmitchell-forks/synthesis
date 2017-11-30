using Assets.Scripts.BUExtensions;
using Assets.Scripts.FEA;
using Assets.Scripts.FSM;
using Assets.Scripts.Utils;
using BulletSharp;
using BulletUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RobotBase : NetworkBehaviour
{
    protected bool isInitialized;

    protected RigidNode_Base rootNode;

    protected Vector3 robotStartPosition = new Vector3(0f, 1f, 0f);
    protected BulletSharp.Math.Matrix robotStartOrientation = BulletSharp.Math.Matrix.Identity;

    protected UnityPacket unityPacket;

    public bool ControlsEnabled = true;

    protected Vector3 nodeToRobotOffset;

    public UnityPacket.OutputStatePacket Packet;

    public string RobotDirectory { get; protected set; }
    public string RobotName;

    public bool Metric { get; set; }
    public bool IsMecanum { get; set; }
    public int ControlIndex { get; set; }

    protected UnityPacket.OutputStatePacket.DIOModule[] emptyDIO = new UnityPacket.OutputStatePacket.DIOModule[2];

    protected Vector3 offset;

    protected DynamicCamera cam;

    //Robot statistics output
    public float Speed { get; protected set; }
    private float oldSpeed;
    public float Weight { get; protected set; }
    public float AngularVelocity { get; protected set; }
    public float Acceleration { get; protected set; }

    /// <summary>
    /// Called when robot is first initialized
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Called once per frame to ensure all rigid bodie components are activated
    /// </summary>
    void Update()
    {
        UpdateRobotPhysics();
    }

    /// <summary>
    /// Called once every physics step (framerate independent) to drive motor joints as well as handle the resetting of the robot
    /// </summary>
    void FixedUpdate()
    {
        UpdateRobotInfo();
    }

    /// <summary>
    /// Initializes physical robot based off of robot directory.
    /// </summary>
    /// <param name="directory">folder directory of robot</param>
    /// <returns></returns>
    public virtual bool InitializeRobot(string directory)
    {
        RobotDirectory = directory;

        RemoveAllNodes();

        if (!File.Exists(directory + "\\skeleton.bxdj"))
            return false;

        if (!CreateNodes(ReadNodeList(directory), directory))
            return false;

        RobotName = new DirectoryInfo(directory).Name;

        isInitialized = true;

        return true;
    }

    /// <summary>
    /// Update the stats for robot depending on whether it's metric or not
    /// </summary>
    public void UpdateStats()
    {
        GameObject mainNode = transform.GetChild(0).gameObject;
        //calculates stats of robot
        if (mainNode != null)
        {
            float currentSpeed = mainNode.GetComponent<BRigidBody>().GetCollisionObject().InterpolationLinearVelocity.Length;

            Speed = (float)Math.Round(Math.Abs(currentSpeed), 3);
            Weight = (float)Math.Round(GetWeight(), 3);
            AngularVelocity = (float)Math.Round(Math.Abs(mainNode.GetComponent<BRigidBody>().angularVelocity.magnitude), 3);
            Acceleration = (float)Math.Round((currentSpeed - oldSpeed) / Time.deltaTime, 3);
            oldSpeed = currentSpeed;

            if (!Metric)
            {
                Speed = (float)Math.Round(Speed * 3.28084, 3);
                Acceleration = (float)Math.Round(Acceleration * 3.28084, 3);
                Weight = (float)Math.Round(Weight * 2.20462, 3);
            }
        }
    }

    /// <summary>
    /// Get the total weight of the robot
    /// </summary>
    /// <returns></returns>
    public float GetWeight()
    {
        float weight = 0;

        foreach (Transform child in gameObject.transform)
        {
            if (child.GetComponent<BRigidBody>() != null)
            {
                weight += (float)child.GetComponent<BRigidBody>().mass;
            }
        }
        return weight;
    }

    /// <summary>
    /// Returns a list of nodes from the given directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    protected List<RigidNode_Base> ReadNodeList(string directory)
    {
        RigidNode_Base.NODE_FACTORY = delegate (Guid guid)
        {
            return new RigidNode(guid);
        };
        List<RigidNode_Base> nodes = new List<RigidNode_Base>();
        rootNode = BXDJSkeleton.ReadSkeleton(directory + "\\skeleton.bxdj");
        rootNode.ListAllNodes(nodes);

        return nodes;
    }

    /// <summary>
    /// Creates the physical robot with each of its nodes from the given robot directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    protected bool CreateNodes(List<RigidNode_Base> nodes, string directory)
    {
        transform.position = robotStartPosition; //Sets the position of the object to the set spawn point

        //Initializes the wheel variables
        int numWheels = nodes.Count(x => x.HasDriverMeta<WheelDriverMeta>() && x.GetDriverMeta<WheelDriverMeta>().type != WheelType.NOT_A_WHEEL);
        float collectiveMass = 0f;

        //Initializes the nodes
        foreach (RigidNode_Base n in nodes)
        {
            RigidNode node = (RigidNode)n;
            node.CreateTransform(transform);

            if (!node.CreateMesh(directory + "\\" + node.ModelFileName))
            {
                Debug.Log("Robot not loaded!");
                return false;
            }

            node.CreateJoint(numWheels, false);

            if (node.PhysicalProperties != null)
                collectiveMass += node.PhysicalProperties.mass;

            if (node.MainObject.GetComponent<BRigidBody>() != null)
                node.MainObject.AddComponent<Tracker>().Trace = true;
        }

        //Get the offset from the first node to the robot for new robot start position calculation
        //This line is CRITICAL to new reset position accuracy! DON'T DELETE IT!
        nodeToRobotOffset = gameObject.transform.GetChild(0).localPosition - robotStartPosition;

        foreach (BRaycastRobot r in GetComponentsInChildren<BRaycastRobot>())
        {
            r.RaycastRobot.OverrideMass = collectiveMass;
            r.RaycastRobot.RootRigidBody = (RigidBody)((RigidNode)nodes[0]).MainObject.GetComponent<BRigidBody>().GetCollisionObject();
        }

        return true;
    }

    /// <summary>
    /// Removes all nodes from the existing robot.
    /// </summary>
    protected void RemoveAllNodes()
    {
        //Deletes all nodes if any exist, take the old node transforms out from the robot object
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            //If this isn't done, the game object is destroyed but the parent-child transform relationship remains!
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Updates the robot's physics.
    /// </summary>
    protected void UpdateRobotPhysics()
    {
        BRigidBody rigidBody = GetComponentInChildren<BRigidBody>();

        if (rigidBody == null)
        {
            AppModel.ErrorToMenu("Could not generate robot physics data.");
            return;
        }

        if (!rigidBody.GetCollisionObject().IsActive)
            rigidBody.GetCollisionObject().Activate();
    }

    /// <summary>
    /// Updates the robot's motor information and stats.
    /// </summary>
    protected void UpdateRobotInfo()
    {
        if (rootNode != null && ControlsEnabled)
            DriveJoints.UpdateAllMotors(rootNode, DriveJoints.GetPwmValues(Packet == null ? emptyDIO : Packet.dio, ControlIndex, IsMecanum));
        
        UpdateStats();
    }
}

