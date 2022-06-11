using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Constants
{
    public class AuthorizationConstants
    {
        public const string DEFAULT_PASSWORD = "Pass@word123";

        public static class Roles
        {
            public const string ADMIN = "Admin";
            public const string CLIENT = "Client";
        }
    }
}
