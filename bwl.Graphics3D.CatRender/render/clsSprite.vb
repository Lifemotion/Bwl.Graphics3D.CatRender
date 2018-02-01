Public Enum AlphaSourceType As Integer
    external = 0
    byWhite = 1
    byBlack = 2
    byPurple = 3
End Enum
Public Class Sprite
    Public texture As PixelSurface
    Public left, right, top, bottom As Integer
    Public scale As Single
    Public minimumScale As Single
    Public maximumScale As Single
    Public name As String
    Public alphaSource As AlphaSourceType
    Sub New()
        maximumScale = 1000
    End Sub

    Public Shared Function CreateSprite(texturePath As String, Optional scale As Single = 1.0) As Sprite
        Dim sprite As New Sprite
        With sprite
            .texture = New PixelSurface
            .texture.LoadFromFile(texturePath)
            .scale = scale
            '.minimumScale = .scale
            '.maximumScale = .scale
            .alphaSource = AlphaSourceType.byPurple
            .left = 0
            .top = 0
            .right = .texture.Width - 1
            .bottom = .texture.Height - 1
        End With
        Return sprite
    End Function

    Public Shared Function CreateSprite(texture As PixelSurface, Optional scale As Single = 1.0) As Sprite
        Dim sprite As New Sprite
        With sprite
            .texture = texture
            .scale = scale
            '.minimumScale = .scale
            '.maximumScale = .scale
            .alphaSource = AlphaSourceType.byPurple
            .left = 0
            .top = 0
            .right = .texture.Width - 1
            .bottom = .texture.Height - 1
        End With
        Return sprite
    End Function

End Class
