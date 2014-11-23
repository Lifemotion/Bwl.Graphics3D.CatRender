Public Class Material
    Public name As String
    Public textureName As String
    Public textureUsed As Boolean
    Public texturePixels() As PixelSurface
    Private materialUID As Integer
    Public maximumMipMapLevel As Integer
    Sub New()
        '        maximumMipMapLevel = 0
        materialUID = Int(Rnd() * Integer.MaxValue)
        ReDim texturePixels(7)
    End Sub
    Sub New(ByVal materialName As String)
        name = materialName
        materialUID = Int(Rnd() * Integer.MaxValue)
        ReDim texturePixels(7)
    End Sub
    Public ReadOnly Property UID() As Integer
        Get
            Return materialUID
        End Get
    End Property
    Private Shared Sub MakeHalfTexture(ByRef sourceSurface As PixelSurface, ByRef targetSurface As PixelSurface)
        Dim y, x As Integer
        Dim width, height, newWidth, newHeight As Integer
        Dim r, g, b As Integer
        width = sourceSurface.Width : height = sourceSurface.Height
        newWidth = Math.Ceiling(width / 2.0)
        newHeight = Math.Ceiling(height / 2.0)
        targetSurface = New PixelSurface(newWidth, newHeight)
        Dim widthOffset1, widthOffset2, widthOffset3, xOffset1, xOffset2, xOffset3 As Integer
        For y = 0 To newHeight - 1
            widthOffset1 = y * width * 6
            widthOffset2 = (y + 1) * width * 6
            widthOffset3 = y * newWidth * 3
            For x = 0 To newWidth - 1
                xOffset1 = x * 6
                xOffset2 = x * 6 + 3
                xOffset3 = x * 3
                r = sourceSurface.pixels2(widthOffset1 + xOffset1)
                r += sourceSurface.pixels2(widthOffset2 + xOffset1)
                r += sourceSurface.pixels2(widthOffset1 + xOffset2)
                r += sourceSurface.pixels2(widthOffset2 + xOffset2)
                g = sourceSurface.pixels2(widthOffset1 + xOffset1 + 1)
                g += sourceSurface.pixels2(widthOffset2 + xOffset1 + 1)
                g += sourceSurface.pixels2(widthOffset1 + xOffset2 + 1)
                g += sourceSurface.pixels2(widthOffset2 + xOffset2 + 1)
                b = sourceSurface.pixels2(widthOffset1 + xOffset1 + 2)
                b += sourceSurface.pixels2(widthOffset2 + xOffset1 + 2)
                b += sourceSurface.pixels2(widthOffset1 + xOffset2 + 2)
                b += sourceSurface.pixels2(widthOffset2 + xOffset2 + 2)
                r = (r \ 4)
                g = (g \ 4)
                b = (b \ 4)
                targetSurface.pixels2(widthOffset3 + xOffset3) = r
                targetSurface.pixels2(widthOffset3 + xOffset3 + 1) = g
                targetSurface.pixels2(widthOffset3 + xOffset3 + 2) = b
            Next
        Next
    End Sub
    Public Sub MakeMipMaps(Optional ByVal maximumDownLevel As Integer = 7)
        Dim i As Integer
        For i = 0 To maximumDownLevel - 1
            MakeHalfTexture(texturePixels(i), texturePixels(i + 1))
            maximumMipMapLevel = i
        Next
    End Sub
    Public Function GetCopy() As Material
        Dim material As New Material
        With material
            .name = name
            .maximumMipMapLevel = maximumMipMapLevel
            .textureName = name
            ReDim .texturePixels(texturePixels.Length - 1)
            For i As Integer = 0 To texturePixels.Length - 1
                .texturePixels(i) = texturePixels(i)
            Next
            .textureUsed = textureUsed
        End With
        Return material
    End Function
End Class
