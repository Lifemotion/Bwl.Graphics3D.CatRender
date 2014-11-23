Public Class BitmapFont
    Public chars() As Sprite
    Public name As String
    Private inited As Boolean
    Private texture As PixelSurface
    Private charWidth, charHeight As Integer
    Public Sub LoadBitmap(ByVal filename As String)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

        texture = New PixelSurface
        Try
            texture.LoadFromFile(filename)
            inited = True
        Catch ex As Exception
            inited = False
        End Try
        If inited Then
            charWidth = texture.Width \ 16
            charHeight = texture.Height \ 16
            ReDim chars(255)
            Dim x, y As Integer
            For x = 0 To 15
                For y = 0 To 15
                    chars(y * 16 + x) = New Sprite
                    With chars(y * 16 + x)
                        .left = x * charWidth
                        .right = x * charWidth + charWidth - 1
                        .top = y * charHeight + 4
                        .bottom = y * charHeight + charHeight - 1 + 4
                        .texture = texture
                        '.alphaSource = AlphaSourceType.byPurple
                        .name = "Char " + Chr(y * 16 + x)
                        .scale = 1
                    End With
                Next
            Next
        End If
    End Sub
    Public ReadOnly Property Loaded() As Boolean
        Get
            Return inited
        End Get
    End Property
    Public ReadOnly Property Width() As Integer
        Get
            Return charWidth
        End Get
    End Property
End Class
