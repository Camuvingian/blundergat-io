# Blundergat.Io

A import/export library for 3D assets written using net5.0. Supported formats are Lightwave OBJ (.obj) and Polygon File Format PLY (.ply). This is the core read/write library used in the Blundergat framework. This project was helped by both Meshellator (https://github.com/tgjones/meshellator) for core formatting and PlyImporter (https://github.com/3DBear/PlyImporter) for the BinaryLittleEndian .ply support. 

# How do I use this to import assets? 

Call without logging

    var importerExporter = new PlyAssetImporterExporter(settings, null);
    var scene = await importerExporter.ImportFileAsync(filePath);

# How do I use this to export assets? 

Call without logging, assume we already have`IPointCloud pointCloud`

    var scene = PointCloudToSceneAdapter.GenerateSceneFromPointCloud(pointCloud);

    var settings = new IoSettings(); // Neet to implement a IIoSettings class!
    settings.DefaultPlyFileFormat = plyFileFormat;

    var importerExporter = new PlyAssetImporterExporter(settings, null);
    await importerExporter.ExportFileAsync(scene, filePath, true);

# Setup to use a factory to handle multiple asset types at runtime 

    var assetImporterExporterFactory = AssetImporterExporterFactory.Instance(
        new List<(AssetImporterExporterType, IAssetImporterExporter)>()
            {
                (AssetImporterExporterType.Ply, new PlyAssetImporterExporter(settings, null)),
                (AssetImporterExporterType.Obj, new ObjAssetImporterExporter(settings, null))
            });

Can get the importer/exporter using enum value

    var assetImporterExporter = _assetImporterExporterFactory.GetImporterExporter(AssetImporterExporterType.Ply);

Or a file path string

    var assetImporterExporter = _assetImporterExporterFactory.GetImporterExporter(filePath);
