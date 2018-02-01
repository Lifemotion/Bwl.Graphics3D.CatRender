Imports bwl.Graphics._3D.CatRender

Public Class ModelGen

    Public Shared Function CreateBoxModel(xSize As Single, ySize As Single, zSize As Single, material As Material) As Model
        Dim Model As New Model
        With Model
            ReDim .meshes(0)
            ReDim .materialsLib(0)
            .materialsLib(0) = material
        End With

        With Model.meshes(0)
            InitializeTriangleArrays(.Triangles, 12)
            InitializeVertexArrays(.Vertexes, 8)

            .Vertexes.pX(0) = -xSize / 2
            .Vertexes.pY(0) = ySize / 2
            .Vertexes.pZ(0) = -zSize / 2
            .Vertexes.tU(0) = 0
            .Vertexes.tV(0) = 0

            .Vertexes.pX(1) = xSize / 2
            .Vertexes.pY(1) = ySize / 2
            .Vertexes.pZ(1) = -zSize / 2
            .Vertexes.tU(1) = 1
            .Vertexes.tV(1) = 0

            .Vertexes.pX(2) = xSize / 2
            .Vertexes.pY(2) = -ySize / 2
            .Vertexes.pZ(2) = -zSize / 2
            .Vertexes.tU(2) = 1
            .Vertexes.tV(2) = 1

            .Vertexes.pX(3) = -xSize / 2
            .Vertexes.pY(3) = -ySize / 2
            .Vertexes.pZ(3) = -zSize / 2
            .Vertexes.tU(3) = 0
            .Vertexes.tV(3) = 1

            .Vertexes.pX(4) = -xSize / 2
            .Vertexes.pY(4) = ySize / 2
            .Vertexes.pZ(4) = zSize / 2
            .Vertexes.tU(4) = 0
            .Vertexes.tV(4) = 0

            .Vertexes.pX(5) = xSize / 2
            .Vertexes.pY(5) = ySize / 2
            .Vertexes.pZ(5) = zSize / 2
            .Vertexes.tU(5) = 1
            .Vertexes.tV(5) = 0

            .Vertexes.pX(6) = xSize / 2
            .Vertexes.pY(6) = -ySize / 2
            .Vertexes.pZ(6) = zSize / 2
            .Vertexes.tU(6) = 1
            .Vertexes.tV(6) = 1

            .Vertexes.pX(7) = -xSize / 2
            .Vertexes.pY(7) = -ySize / 2
            .Vertexes.pZ(7) = zSize / 2
            .Vertexes.tU(7) = 0
            .Vertexes.tV(7) = 1

            .Vertexes.count = 8

            'front
            .Triangles.v1(0) = 0
            .Triangles.v2(0) = 2
            .Triangles.v3(0) = 1

            .Triangles.v1(1) = 0
            .Triangles.v2(1) = 3
            .Triangles.v3(1) = 2

            'back
            .Triangles.v1(2) = 4
            .Triangles.v2(2) = 5
            .Triangles.v3(2) = 6

            .Triangles.v1(3) = 4
            .Triangles.v2(3) = 6
            .Triangles.v3(3) = 7

            'top
            .Triangles.v1(4) = 4
            .Triangles.v2(4) = 0
            .Triangles.v3(4) = 1

            .Triangles.v1(5) = 5
            .Triangles.v2(5) = 4
            .Triangles.v3(5) = 1

            'left
            .Triangles.v1(6) = 0
            .Triangles.v2(6) = 4
            .Triangles.v3(6) = 7

            .Triangles.v1(7) = 3
            .Triangles.v2(7) = 0
            .Triangles.v3(7) = 7

            'right
            .Triangles.v1(8) = 5
            .Triangles.v2(8) = 1
            .Triangles.v3(8) = 6

            .Triangles.v1(9) = 6
            .Triangles.v2(9) = 1
            .Triangles.v3(9) = 2

            'bottom
            .Triangles.v1(10) = 3
            .Triangles.v2(10) = 7
            .Triangles.v3(10) = 2

            .Triangles.v1(11) = 7
            .Triangles.v2(11) = 6
            .Triangles.v3(11) = 2

            .Triangles.count = 12
        End With

        Model.ComputeNormals()
        Model.Prepare()
        Return Model
    End Function

    Public Shared Function CreatePlateModel(width As Single, height As Single, material As Material) As Model
        Dim Model As New Model
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
