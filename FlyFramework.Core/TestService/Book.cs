using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Core.TestService
{
    public class Book
    {
        public Book()
        {
            Title = string.Empty;
            ISBN = string.Empty;
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }

        public long CategoryId { get; set; }

        //导航属性
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }



}
