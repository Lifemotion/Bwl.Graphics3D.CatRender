Public Class WaterGen
    Private Structure waterPoint
        Dim x, y, z, y0 As Single
        Dim dy, ddy As Single
        Dim tu, tv As Single
    End Structure
    Public gridSizeX, gridSizeZ As Integer
    Public gridStepX, gridStepZ As Integer
    Public framesCount As Integer
    Public wavesAmplitude, wavesKoeffX, wavesKoeffZ As Single
    Public disturbStregth, disturbProbability, disturbAtt1, disturbAtt2 As Single
    Public disturbRange As Integer, disturbRangeAtt As Single
    Public disturbMaxFrame As Integer
    Public smallDisturb As Single
    Public waterNoise As Single
    Public phaseStep As Single
    Public textureMoveX As Single
    Public textureMovez As Single
    Public textureTileX As Single
    Public textureTileZ As Single
    Public material As Material
    Public yMultiplier As Single
    Private phase As Single
    Private frame As Integer
    Private grid(,) As waterPoint

    Private Sub Defaults()
        gridSizeX = 10
        gridSizeZ = 10
        gridStepX = 20
        gridStepZ = 20
        wavesAmplitude = 1
        wavesKoeffX = 0.1
        wavesKoeffZ = 0.2
        disturbStregth = 5
        disturbProbability = 0.3
        disturbAtt1 = 0.7
        disturbAtt2 = -0.3
        disturbRange = 0
        disturbRangeAtt = 0.3
        disturbMaxFrame = 80
        framesCount = 100
        smallDisturb = 1
        waterNoise = 0.1
        phaseStep = 0.2512
        textureMoveX = 0.04
        textureMovez = 0.04
        yMultiplier = 1
        textureTileX = 1
        textureTileZ = 1
    End Sub
    Private Sub InitGrid()
        Dim z, x As Integer
        ReDim grid(gridSizeZ, gridSizeX)
        For z = 0 To gridSizeZ
            For x = 0 To gridSizeX
                grid(z, x).x = (x - (gridSizeX >> 1)) * gridStepX
                grid(z, x).z = (z - (gridSizeZ >> 1)) * gridStepZ
                grid(z, x).y0 = 0
                grid(z, x).dy = 0
                grid(z, x).ddy = Rnd() * smallDisturb
                grid(z, x).tu = (x / gridSizeX) * textureTileX
                grid(z, x).tv = (z / gridSizeZ) * textureTileZ
            Next
        Next
    End Sub
    Private Sub MakeDisturb(ByVal z As Integer, ByVal x As Integer)
        Dim lx, lz As Integer
        Dim stregth As Single
        For lz = z - disturbRange To z + disturbRange
            For lx = x - disturbRange To x + disturbRange
                If lz >= 0 AndAlso lx >= 0 AndAlso lz <= gridSizeZ AndAlso lx <= gridSizeX Then
                    stregth = disturbStregth - (Math.Abs(lz - z) + Math.Abs(lx - x)) * disturbStregth * disturbRangeAtt
                    If stregth < 0 Then stregth = 0
                    grid(lz, lx).ddy = grid(lz, lx).ddy - stregth
                End If
            Next
        Next
    End Sub
    Private Sub MoveNodes()
        Dim z, x As Integer
        For z = 0 To gridSizeZ
            For x = 0 To gridSizeX
                With grid(z, x)
                    .dy = .dy + .ddy
                    .dy = .dy * disturbAtt1
                    .ddy = .ddy + disturbAtt2 * .dy
                    .y = .y0 + .dy + wavesAmplitude * Math.Sin(wavesKoeffX * .x + phase + wavesKoeffZ * .z) + waterNoise * (Rnd() - 0.5)
                    .tu = (x / gridSizeX) * textureTileX + phase * textureMoveX
                    .tv = (z / gridSizeZ) * textureTileZ + phase * textureMovez
                End With
            Next
        Next
    End Sub
    Private Sub InitModel(ByRef model As Model)
        If framesCount > 0 Then
            With model
                ReDim .meshes(framesCount - 1)
                ReDim .materialsLib(0)
                .materialsLib(0) = material
            End With
        End If
    End Sub
    Private Sub WriteMeshInModel(ByVal meshIndex As Integer, ByRef model As Model)
        With model.meshes(meshIndex)
            .Name = "water_phase_" + meshIndex.ToString
            InitializeTriangleArrays(.Triangles, (gridSizeX) * (gridSizeZ) * 2)
            InitializeVertexArrays(.Vertexes, (gridSizeX + 1) * (gridSizeZ + 1))
            Dim x, z As Integer
            Dim triangle As Integer
            Dim vertex As Integer
            Dim vertex1, vertex2, vertex3, vertex4 As Integer
            'переносим сетку
            For z = 0 To gridSizeZ
                For x = 0 To gridSizeX
                    vertex = z * (gridSizeX + 1) + x
                    .Vertexes.pX(vertex) = grid(z, x).x
                    .Vertexes.pY(vertex) = grid(z, x).y * yMultiplier
                    .Vertexes.pZ(vertex) = grid(z, x).z
                    .Vertexes.tU(vertex) = grid(z, x).tu
                    .Vertexes.tV(vertex) = grid(z, x).tv
                Next
            Next
            'формируем полигоны
            For z = 0 To gridSizeZ - 1
                For x = 0 To gridSizeX - 1
                    vertex1 = z * (gridSizeX + 1) + x
                    vertex2 = z * (gridSizeX + 1) + x + 1
                    vertex3 = (z + 1) * (gridSizeX + 1) + x
                    vertex4 = (z + 1) * (gridSizeX + 1) + x + 1
                    .Triangles.v1(triangle) = vertex1
                    .Triangles.v2(triangle) = vertex2
                    .Triangles.v3(triangle) = vertex3
                    .Triangles.materialID(triangle) = 0
                    .Triangles.v1(triangle + 1) = vertex3
                    .Triangles.v2(triangle + 1) = vertex2
                    .Triangles.v3(triangle + 1) = vertex4
                    .Triangles.materialID(triangle + 1) = 0
                    triangle += 2
                Next
            Next
        End With
    End Sub
    Private Sub TimeStep()
        Dim x, z As Integer
        phase += phaseStep
        For x = 1 To (gridSizeX - 2)
            For z = 1 To (gridSizeZ - 2)
                If Rnd() <= disturbProbability And disturbMaxFrame >= frame Then
                    MakeDisturb(z, x)
                End If
            Next
        Next
    End Sub
    Public Function MakeModel() As Model
        If material Is Nothing Then
            MsgBox("Не указан материал в генераторе воды!")
            Return Nothing
        Else
            Dim newModel As New Model
            Dim i As Integer
            InitGrid()
            InitModel(newModel)
            phase = 0
            frame = 0
            For i = 0 To framesCount - 1
                MoveNodes()
                WriteMeshInModel(i, newModel)
                TimeStep()
                frame += 1
            Next
            newModel.ComputeNormals()
            newModel.Prepare()
            newModel.name = "watergen"
            Return newModel
        End If
    End Function

    Public Sub New()
        Defaults()
    End Sub
End Class
