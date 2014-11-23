Public Structure PresentParametersStruct
    Dim WindowWidth As Integer
    Dim WindowHeight As Integer
    Dim Antialiasing As Integer
    Dim TargetType As OutputTargetTypeEnum
    Dim TargetPictureBox As PictureBox
    Dim TargetMaterial As Material
    Dim TargetGraphics As Drawing.Graphics
End Structure
Public Enum OutputTargetTypeEnum
    none = 0
    pictureBox = 1
    material = 2
    graphics = 3
    lastBitmapOnly = 4
End Enum
Partial Public Class Render3D
    ''' <summary>
    ''' ��������� ������������� � ��������� �����������
    ''' </summary>
    ''' <remarks></remarks>
    Public presentSettings As PresentParametersStruct
    Public DirectDraw As DirectDrawClass
    Public SceneDraw As SceneDrawClass
    Public lastPresentedBitmap As Bitmap
    Private currentPresenting As PresentParametersStruct
    Private presentBuffer As PixelSurface
    Public drawBuffer As PixelSurface
    Public width, height As Integer
    Private diagonal As Integer
    Private tmpBitmap As Bitmap
    Private graphics As Drawing.Graphics
    Private finalSurface As PixelSurface
    ''' <summary>
    ''' �������������� ���������� ��������� �����������.
    ''' �������������� ��������� ����������.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Init()
        '����������� ���������� ��������� �� ���������� �����
        currentPresenting = presentSettings
        With currentPresenting
            If .Antialiasing < 1 And .Antialiasing > 4 Then .Antialiasing = 1
            width = .WindowWidth * .Antialiasing
            height = .WindowHeight * .Antialiasing
            '������� ������� ����������� ��� ���������
            drawBuffer = New PixelSurface(width, height)
            presentBuffer = drawBuffer
            If .Antialiasing > 1 Then
                finalSurface = New PixelSurface(.WindowWidth, .WindowHeight)
            Else
                finalSurface = drawBuffer
            End If
            tmpBitmap = New Bitmap(.WindowWidth, .WindowHeight, Imaging.PixelFormat.Format24bppRgb)
        End With
        '����������� ���������
        diagonal = Math.Sqrt(width ^ 2 + height ^ 2)
        SceneDraw.Init()
        DirectDraw.Init()
        '�������������� ������� � �����������
    End Sub
    ''' <summary>
    ''' ������� ������� ����������� ������������� � ������ ������
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        'Array.
        ' drawBuffer.SetSizeNoSave(width, height)
        Array.Clear(drawBuffer.pixels2, 0, drawBuffer.pixels2.GetUpperBound(0))
        SceneDraw.Clear()
    End Sub

    Private Sub AntiAlias()

    End Sub

    ''' <summary>
    ''' ������������ ����� ��������� � ��������� �������
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Present()
        '���� �����, ����������
        Dim width1 As Integer = presentBuffer.Width
        Dim width2 As Integer = finalSurface.Width
        Dim aa As Integer = currentPresenting.Antialiasing
        Dim aa2 As Single = 1 / (aa * aa)
        If currentPresenting.Antialiasing > 1 Then
            For x = 0 To finalSurface.Width - 1
                For y = 0 To finalSurface.Height - 1
                    Dim tmp1 As Integer = 0
                    Dim tmp2 As Integer = 0
                    Dim tmp3 As Integer = 0
                    For x1 = 0 To aa - 1
                        For y1 = 0 To aa - 1
                            tmp1 += presentBuffer.pixels2(((y * aa + y1) * width1 + x * aa + x1) * 3 + 0)
                            tmp2 += presentBuffer.pixels2(((y * aa + y1) * width1 + x * aa + x1) * 3 + 1)
                            tmp3 += presentBuffer.pixels2(((y * aa + y1) * width1 + x * aa + x1) * 3 + 2)
                        Next
                    Next
                    tmp1 = tmp1 * aa2
                    tmp2 = tmp2 * aa2
                    tmp3 = tmp3 * aa2
                    finalSurface.pixels2((y * width2 + x) * 3 + 0) = tmp1
                    finalSurface.pixels2((y * width2 + x) * 3 + 1) = tmp2
                    finalSurface.pixels2((y * width2 + x) * 3 + 2) = tmp3
                Next
            Next
        End If
        '������������ ����������� ��� ���������
        If currentPresenting.TargetType = OutputTargetTypeEnum.graphics Then
            finalSurface.GetBitmap(tmpBitmap)
            lastPresentedBitmap = tmpBitmap
            currentPresenting.TargetGraphics.DrawImage(tmpBitmap, 0, 0)
        Else
            If currentPresenting.TargetType = OutputTargetTypeEnum.pictureBox Then
                finalSurface.DrawToPictureBox(currentPresenting.TargetPictureBox)
            End If
            If currentPresenting.TargetType = OutputTargetTypeEnum.material Then
                currentPresenting.TargetMaterial.texturePixels(0).CopyFromPixelSurface(presentBuffer)
                currentPresenting.TargetMaterial.textureUsed = True
                currentPresenting.TargetMaterial.maximumMipMapLevel = 0
            End If
            If currentPresenting.TargetType = OutputTargetTypeEnum.lastBitmapOnly Then
                finalSurface.GetBitmap(tmpBitmap)
                lastPresentedBitmap = tmpBitmap
            End If
        End If
        '������ ������ �������, ���� ������������ ������ ���������,
        ' ��� ��� ����� ������ �� ������
        'Dim tmpBufferLink As PixelSurface
        'tmpBufferLink = presentBuffer
        'presentBuffer = drawBuffer
        'drawBuffer = tmpBufferLink
    End Sub
    Public Sub New()
        DefaultSettings()
        DirectDraw = New DirectDrawClass(Me)
        SceneDraw = New SceneDrawClass(Me)
    End Sub
    Public Sub ReNew(ByVal ddClass As DirectDrawClass, ByVal scClass As SceneDrawClass)
        DefaultSettings()
        DirectDraw = ddClass
        SceneDraw = scClass
        If DirectDraw Is Nothing Then DirectDraw = New DirectDrawClass(Me)
        If SceneDraw Is Nothing Then SceneDraw = New SceneDrawClass(Me)
    End Sub
    ''' <summary> 
    ''' �������������� ���������� �������� ������ ��-���������.
    '''</summary>
    ''' <remarks></remarks>
    Private Sub DefaultSettings()
        With presentSettings
            .Antialiasing = 1
            .TargetType = OutputTargetTypeEnum.lastBitmapOnly
            .WindowHeight = 240
            .WindowWidth = 320
        End With
    End Sub
End Class
