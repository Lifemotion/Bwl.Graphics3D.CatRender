''' <summary>
''' Представляет поверхность из пикселов
''' как трехмерный массив. Для хранения текстур,
''' результатов рисования и прочего.
''' </summary>
''' <remarks></remarks>
Public Class PixelSurface
    'Public pixels(,,) As Byte
    Public pixels2() As Byte
    Public name As String
    Public loadedFromFile As String
    Private tmpBitmap As Bitmap
    Private bmpWidth, bmpHeight As Integer
    ''' <summary>
    ''' загружает плоскость из объекта класса Bitmap
    ''' </summary>
    ''' <param name="srcBitmap">Откуда загружать</param>
    ''' <remarks></remarks>
    Public Sub LoadFromBitmap(ByRef srcBitmap As Bitmap)
        Dim tmpBD As Imaging.BitmapData
        Dim tmpRect As Rectangle
        tmpRect = Rectangle.FromLTRB(0, 0, srcBitmap.Width, srcBitmap.Height)
        tmpBD = srcBitmap.LockBits(tmpRect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format24bppRgb)
        bmpWidth = srcBitmap.Width
        bmpHeight = srcBitmap.Height
        Dim i As Integer
        Dim size As Integer = srcBitmap.Width * srcBitmap.Height
        Dim tmpBytes As Byte()
        ReDim tmpBytes(size * 3)
        'ReDim pixels(bmpHeight - 1, bmpWidth - 1, 2)
        ReDim pixels2((bmpWidth + 3) * (bmpHeight + 3) * 3)
        Runtime.InteropServices.Marshal.Copy(tmpBD.Scan0, tmpBytes, 0, size * 3)
        Dim x, y As Integer
        For i = 0 To size - 1
            ' pixels(y, x, 2) = tmpBytes(i * 3 + 0)
            ' pixels(y, x, 1) = tmpBytes(i * 3 + 1)
            '  pixels(y, x, 0) = tmpBytes(i * 3 + 2)
            pixels2(y * bmpWidth * 3 + 3 * x + 0) = tmpBytes(i * 3 + 0)
            pixels2(y * bmpWidth * 3 + 3 * x + 1) = tmpBytes(i * 3 + 1)
            pixels2(y * bmpWidth * 3 + 3 * x + 2) = tmpBytes(i * 3 + 2)
            x += 1
            If x = bmpWidth Then
                y += 1
                x = 0
            End If
        Next
        loadedFromFile = ""
        srcBitmap.UnlockBits(tmpBD)
    End Sub
    ''' <summary>
    ''' загружает плоскость из графического файла
    ''' </summary>
    ''' <param name="filename">Имя файла с изображением</param>
    ''' <remarks></remarks>
    Public Sub LoadFromFile(ByVal filename As String)
        Try
            filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

            Dim tmpBitmap As Bitmap
            tmpBitmap = Bitmap.FromFile(filename)
            LoadFromBitmap(tmpBitmap)
            loadedFromFile = filename
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    ''' <summary>
    ''' Генерирует битмап по внутренним массивам точек. 
    ''' </summary>
    ''' <param name="destBitmap">Куда писать результат.</param>
    ''' <remarks></remarks>
    Public Sub GetBitmap(ByRef destBitmap As Bitmap)
        Dim tmpBD As Imaging.BitmapData
        Dim tmpRect As Rectangle
        tmpRect = Rectangle.FromLTRB(0, 0, bmpWidth, bmpHeight)
        tmpBD = destBitmap.LockBits(tmpRect, Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format24bppRgb)
        Dim size As Integer = bmpWidth * bmpHeight
        System.Runtime.InteropServices.Marshal.Copy(pixels2, 0, tmpBD.Scan0, size * 3)
        destBitmap.UnlockBits(tmpBD)
    End Sub
    ''' <summary>
    ''' Отрисовывает картинку
    ''' </summary>
    ''' <param name="pictBox"></param>
    ''' <remarks></remarks>
    Public Sub DrawToPictureBox(ByRef pictBox As PictureBox)
        If bmpWidth > 0 And bmpHeight > 0 Then
            'Dim graphics As Graphics = pictBox.CreateGraphics
            GetBitmap(tmpBitmap)
            ' graphics.DrawImage(tmpBitmap, 0, 0)
            pictBox.Image = tmpBitmap
        End If
    End Sub

    Public Sub New(ByVal width As Integer, ByVal height As Integer)
        SetSizeNoSave(width, height)
        'bmpWidth = width
        'bmpHeight = height
        'ReDim pixels2((bmpWidth + 3) * (bmpHeight + 3) * 3)
    End Sub
    Public Sub New()
        ReDim pixels2(10)
    End Sub
    Public Sub Clear()
        Array.Clear(pixels2, 0, pixels2.GetUpperBound(0))
    End Sub
    Public Sub SetSizeNoSave(ByVal newWidth As Integer, ByVal newHeight As Integer)
        bmpHeight = newHeight
        bmpWidth = newWidth
        ' ReDim pixels(bmpHeight - 1, bmpWidth - 1, 2)
        ReDim pixels2((bmpWidth + 3) * (bmpHeight + 3) * 3)
        tmpBitmap = New Bitmap(bmpWidth, bmpHeight, Imaging.PixelFormat.Format24bppRgb)
    End Sub
    ' Private Sub CreateTmpB
    Public Sub CopyFromPixelSurface(ByVal source As PixelSurface)
        Dim newWidth, newHeight As Integer
        newWidth = source.Width
        newHeight = source.Height
        If newWidth <> bmpWidth Or newHeight <> bmpHeight Then
            SetSizeNoSave(newWidth, newHeight)
        End If
        Array.Copy(source.pixels2, pixels2, source.pixels2.GetUpperBound(0))
    End Sub
    Public Sub GetSize(ByRef width As Integer, ByRef height As Integer)
        width = bmpWidth
        height = bmpHeight
    End Sub
    Public ReadOnly Property Width() As Integer
        Get
            Return bmpWidth
        End Get
    End Property
    Public ReadOnly Property Height() As Integer
        Get
            Return bmpHeight
        End Get
    End Property
End Class
