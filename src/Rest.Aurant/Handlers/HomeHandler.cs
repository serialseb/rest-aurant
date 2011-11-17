using System;
using Rest.Aurant.Resources;
using OpenRasta.Web;

namespace Rest.Aurant.Handlers
{
    public class HomeHandler
    {
        public Home Get()
        {
            return new Home { Message = "Hello world" };
        }
    }
}