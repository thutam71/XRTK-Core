﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Interfaces.SpatialAwarenessSystem;
using XRTK.Services;

namespace XRTK.Providers.SpatialObservers
{
    /// <summary>
    /// Base <see cref="IMixedRealitySpatialAwarenessDataProvider"/> implementation
    /// </summary>
    public abstract class BaseMixedRealitySpatialObserverDataProvider : BaseDataProvider, IMixedRealitySpatialAwarenessDataProvider
    {
        /// <inheritdoc />
        protected BaseMixedRealitySpatialObserverDataProvider(string name, uint priority, BaseMixedRealitySpatialObserverProfile profile, IMixedRealitySpatialAwarenessSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile == null)
            {
                profile = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.GlobalMeshObserverProfile;
            }

            if (profile == null)
            {
                throw new ArgumentNullException($"Missing a {profile.GetType().Name} profile for {name}");
            }

            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewObserverId();
            StartupBehavior = profile.StartupBehavior;
            UpdateInterval = profile.UpdateInterval;
            PhysicsLayer = profile.PhysicsLayer;
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverDetected(this);

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                StartObserving();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            StopObserving();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverLost(this);
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialObserverDataProvider Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        /// <inheritdoc />
        public virtual int PhysicsLayer { get; set; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        public virtual void StartObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = true;
        }

        /// <inheritdoc />
        public virtual void StopObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = false;
        }

        #endregion IMixedRealitySpatialObserverDataProvider Implementation

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; }

        #endregion IMixedRealityEventSource Implementation

        #region IEquality Implementation

        /// <summary>
        /// Determines if the specified objects are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(IMixedRealitySpatialAwarenessDataProvider left, IMixedRealitySpatialAwarenessDataProvider right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialAwarenessDataProvider)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessDataProvider other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}