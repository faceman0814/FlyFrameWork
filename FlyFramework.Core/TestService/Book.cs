using FlyFramework.Common.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Core.TestService
{
    public class Book : IEntity<string>
    {
        public Book()
        {
            Title = string.Empty;
            ISBN = string.Empty;
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }

        public string CategoryId { get; set; }

        //导航属性
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public bool IsTransient()
        {
            throw new NotImplementedException();
        }

        public IEntity<string> JsonClone()
        {
            throw new NotImplementedException();
        }
    }



}
