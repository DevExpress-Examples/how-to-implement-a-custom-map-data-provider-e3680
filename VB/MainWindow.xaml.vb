Imports System
Imports System.Globalization
Imports System.Net
Imports System.Windows
Imports DevExpress.Xpf.Map

Namespace CustomMapDataProviderApp

	Partial Public Class MainWindow
		Inherits Window

		Public Sub New()
			InitializeComponent()
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
			Dim provider As New CustomMapDataProvider()
			imageLayer.DataProvider = provider
			AddHandler provider.WebRequest, AddressOf Provider_WebRequest
		End Sub

		Private Sub Provider_WebRequest(ByVal sender As Object, ByVal e As MapWebRequestEventArgs)
			e.UserAgent = "Test Map App"
		End Sub
	End Class

	Public Class CustomMapDataProvider
		Inherits MapDataProviderBase

'INSTANT VB NOTE: The field projection was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly projection_Conflict As New SphericalMercatorProjection()

		Public Overrides ReadOnly Property Projection() As ProjectionBase
			Get
				Return projection_Conflict
			End Get
		End Property

		Public Sub New()
			SetTileSource(New CustomTileSource())
		End Sub
		Protected Overrides Function CreateObject() As MapDependencyObject
			Return New CustomMapDataProvider()
		End Function
		Public Overrides Function GetMapSizeInPixels(ByVal zoomLevel As Double) As Size
			Return New Size(Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize, Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize)
		End Function
	End Class

	Public Class CustomTileSource
		Inherits MapTileSourceBase

		Private Const roadUrlTemplate As String = "http://{subdomain}.tile.openstreetmap.org/{tileLevel}/{tileX}/{tileY}.png"
		Public Const maxZoomLevel As Integer = 20
		Public Const tileSize As Integer = 256

		Private Shared imageWidth As Integer = CInt(Math.Truncate(Math.Pow(2.0, maxZoomLevel))) * tileSize
		Private Shared imageHeight As Integer = CInt(Math.Truncate(Math.Pow(2.0, maxZoomLevel))) * tileSize
		Private Shared subdomains() As String = { "a", "b", "c" }

		Public Sub New()
			MyBase.New(imageWidth, imageHeight, tileSize, tileSize)

		End Sub
		Public Overrides Function GetTileByZoomLevel(ByVal zoomLevel As Integer, ByVal tilePositionX As Long, ByVal tilePositionY As Long) As Uri
			Dim url As String = roadUrlTemplate
			url = url.Replace("{tileX}", tilePositionX.ToString(CultureInfo.InvariantCulture))
			url = url.Replace("{tileY}", tilePositionY.ToString(CultureInfo.InvariantCulture))
			url = url.Replace("{tileLevel}", zoomLevel.ToString(CultureInfo.InvariantCulture))
			url = url.Replace("{subdomain}", subdomains(GetSubdomainIndex(subdomains.Length)))
			Return New Uri(url)
		End Function
	End Class
End Namespace
