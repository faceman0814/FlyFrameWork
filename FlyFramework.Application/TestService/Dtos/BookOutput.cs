using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.TestService.Dtos
{
    public class BookOutput
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryId { get; set; }
        public string Id { get; set; }
    }
}
