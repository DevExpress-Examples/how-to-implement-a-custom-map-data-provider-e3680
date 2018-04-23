﻿Imports Microsoft.VisualBasic
Imports System
Imports System.Windows
Imports System.Globalization
Imports DevExpress.Xpf.Map


Namespace CustomMapDataProviderApp

    Partial Public Class MainWindow
        Inherits Window
        Public Sub New()
            InitializeComponent()
            imageTilesLayer.DataProvider = New CustomMapDataProvider()
        End Sub
    End Class

    Public Class CustomMapDataProvider
        Inherits MapDataProviderBase
        Private ReadOnly projection_Renamed As New SphericalMercatorProjection()

        Public Overrides ReadOnly Property Projection() As IProjection
            Get
                Return projection_Renamed
            End Get
        End Property

        Public Sub New()
            SetTileSource(New CustomTileSource())
        End Sub
        Protected Overrides Function CreateObject() As MapDependencyObject
            Return New CustomMapDataProvider()
        End Function
        Public Overrides Function GetMapSizeInPixels(ByVal zoomLevel As Integer) As Size
            Return New Size(Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize, _
                            Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize)
        End Function
    End Class

    Public Class CustomTileSource
        Inherits MapTileSourceBase
        Private Const roadUrlTemplate As String = _
            "http://{subdomain}.tile.openstreetmap.org/{tileLevel}/{tileX}/{tileY}.png"
        Public Const maxZoomLevel As Integer = 20
        Public Const tileSize As Integer = 256

        Private Shared imageWidth As Integer = CInt(Fix(Math.Pow(2.0, maxZoomLevel))) * tileSize
        Private Shared imageHeight As Integer = CInt(Fix(Math.Pow(2.0, maxZoomLevel))) * tileSize
        Private Shared subdomains() As String = {"a", "b", "c"}

        Public Sub New()
            MyBase.New(imageWidth, imageHeight, tileSize, tileSize)

        End Sub
        Public Overrides Function GetTileByZoomLevel(ByVal zoomLevel As Integer, _
                                                     ByVal tilePositionX As Integer, _
                                                     ByVal tilePositionY As Integer) As Uri
            Dim url As String = roadUrlTemplate
            url = url.Replace("{tileX}", tilePositionX.ToString(CultureInfo.InvariantCulture))
            url = url.Replace("{tileY}", tilePositionY.ToString(CultureInfo.InvariantCulture))
            url = url.Replace("{tileLevel}", zoomLevel.ToString(CultureInfo.InvariantCulture))
            url = url.Replace("{subdomain}", subdomains(GetSubdomainIndex(subdomains.Length)))
            Return New Uri(url)
        End Function
    End Class
End Namespace
