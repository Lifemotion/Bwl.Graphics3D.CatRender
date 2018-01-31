Imports bwl.Graphics._3D.CatRender


Public Class TestForm

    Public Function CreateTriangleObject2()
        Dim obj = CreateTriangleObject()
        obj.rotateY = 100
        Return obj
    End Function

    Public Function CreateTriangleObject() As Object3D
        Dim model As New Model
        ReDim model.meshes(0)
        Dim material As New Material("Test")
        With material
            .textureUsed = False
            .color = Color.Red
        End With
        With model.meshes(0)
            model.materialsLib = {material}
            With .Vertexes
                ReDim .pX(2)
                ReDim .pY(2)
                ReDim .pZ(2)
                ReDim .nX(2)
                ReDim .nY(2)
                ReDim .nZ(2)
                ReDim .rX(2)
                ReDim .rY(2)
                ReDim .rZ(2)
                ReDim .tU(2)
                ReDim .tV(2)
                ReDim .temp(2)
                ReDim .Color(2)
                ReDim .material(2)
                ReDim .materialUID(2)
                .pX(1) = 10 : .pY(1) = 10 : .pZ(1) = 10
                .pX(2) = -10 : .pY(2) = -10 : .pZ(2) = 10
                .count = 2
            End With
            With .Triangles
                ReDim .v1(0)
                ReDim .v2(0)
                ReDim .v3(0)
                ReDim .materialID(0)
                ReDim .nX(0)
                ReDim .nY(0)
                ReDim .nZ(0)
                ReDim .textureSize(0)
                ReDim .renderSettings(0)
                .v1(0) = 0
                .v2(0) = 1
                .v3(0) = 2
                .count = 1
            End With
        End With
        model.Prepare()

        Dim obj As New Object3D
        obj.model = model
        obj.type = Object3DType.model

        Return obj
    End Function

    Public Function CreateSpriteObject()
        Dim sprite As New Sprite

        With sprite
            .texture = New PixelSurface
            .texture.LoadFromFile("..\..\point.bmp")
            .scale = 1.0
            ' .minimumScale = .scale
            ' .maximumScale = .scale
            .alphaSource = AlphaSourceType.byPurple
            .left = 0
            .top = 0
            .right = .texture.Width - 1
            .bottom = .texture.Height - 1
        End With

        Dim obj As New Object3D
        With obj
            .type = Object3DType.sprite
            .sprite = sprite
            'light не обязателен. Если он есть, будет использоваться для перекраски
            .light = New Lighter
            .light.colorR = 255
            .light.colorG = 255
            .light.colorB = 0
        End With
        Return obj

    End Function
    Public Function CreateSpriteObject2()
        Dim sprite As New Sprite

        With sprite
            .texture = New PixelSurface
            .texture.LoadFromFile("..\..\point.bmp")
            .scale = 1.0
            '   .minimumScale = .scale
            '   .maximumScale = .scale
            .alphaSource = AlphaSourceType.byPurple
            .left = 0
            .top = 0
            .right = .texture.Width - 1
            .bottom = .texture.Height - 1
        End With

        Dim obj As New Object3D
        With obj
            .type = Object3DType.sprite
            .sprite = sprite
            .positionX = -100
            .positionY = -100
            'light не обязателен. Если он есть, будет использоваться для перекраски
            .light = New Lighter
            .light.colorR = 255
            .light.colorG = 0
            .light.colorB = 0
        End With
        Return obj

    End Function

    Public Function CreateLighterObject()
        Dim obj As New Object3D
        With obj
            .type = Object3DType.light
            .positionX = 100
            .positionY = 100
            .light = New Lighter
            .light.type = LighterTypeEnum.ambient
            .light.colorR = 100
            .light.colorG = 100
            .light.colorB = 100
        End With
        Return obj
    End Function
    Public Function CreateLighterObject2()
        Dim obj As New Object3D
        With obj
            .type = Object3DType.light
            .positionX = 0
            .positionY = -100
            .light = New Lighter
            .light.intense = 1000
            .light.attenutionA = 0.0
            .light.attenutionB = 0.0
            .light.type = LighterTypeEnum.point
            .light.colorR = 255
            .light.colorG = 255
            .light.colorB = 255
        End With
        Return obj
    End Function

    Private Sub TestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AddHandler CatRender1.Display.DisplayPicturebox.MouseDown, AddressOf TestHandler
        '  CatRender1.Render.SceneDraw.
        CatRender1.Render.SceneDraw.defaults.lighting = LightingMode.full
        CatRender1.Render.SceneDraw.settings.drawLines = True
        CatRender1.Render.SceneDraw.settings.drawTriangles = True
        CatRender1.RenderScene.AddObject(CreateLighterObject)
        CatRender1.RenderScene.AddObject(CreateLighterObject2)
        CatRender1.RenderScene.AddObject(CreateTriangleObject)
        CatRender1.RenderScene.AddObject(CreateTriangleObject2)
        CatRender1.RenderScene.AddObject(CreateSpriteObject)
        CatRender1.RenderScene.AddObject(CreateSpriteObject2)
        CatRender1.RenderDrawingEnabled = True
        CatRender1.RenderWorkingEnabled = True
        CatRender1.RenderAutoMove = True
        CatRender1.RenderMouseLocking = False


    End Sub

    Private Sub CatRender1_RenderWorkCycle() Handles CatRender1.RenderWorkCycle
        Dim sprites = CatRender1.RenderSceneDraw.DebugSpritesList
        Debug.WriteLine(sprites.Count)
        If sprites.Count > 0 Then
            Debug.WriteLine("uid, " + sprites(0).UID.ToString + " px, " + sprites(0).px.ToString)
        End If
    End Sub

    Private Sub TestHandler(sender As Object, e As MouseEventArgs)
        'если была правая кнопка - блокируем
        If e.Button = Windows.Forms.MouseButtons.Right Then
            CatRender1.Display.MouseLocked = True
        Else
            MsgBox(e.X.ToString + ", " + e.Y.ToString)
        End If
    End Sub

End Class
