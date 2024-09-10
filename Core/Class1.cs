using Core.Extentions.DynamicWebAPI;

using Microsoft.AspNetCore.Mvc;

namespace Core
{
    public class Class1 : IApplicationService
    {
        public string Hello()
        {
            return "Hello from Class1";
        }
    }
}
