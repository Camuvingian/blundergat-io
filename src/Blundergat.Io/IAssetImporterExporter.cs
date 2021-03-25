using Blundergat.Common.Model.Io;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Blundergat.Io
{
	public interface IAssetImporterExporter : IAssetImporter, IAssetExporter 
	{
		string CorrectFilePath(string filePath);

		string Extension { get; }
	}

	public interface IAssetImporter
	{
		Task<Scene> ImportFileAsync(string filePath);

		event EventHandler<FileInfo> ImportCompleted;
	}

	public interface IAssetExporter
	{
		Task ExportFileAsync(Scene scene, string filePath, bool overwrite);

		event EventHandler<FileInfo> ExportCompleted;
	}
}