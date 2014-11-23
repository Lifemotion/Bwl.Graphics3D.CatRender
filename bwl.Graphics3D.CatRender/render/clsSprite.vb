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

    
End Class
