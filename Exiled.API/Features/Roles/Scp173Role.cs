// -----------------------------------------------------------------------
// <copyright file="Scp173Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;

    /// <summary>
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp173Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp173Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        internal Scp173Role(Player owner)
            : base(owner)
        {
            SubroutineModule = (Base as Scp173GameRole).SubroutineModule;
            MovementModule = FirstPersonController.FpcModule as Scp173MovementModule;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-173.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp173;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability) && ability.IsObserved;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IReadOnlyCollection<Player> ObservingPlayers
        {
            get
            {
                HashSet<Player> players = new();

                if (SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability))
                {
                    foreach (var player in ability.Observers)
                        players.Add(Player.Get(player));
                }

                return players;
            }
        }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp173MovementModule MovementModule { get; }

        /// <summary>
        /// Gets SCP-173's max move speed.
        /// </summary>
        public float MaxMovementSpeed => MovementModule.MaxMovementSpeed;

        /// <summary>
        /// Gets or sets the SCP-173's movement speed.
        /// </summary>
        public float MovementSpeed
        {
            get => MovementModule.ServerSpeed;
            set => MovementModule.MovementSpeed = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) && ability.AbilityReady;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                {
                    ability._endSustainTime = -1;
                    ability._totalCooldown = 0;
                    ability._initialStopTime = NetworkTime.time;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) ? ability.RemainingBlinkCooldown : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                {
                    ability._initialStopTime = NetworkTime.time;
                    ability._totalCooldown *= ability._breakneckSpeedsAbility.IsActive ? 2 : 1;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => SubroutineModule.TryGetSubroutine(out Scp173TeleportAbility ability) ? ability.EffectiveBlinkDistance : 0;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) && ability._breakneckSpeedsAbility.IsActive;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                    ability._breakneckSpeedsAbility.IsActive = true;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breackneck speed again.
        /// </summary>
        public float BreakneckCooldown
        {
            get => 40; // It's hardcoded
            set { }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float TantrumCooldown
        {
            get => 30; // It's hardcoded
            set { }
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <param name="failIfObserved">Whether or not to place the tantrum if SCP-173 is currently being viewed.</param>
        /// <returns>The tantrum's <see cref="UnityEngine.GameObject"/>, or <see langword="null"/> if it cannot be placed.</returns>
        public UnityEngine.GameObject Tantrum(bool failIfObserved = false)
        {
            if (failIfObserved && IsObserved)
                return null;

            return Owner.PlaceTantrum();
        }
    }
}