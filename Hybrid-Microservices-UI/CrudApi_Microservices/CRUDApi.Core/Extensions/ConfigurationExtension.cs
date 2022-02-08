using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUD.Transaction.CRUDApi.Core.Extensions
{
	public static class ConfigurationExtension
	{
		/// <summary>
		/// Returns configuration value for given key or throws exception if key not found
		/// </summary>
		/// <param name="key">string value for given configuration.key value</param>
		/// <returns></returns>
		public static string GetAppSettingValue(this IConfiguration config, string key)
		{
			try
			{
				var value = config.GetValue<string>(key);
				if (string.IsNullOrEmpty(value))
				{
					throw new NullReferenceException($"Configuration error accessing key:  {key.ToString()}");
				}
				return value;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Error accessing configuration information for key {key}", ex);
			}
		}
	}
}
