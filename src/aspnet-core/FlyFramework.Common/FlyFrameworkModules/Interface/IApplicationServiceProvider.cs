﻿namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IApplicationServiceProvider
    {
        void Initialize(IServiceProvider serviceProvider);

        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}