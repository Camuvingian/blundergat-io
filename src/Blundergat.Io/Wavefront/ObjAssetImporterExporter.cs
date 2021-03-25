using Blundergat.Common.Helpers;
using Blundergat.Common.Model.Io;
using Blundergat.Common.Settings.Interfaces;
using Blundergat.Io.Wavefront.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using SystemColor = System.Drawing.Color;

namespace Blundergat.Io.Wavefront
{
	[AssetImporterExporter(".obj", "Wavefront Technologies OBJ")]
	public class ObjAssetImporterExporter : AssetImporterExporterBase, IObjAssetImporterExporter
	{
		public ObjAssetImporterExporter(
			ISettingsProvider settingsProvider,
			ILogger<ObjAssetImporterExporter> logger) : base(settingsProvider, logger)
		{
		}

		#region AssetImporterExporterBase Implementation.
		public override Task<Scene> ImportFileImpl(string filePath)
		{
			return Task.Run(async () =>
			{
				var wavefrontObject = await WavefrontObject.LoadAsync(filePath);
				var defaultMaterial = new Material
				{
					Name = "Default",
					DiffuseColor = SystemColor.Gray.ToCustomColor(),
					SpecularColor = SystemColor.White.ToCustomColor(),
					SpecularExponent = 64
				};

				var scene = new Scene();
				foreach (Group group in wavefrontObject.Groups)
				{
					Mesh mesh = new Mesh();
					scene.Meshes.Add(mesh);

					Material material;
					Material groupMaterial = group.Material;

					if (groupMaterial != null)
					{
						material = new Material
						{
							Name = groupMaterial.Name,
							FileName = groupMaterial.FileName
						};

						if (!string.IsNullOrEmpty(groupMaterial.TextureName))
							material.DiffuseTextureName = groupMaterial.TextureName;
						else
						{
							material.DiffuseColor = (groupMaterial.DiffuseColor != null)
								? groupMaterial.DiffuseColor
								: SystemColor.Gray.ToCustomColor();
						}

						material.SpecularColor = (groupMaterial.SpecularColor != null)
							? groupMaterial.SpecularColor
							: SystemColor.Black.ToCustomColor();
						material.SpecularExponent = (int)groupMaterial.SpecularExponent; // TODO: Check
					}
					else
					{
						material = defaultMaterial;
					}
					mesh.Material = material;
					if (!scene.Materials.Contains(material))
						scene.Materials.Add(material);

					// Currently this is not utilising indices.
					int counter = 0;
					foreach (Face f in group.Faces)
					{
						// Copy normals.
						foreach (int normalIndex in f.NormalIndices)
						{
							Vector3 n = wavefrontObject.Normals[normalIndex];
							mesh.Normals.Add(new Vector3(n.X, n.Y, n.Z));
						}

						// Copy texture coordinates.
						foreach (int textureIndex in f.TextureIndices)
						{
							if (wavefrontObject.Textures.Count > textureIndex)
							{
								TextureCoordinate tc = wavefrontObject.Textures[textureIndex];
								mesh.TextureCoordinates.Add(new Vector3(tc.U, tc.V, tc.W));
							}
						}

						switch (f.Type)
						{
							case FaceType.Triangles:
								// Copy positions.
								foreach (int vertexIndex in f.VertexIndices)
								{
									Vector3 v = wavefrontObject.Vertices[vertexIndex];
									mesh.Points.Add(new Vector3(v.X, v.Y, v.Z));
								}
								mesh.Indices.Add(counter++);
								mesh.Indices.Add(counter++);
								mesh.Indices.Add(counter++);
								//mesh.Indices.AddRange(f.VertexIndices);
								break;
							default:
								throw new NotSupportedException();
						}
					}

					foreach (Vector3 v in wavefrontObject.Vertices)
						mesh.Points.Add(new Vector3(v.X, v.Y, v.Z));

					foreach (Vector3 v in wavefrontObject.Normals)
						mesh.Normals.Add(new Vector3(v.X, v.Y, v.Z));

					foreach (TextureCoordinate tc in wavefrontObject.Textures)
						mesh.TextureCoordinates.Add(new Vector3(tc.U, tc.V, 0));
				}
				return scene;
			});
		}

		// TODO. This is a poor mans exporter. We need to finish off the implementation and write a 
		// comprehensive vesion - or do we? o_O
		protected override Task ExportFileImpl(Scene scene, string filePath)
		{
			if (scene.Meshes == null || scene.Meshes.Count != 1)
				throw new ArgumentException("Can only deal with scenes containing a single mesh");

			return Task.Run(() =>
			{
				using (var fileStream = File.OpenWrite(filePath))
				using (var streamWriter = new StreamWriter(fileStream))
				{
					var mesh = scene.Meshes[0];
					WriteHeader(streamWriter, mesh);

					// Vertices.
					foreach (var point in mesh.Points)
						streamWriter.WriteLine($"v {point.X} {point.Y} {point.Z}");

					// Normals. 
					foreach (var normal in mesh.Normals)
						streamWriter.WriteLine($"vn {normal.X} {normal.Y} {normal.Z}");

					streamWriter.Close();
				}
				File.SetCreationTime(filePath, DateTime.Now);
			});
		}

		private void WriteHeader(StreamWriter writer, Mesh mesh)
		{
			foreach (var line in GetBanner(mesh))
				writer.WriteLine($"#{line}");
		}

		public override string Extension => ".obj";
		#endregion // AssetImporterExporterBase Implementation.
	}
}