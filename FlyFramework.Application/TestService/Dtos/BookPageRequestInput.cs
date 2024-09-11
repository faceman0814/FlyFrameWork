using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.TestService.Dtos
{
    public class BookPageRequestInput
    {
        public string Title { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
