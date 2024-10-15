// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FlyFramework.Seed.Host
{

    public class InitialHostDbBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FlyFrameworkDbContext _context;

        public InitialHostDbBuilder(FlyFrameworkDbContext context,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public void Create()
        {
            new HostRoleAndUserCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}
