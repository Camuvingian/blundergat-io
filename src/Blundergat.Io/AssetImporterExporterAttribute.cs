using System;
using System.ComponentModel.Composition;

namespace Blundergat.Io
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AssetImporterExporterAttribute : ExportAttribute, IAssetImporterExporterMetadata
	{
		public AssetImporterExporterAttribute(string extension, string name)
			: base(typeof(IAssetImporterExporter))
		{
			Extension = extension;
			Name = name;
		}

		public string Name { get; set; }
		public string Extension { get; set; }
	}
}