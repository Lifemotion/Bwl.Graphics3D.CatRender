Imports bwl.Graphics._3D.CatRender

Public Class TestForm
    Private Sub TestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim model As New Model
        ReDim model.meshes(0)
        Dim material As New Material("Test")
        With material
            .textureUsed = False
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

        CatRender1.Render.SceneDraw.settings.drawLines = True
        CatRender1.Render.SceneDraw.settings.drawTriangles = False

        CatRender1.RenderScene.AddObject(obj)

        CatRender1.RenderDrawingEnabled = True
        CatRender1.RenderWorkingEnabled = True
        CatRender1.RenderAutoMove = True
        CatRender1.RenderMouseLocking = True

    End Sub
End Class
