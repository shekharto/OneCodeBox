using System;
using System.Collections.Generic;
using System.Text;

namespace CRUD.Transaction.Types
{
    /// <summary>
    /// Type representing Api project configuration settings custom for application
    /// </summary>
    public static class ConfigurationKeyType
    {
        public class App
        {
            public const string CommandTimeout = "App:CommandTimeout";
            public const string UseHsts = "App:UseHsts";
            public const string SwaggerEnabled = "App:SwaggerEnabled";
            public const string DbConnectionSource = "App:DbConnectionSource";
        }
        // root values
        public const string AllowedHosts = "AllowedHosts";

    }
}
