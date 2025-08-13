using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Entities
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Comment("主键")]
        [MaxLength(32)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TPrimaryKey Id { get; set; }

        /// <summary>
        /// 并发令牌
        /// </summary>
        [Comment("并发令牌")]
        [MaxLength(32)]
        public virtual string ConcurrencyToken { get; set; }

        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default))
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
