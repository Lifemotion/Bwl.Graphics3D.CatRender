Imports bwl.Graphics._3D.CatRender

Public Class TestForm

    Private Sub TestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AddHandler CatRender1.Display.DisplayPicturebox.MouseDown, AddressOf TestHandler
        CatRender1.Render.SceneDraw.defaults.lighting = LightingMode.full
        CatRender1.Render.SceneDraw.settings.drawLines = True
        CatRender1.Render.SceneDraw.settings.drawTriangles = True
        CatRender1.RenderScene.AddObject(Object3D.CreateAmbientLightObject(Color.DarkGray))
        CatRender1.RenderScene.AddObject(Object3D.CreatePointLightObject(Color.White, 100, 50, 100, -50))
        CatRender1.RenderScene.AddObject(Object3D.CreateModelObject(
                                         ModelGen.CreateBoxModel(10, 5, 20,
                                         Material.CreateMaterial(Color.Red)),
                                         0, 0, 0, 0, 0, 0))
        CatRender1.RenderScene.AddObject(Object3D.CreateSpriteObject(
                                         Sprite.CreateSprite("..\..\point.bmp"),
                                         0, 0, 0))
        CatRender1.RenderScene.AddObject(Object3D.CreateSpriteObject(
                                         Sprite.CreateSprite("..\..\point.bmp"),
                                         10, 10, -10, Color.Red))
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
