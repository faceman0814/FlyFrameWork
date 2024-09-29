using FlyFramework.Repositories;

using System;

namespace FlyFramework.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private IUnitOfWork _currentUnitOfWork;
        private readonly IDbContextProvider _dbContextProvider;
        public UnitOfWorkManager(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }
        // 获取当前的工作单元，如果不存在，则返回null。
        public IUnitOfWork Current => _currentUnitOfWork;

        /// <summary>
        /// 开始一个新的工作单元，并设置为当前工作单元。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IUnitOfWork Begin()
        {
            if (_currentUnitOfWork != null)
            {
                throw new InvalidOperationException("There is already an active unit of work.");
            }

            _currentUnitOfWork = new UnitOfWork(_dbContextProvider);
            return _currentUnitOfWork;
        }

        /// <summary>
        /// 创建一个新的工作单元，但不设置为当前工作单元。
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork Reserve()
        {
            // 创建一个新的UnitOfWork实例但不设置为当前工作单元
            return new UnitOfWork(_dbContextProvider);
        }
    }
}
