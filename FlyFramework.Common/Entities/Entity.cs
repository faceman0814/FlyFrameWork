using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Entities
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        [MaxLength(32)]
        public virtual TPrimaryKey Id { get; set; }
        [MaxLength(32)]
        public virtual string ConcurrencyToken { get; set; }

        //
        // 摘要:
        //     用于检查实体是否是瞬态的（即没有主键或主键为默认值）。
        //
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default(TPrimaryKey)))
            {
                return true;
            }

            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }

        /// <summary>
        /// 用于比较两个实体是否相等，主要通过比较主键来实现。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public virtual bool EntityEquals(object obj)
        {
            if (obj == null || !(obj is Entity<TPrimaryKey>))
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            Entity<TPrimaryKey> entity = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && entity.IsTransient())
            {
                return false;
            }

            Type type = GetType();
            Type type2 = entity.GetType();
            if (!type.GetTypeInfo().IsAssignableFrom(type2) && !type2.GetTypeInfo().IsAssignableFrom(type))
            {
                return false;
            }

            //Todo:实体多租户处理
            //if (this is IMayHaveTenant && entity is IMayHaveTenant && this.As<IMayHaveTenant>().TenantId != entity.As<IMayHaveTenant>().TenantId)
            //{
            //    return false;
            //}

            //if (this is IMustHaveTenant && entity is IMustHaveTenant && this.As<IMustHaveTenant>().TenantId != entity.As<IMustHaveTenant>().TenantId)
            //{
            //    return false;
            //}

            return Id.Equals(entity.Id);
        }

        public override string ToString()
        {
            return $"[{GetType().Name} {Id}]";
        }
    }
}
