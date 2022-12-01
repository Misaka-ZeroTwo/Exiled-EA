// -----------------------------------------------------------------------
// <copyright file="RespawningTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before spawning a wave of <see cref="SpawnableTeamType.NineTailedFox" /> or
    ///     <see cref="SpawnableTeamType.ChaosInsurgency" />.
    /// </summary>
    public class RespawningTeamEventArgs : IDeniableEvent
    {
        private SpawnableTeamType nextKnownTeam;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RespawningTeamEventArgs" /> class.
        /// </summary>
        /// <param name="players">
        ///     <inheritdoc cref="Players" />
        /// </param>
        /// <param name="maxRespawn">
        ///     <inheritdoc cref="MaximumRespawnAmount" />
        /// </param>
        /// <param name="nextKnownTeam">
        ///     <inheritdoc cref="NextKnownTeam" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public RespawningTeamEventArgs(List<Player> players, int maxRespawn, SpawnableTeamType nextKnownTeam, bool isAllowed = true)
        {
            Players = players;
            MaximumRespawnAmount = Mathf.Min(
                maxRespawn,
                NextKnownTeam != SpawnableTeamType.None ? RespawnManager.SpawnableTeams[NextKnownTeam].MaxWaveSize : 0);

            this.nextKnownTeam = nextKnownTeam;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the list of players that are going to be respawned.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        ///     Gets or sets the maximum amount of respawnable players.
        /// </summary>
        public int MaximumRespawnAmount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the next respawnable team is.
        /// </summary>
        public SpawnableTeamType NextKnownTeam
        {
            get => nextKnownTeam;
            set
            {
                nextKnownTeam = value;
                ReissueNextKnownTeam();
            }
        }

        /// <summary>
        ///     Gets the current spawnable team.
        /// </summary>
        public SpawnableTeamHandlerBase SpawnableTeam
            => RespawnManager.SpawnableTeams.TryGetValue(NextKnownTeam, out SpawnableTeamHandlerBase @base) ? @base : null;

        /// <summary>
        ///     Gets or sets a value indicating whether or not the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; }

        private void ReissueNextKnownTeam()
        {
            SpawnableTeamHandlerBase @base = SpawnableTeam;

            if (@base is null)
                return;

            // Refer to the game code
            int availableTickets = 0;

            for (int i = 0; i < RespawnTokensManager._teamsCount; i++)
            {
                if (NextKnownTeam == RespawnTokensManager.Counters[i].Team)
                    availableTickets = (int)RespawnTokensManager.Counters[i].Amount;
            }

            if (availableTickets == 0)
            {
                availableTickets = 5;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, availableTickets);
            }

            MaximumRespawnAmount = Mathf.Min(
                availableTickets,
                NextKnownTeam != SpawnableTeamType.None ? RespawnManager.SpawnableTeams[NextKnownTeam].MaxWaveSize : 0);
        }
    }
}