namespace Blundergat.Io
{
	public interface IAssetImporterExporterFactory
	{
		IAssetImporterExporter GetImporterExporter(string filePath);

		IAssetImporterExporter GetImporterExporter(AssetImporterExporterType assetImporterExporterType);
	}
}