using Blundergat.Common.Settings.Impl;
using Microsoft.Extensions.Logging;

namespace Blundergat.Common.Settings.Interfaces
{
	public interface ISettingsProvider 
	{
		void DumpSettingsInfo(ILogger logger);

		void Save();

		void RestoreMissingSettingsToDefault(bool force = false);

		DataSettings DataSettings { get; set; }
	}
}