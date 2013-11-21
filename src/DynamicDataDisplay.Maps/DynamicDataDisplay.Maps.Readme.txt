This is a part of WPF DynamicDataDisplay project - http://dynamicdatadisplay.codeplex.com/

Contains controls for displaying 2d tiled maps, downloaded from network tile server or loaded from disk.

Uses Shader Effects BuildTask from http://wpf.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=14962 to build shader-powered MercatorShaderMap, where
pixel shader is used to perform non-linear scale of map's tiles.

Contains sample network tile server - OpenStreetMapServer - which downloads tiles from http://www.openstreetmap.org/.
For more info, read comments of OpenStreetMapServer.

Version 0.1.0.0: initial release.