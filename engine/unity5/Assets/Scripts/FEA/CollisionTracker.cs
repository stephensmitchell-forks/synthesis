﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletSharp;
using BulletUnity;
using UnityEngine;

namespace Assets.Scripts.FEA
{
    public class CollisionTracker : ICollisionCallback
    {
        private MainState mainState;
        private BPhysicsWorld physicsWorld;
        private bool newFrame;
        private int lastFrameCount;

        /// <summary>
        /// The list of contact points tracked by the CollisionTracker.
        /// </summary>
        public FixedQueue<List<ContactDescriptor>> ContactPoints { get; private set; }

        /// <summary>
        /// Creates a new CollisionTracker instance.
        /// </summary>
        /// <param name="mainState"></param>
        public CollisionTracker(MainState mainState)
        {
            this.mainState = mainState;
            physicsWorld = BPhysicsWorld.Get();
            lastFrameCount = physicsWorld.frameCount;

            ContactPoints = new FixedQueue<List<ContactDescriptor>>(Tracker.Length);
        }

        /// <summary>
        /// Resets the CollisionTracker and clears all stored contact information.
        /// </summary>
        public void Reset()
        {
            ContactPoints.Clear(null);
            lastFrameCount = physicsWorld.frameCount - 1;
        }

        /// <summary>
        /// Finds any robot collisions and adds them to the list of collisions for the current frame.
        /// </summary>
        /// <param name="pm"></param>
        public void OnVisitPersistentManifold(PersistentManifold pm)
        {
            if (!mainState.Tracking)
            {
                pm.ClearManifold();
                return;
            }

            int framesPassed = physicsWorld.frameCount - lastFrameCount;
            lastFrameCount += framesPassed;

            for (int i = 0; i < framesPassed; i++)
                ContactPoints.Add(new List<ContactDescriptor>());

            BRigidBody obA = pm.Body0.UserObject as BRigidBody;
            BRigidBody obB = pm.Body1.UserObject as BRigidBody;
            BRigidBody robotBody = obA != null && obA.gameObject.name.StartsWith("node") ? obA : obB != null && obB.gameObject.name.StartsWith("node") ? obB : null;

            if (robotBody == null)
                return;

            if (pm.NumContacts < 1)
                return;

            int numContacts = pm.NumContacts;

            for (int i = 0; i < numContacts; i++)
            {
                ManifoldPoint mp = pm.GetContactPoint(i);

                ContactDescriptor cd = new ContactDescriptor
                {
                    AppliedImpulse = mp.AppliedImpulse,
                    Position = (mp.PositionWorldOnA + mp.PositionWorldOnB) * 0.5f,
                    RobotBody = robotBody
                };

                ContactPoints[i].Add(cd);
            }

            pm.ClearManifold();
        }

        public void OnFinishedVisitingManifolds()
        {
            // Not implemented
        }

        public void BOnCollisionEnter(CollisionObject other, BCollisionCallbacksDefault.PersistentManifoldList manifoldList)
        {
            // Not implemented
        }

        public void BOnCollisionExit(CollisionObject other)
        {
            // Not implemented
        }

        public void BOnCollisionStay(CollisionObject other, BCollisionCallbacksDefault.PersistentManifoldList manifoldList)
        {
            // Not implemented
        }
    }
}
