// -----------------------------------------------------------------------
// <copyright file="NoneRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;

    /// <summary>
    /// Defines a role that represents players with no role.
    /// </summary>
    public class NoneRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneRole"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public NoneRole(Player owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.None;
    }
}