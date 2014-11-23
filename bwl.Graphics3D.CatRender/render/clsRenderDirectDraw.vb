Partial Public Class Render3D
    Public Class DirectDrawClass
        Dim myParent As Render3D
        Dim width, height As Integer
        Dim pixels() As Byte
        Friend Sub New(ByVal parent As Render3D)
            myParent = parent
        End Sub
        ' ;Public Overloads Sub Line(ByVal vertex1 As Vertex, ByVal vertex2 As Vertex)
        Public Sub Init()
            width = myParent.width
            height = myParent.height
            pixels = myParent.drawBuffer.pixels2
        End Sub
        ' End Sub
        ''' <summary>
        ''' –исует линию на плоскость представлени€
        ''' </summary>
        ''' <param name="x1"></param>
        ''' <param name="y1"></param>
        ''' <param name="x2"></param>
        ''' <param name="y2"></param>
        ''' <param name="color">÷вет линии</param>
        ''' <remarks></remarks>
        Public Overloads Sub DrawLine(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal color As Color)
            Dim reversed As Boolean
            If x1 < 2 Then x1 = 2
            If x2 < 2 Then x2 = 2
            If y1 < 2 Then y1 = 2
            If y2 < 2 Then y2 = 2
            If x1 > myParent.width - 3 Then x1 = myParent.width - 3
            If x2 > myParent.width - 3 Then x2 = myParent.width - 3
            If y1 > myParent.height - 3 Then y1 = myParent.height - 3
            If y2 > myParent.height - 3 Then y2 = myParent.height - 3
            Dim width As Integer = myParent.width
            If Math.Abs(y2 - y1) / Math.Abs(x2 - x1) > 1 Then
                Swap(x1, y1)
                Swap(x2, y2)
                reversed = True
            End If
            If x1 > x2 Then
                Swap(x1, x2)
                Swap(y1, y2)
            End If
            Dim diffYX As Single = Math.Abs((y2 - y1) / (x2 - x1))
            Dim currentDiffXY As Single
            Dim x, y As Integer
            'Dim pixels() As Byte = myParent.presentBuffer.pixels2
            Dim r, g, b As Byte
            r = color.R
            g = color.G
            b = color.B
            Dim ystep As Integer
            y = y1
            If y2 > y1 Then
                ystep = +1
            Else
                ystep = -1
            End If
            If reversed Then
                For x = x1 To x2
                    pixels(x * width * 3 + 3 * y + 0) = r
                    pixels(x * width * 3 + 3 * y + 0) = g
                    pixels(x * width * 3 + 3 * y + 0) = b
                    currentDiffXY += diffYX
                    If currentDiffXY > 0.5 Then
                        y += ystep
                        currentDiffXY -= 1
                    End If
                Next
            Else
                For x = x1 To x2
                    pixels(y * width * 3 + 3 * x + 0) = r
                    pixels(y * width * 3 + 3 * x + 0) = g
                    pixels(y * width * 3 + 3 * x + 0) = b
                    currentDiffXY += diffYX
                    If currentDiffXY > 0.5 Then
                        y += ystep
                        currentDiffXY -= 1
                    End If
                Next
            End If
        End Sub
        Private Shared Sub Swap(ByRef var1 As Single, ByRef var2 As Single)
            Dim varTmp As Single
            varTmp = var1
            var1 = var2
            var2 = varTmp
        End Sub
        Public Sub DrawSprite(ByVal sprite As Sprite, ByVal x As Single, ByVal y As Single, ByVal scale As Single, ByVal alpha As Single, Optional ByVal red As Integer = 255, Optional ByVal green As Integer = 255, Optional ByVal blue As Integer = 255)
            With sprite
                Dim tx1, tx2, ty1, ty2 As Integer
                Dim dsx, dsy, sx, sy, sx0 As Single
                tx1 = x
                ty1 = y
                If scale < 0 Then scale = 0
                tx2 = ((.right - .left) * scale) + x
                ty2 = ((.bottom - .top) * scale) + y
                If tx2 > 0 AndAlso ty2 > 0 AndAlso tx1 < width AndAlso ty1 < height And alpha > 0 Then
                    If tx1 < 0 Then tx1 = 0
                    If ty1 < 0 Then ty1 = 0
                    If tx2 > width - 1 Then tx2 = width - 1
                    If ty2 > height - 1 Then ty2 = height - 1
                    sx = .left - (tx1 - x)
                    sx0 = sx
                    sy = .top - (ty1 - y)
                    dsx = scale
                    dsy = scale
                    Dim tx, ty As Integer
                    Dim sxi, syi As Integer
                    Dim widthOffset As Integer
                    Dim textureWidth As Integer
                    Dim r1, g1, b1 As Integer
                    Dim r2, g2, b2 As Integer
                    textureWidth = .texture.Width
                    If alpha = 1 And .alphaSource = AlphaSourceType.external Then
                        'вариант без полупрозрачности с ключевым цветом
                        For ty = ty1 To ty2
                            widthOffset = ty * width * 3
                            For tx = tx1 To tx2
                                sxi = CInt(sx)
                                syi = CInt(sy)
                                r2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 2)
                                g2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 1)
                                b2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 0)
                                If g2 > 0 OrElse r2 < 255 OrElse b2 < 255 Then
                                    r2 = (((r2 * red) >> 8) And 255)
                                    g2 = (((g2 * green) >> 8) And 255)
                                    b2 = (((b2 * blue) >> 8) And 255)
                                    pixels(tx * 3 + widthOffset + 2) = r2
                                    pixels(tx * 3 + widthOffset + 1) = g2
                                    pixels(tx * 3 + widthOffset + 0) = b2
                                End If
                                sx += dsx
                            Next
                            sx = sx0
                            sy += dsy
                        Next
                    Else
                        'вариант с полупрозрачностью
                        Dim baseR, baseB, baseG As Integer
                        Dim alphaResult As Single = 1
                        baseB = 1000 : baseR = 1000 : baseG = 1000
                        If .alphaSource = AlphaSourceType.byWhite Then baseB = 255 : baseR = 255 : baseG = 255
                        If .alphaSource = AlphaSourceType.byPurple Then baseB = 255 : baseR = 255 : baseG = 0
                        If .alphaSource = AlphaSourceType.byBlack Then baseB = 0 : baseR = 0 : baseG = 0
                        For ty = ty1 To ty2
                            widthOffset = ty * width * 3
                            For tx = tx1 To tx2
                                sxi = CInt(sx)
                                syi = CInt(sy)
                                r2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 2)
                                g2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 1)
                                b2 = .texture.pixels2(sxi * 3 + syi * textureWidth * 3 + 0)
                                If g2 > 0 OrElse r2 < 255 OrElse b2 < 255 Then
                                    alphaResult = (Abs(baseR - r2) + Abs(baseG - g2) + Abs(baseB - b2)) * 0.00130208
                                    If alphaResult > 1 Then alphaResult = 1
                                    alphaResult *= alpha
                                    'alpha = 0.5
                                    r1 = pixels(tx * 3 + widthOffset + 2)
                                    g1 = pixels(tx * 3 + widthOffset + 1)
                                    b1 = pixels(tx * 3 + widthOffset + 0)
                                    r2 = (r2 * alpha + r1 * (1 - alpha))
                                    g2 = (g2 * alpha + g1 * (1 - alpha))
                                    b2 = (b2 * alpha + b1 * (1 - alpha))
                                    r2 = (((r2 * red) >> 8) And 255)
                                    g2 = (((g2 * green) >> 8) And 255)
                                    b2 = (((b2 * blue) >> 8) And 255)
                                    pixels(tx * 3 + widthOffset + 2) = r2
                                    pixels(tx * 3 + widthOffset + 1) = g2
                                    pixels(tx * 3 + widthOffset + 0) = b2
                                End If
                                sx += dsx
                            Next
                            sx = sx0
                            sy += dsy
                        Next
                    End If
                End If
            End With
        End Sub
        Public Overloads Sub DrawString(ByVal textMessage As TextMessage)
            With textMessage
                DrawString(.text, .font, .currentX, .currentY, .alpha, .red, .green, .blue)
            End With
        End Sub
        Public Overloads Sub DrawString(ByVal text As String, ByVal font As BitmapFont, ByVal x As Integer, ByVal y As Integer, ByVal alpha As Single, ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer)
            Dim width, i As Integer
            If font.Loaded Then
                width = font.Width
                'pixels = myParent.drawBuffer.pixels2
                For i = 0 To text.Length - 1
                    DrawSprite(font.chars(Asc(text(i))), x + width * i, y, 1, alpha, red, green, blue)
                Next
            End If
        End Sub
        Private Function Abs(ByVal value As Integer) As Integer
            If value > 0 Then Return value Else Return -value
        End Function
    End Class
End Class
