Imports bwl.Graphics._3D.CatRender

Public Class PlateGen

    Public Shared Function CreatePlateModel(width As Single, height As Single, texture As PixelSurface) As Model
        Dim Model As New Model

        Dim material As New Material
        material.texturePixels(0) = texture
        material.maximumMipMapLevel = 0

        With Model
            ReDim .meshes(0)
            ReDim .materialsLib(0)
            .materialsLib(0) = material
        End With

        With Model.meshes(0)
            InitializeTriangleArrays(.Triangles, 2)
            InitializeVertexArrays(.Vertexes, 4)

            .Vertexes.pX(0) = -width / 2
            .Vertexes.pY(0) = height / 2
            .Vertexes.pZ(0) = 0
            .Vertexes.tU(0) = 0
            .Vertexes.tV(0) = 0

            .Vertexes.pX(1) = width / 2
            .Vertexes.pY(1) = height / 2
            .Vertexes.pZ(1) = 0
            .Vertexes.tU(1) = 1
            .Vertexes.tV(1) = 0

            .Vertexes.pX(2) = width / 2
            .Vertexes.pY(2) = -height / 2
            .Vertexes.pZ(2) = 0
            .Vertexes.tU(2) = 1
            .Vertexes.tV(2) = 1

            .Vertexes.pX(3) = -width / 2
            .Vertexes.pY(3) = -height / 2
            .Vertexes.pZ(3) = 0
            .Vertexes.tU(3) = 0
            .Vertexes.tV(3) = 1

            .Vertexes.count = 4

            .Triangles.v1(0) = 0
            .Triangles.v2(0) = 2
            .Triangles.v3(0) = 1
            .Triangles.materialID(0) = 0
            .Triangles.v1(1) = 0
            .Triangles.v2(1) = 3
            .Triangles.v3(1) = 2
            .Triangles.materialID(1) = 0

            .Triangles.count = 2
        End With

        Model.ComputeNormals()
        Model.Prepare()
        Return Model
    End Function
End Class
