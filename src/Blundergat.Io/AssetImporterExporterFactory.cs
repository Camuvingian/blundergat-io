using Blundergat.Common.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Blundergat.Io
{
	public class AssetImporterExporterFactory : IAssetImporterExporterFactory
	{
		private static AssetImporterExporterFactory _instance;
		private readonly Dictionary<AssetImporterExporterType, IAssetImporterExporter> _importerExporterDict;

		public static AssetImporterExporterFactory Instance(
			IEnumerable<(AssetImporterExporterType, IAssetImporterExporter)> assetImporterExporterCollection)
		{
			if (_instance == null)
				_instance = new AssetImporterExporterFactory(assetImporterExporterCollection);
			
			return _instance;
		}

		private AssetImporterExporterFactory(IEnumerable<(AssetImporterExporterType, IAssetImporterExporter)> assetImporterExporterCollection)
		{
			_importerExporterDict = new Dictionary<AssetImporterExporterType, IAssetImporterExporter>();
			foreach (var t in assetImporterExporterCollection)
				_importerExporterDict.Add(t.Item1, t.Item2);
		}

		public IAssetImporterExporter GetImporterExporter(string filePath)
		{
			string ext = Path.GetExtension(filePath).Trim('.').ToLowerInvariant();
			TextInfo info = CultureInfo.CurrentCulture.TextInfo;

			var importerExporterType = info.ToTitleCase(ext).ToEnum<AssetImporterExporterType>();
			return GetImporterExporter(importerExporterType);
		}

		public IAssetImporterExporter GetImporterExporter(AssetImporterExporterType assetImporterExporterType)
		{
			return _importerExporterDict[assetImporterExporterType];
		}
	}
}