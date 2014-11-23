Public Module mdlStructureFunctions
    Sub InitializeVertexArrays(ByRef vertexArray As Vertexes, ByVal size As Integer)
        With vertexArray
            ReDim .nX(size)
            ReDim .nY(size)
            ReDim .nZ(size)
            ReDim .pX(size)
            ReDim .pY(size)
            ReDim .pZ(size)
            ReDim .tU(size)
            ReDim .tV(size)
            ReDim .Color(size)
            ReDim .temp(size)
            ReDim .material(size)
            ReDim .materialUID(size)
            ReDim .size(size)
            ReDim .triangleNum(size)
            ReDim .rX(size)
            ReDim .rY(size)
            ReDim .rZ(size)
            .count = size
        End With
    End Sub
    Sub InitializeVertexPreserve(ByRef vertexArray As Vertexes, ByVal size As Integer)
        With vertexArray
            ReDim Preserve .nX(size)
            ReDim Preserve .nY(size)
            ReDim Preserve .nZ(size)
            ReDim Preserve .pX(size)
            ReDim Preserve .pY(size)
            ReDim Preserve .pZ(size)
            ReDim Preserve .tU(size)
            ReDim Preserve .tV(size)
            ReDim Preserve .Color(size)
            ReDim Preserve .temp(size)
            ReDim Preserve .material(size)
            ReDim Preserve .materialUID(size)
            ReDim Preserve .triangleNum(size)
            ReDim Preserve .rX(size)
            ReDim Preserve .rY(size)
            ReDim Preserve .rZ(size)
            .count = size
        End With
    End Sub
    Sub InitializeVertexDraw(ByRef vertexArray As VertexesDraw, ByVal size As Integer)
        With vertexArray
            ReDim .scrX(size)
            ReDim .scrY(size)
            ReDim .hidden(size)
            ReDim .w(size)
        End With
    End Sub
    Sub InitializeTrianglePreserve(ByRef triangleArray As Triangles, ByVal size As Integer)
        With triangleArray
            ReDim Preserve .v1(size)
            ReDim Preserve .v2(size)
            ReDim Preserve .v3(size)
            ReDim Preserve .nX(size)
            ReDim Preserve .nY(size)
            ReDim Preserve .nZ(size)
            ReDim Preserve .materialID(size)
            ' ReDim Preserve .material(size)
            '  ReDim Preserve .materialUID(size)
            ReDim Preserve .textureSize(size)
            ReDim Preserve .renderSettings(size)
            .count = size '+ 1
        End With
    End Sub
    Sub InitializeTriangleArrays(ByRef triangleArray As Triangles, ByVal size As Integer)
        With triangleArray
            ReDim .v1(size)
            ReDim .v2(size)
            ReDim .v3(size)
            ReDim .nX(size)
            ReDim .nY(size)
            ReDim .nZ(size)
            ReDim .materialID(size)
            ' ReDim .material(size)
            ' ReDim .materialUID(size)
            ReDim .textureSize(size)
            ReDim .renderSettings(size)
            .count = size '+ 1
        End With
    End Sub
    Sub InitializeTrianglesDraw(ByRef triangleArray As TrianglesDraw, ByVal size As Integer)
        With triangleArray
            ReDim .minX(size)
            ReDim .maxX(size)
            ReDim .minY(size)
            ReDim .maxY(size)
            ReDim .screenSize(size)
            ReDim .minW(size)
            ReDim .maxW(size)
        End With
    End Sub
    Sub VertexCopy(ByRef vertexArray1 As Vertexes, ByRef vertexArray2 As Vertexes, ByVal index1 As Integer, ByVal index2 As Integer)
        vertexArray2.pX(index2) = vertexArray1.pX(index1)
        vertexArray2.pY(index2) = vertexArray1.pY(index1)
        vertexArray2.pZ(index2) = vertexArray1.pZ(index1)
        vertexArray2.nX(index2) = vertexArray1.nX(index1)
        vertexArray2.nY(index2) = vertexArray1.nY(index1)
        vertexArray2.nZ(index2) = vertexArray1.nZ(index1)
        vertexArray2.tU(index2) = vertexArray1.tU(index1)
        vertexArray2.tV(index2) = vertexArray1.tV(index1)
        vertexArray2.Color(index2) = vertexArray1.Color(index1)
        vertexArray2.temp(index2) = vertexArray1.temp(index1)
        vertexArray2.material(index2) = vertexArray1.material(index1)
        vertexArray2.materialUID(index2) = vertexArray1.materialUID(index1)
        vertexArray2.rX(index2) = vertexArray1.rX(index1)
        vertexArray2.rY(index2) = vertexArray1.rY(index1)
        vertexArray2.rZ(index2) = vertexArray1.rZ(index1)
    End Sub
    Sub TriangleCopy(ByRef triangleArray1 As Triangles, ByRef triangleArray2 As Triangles, ByVal index1 As Integer, ByVal index2 As Integer)
        triangleArray2.nX(index2) = triangleArray1.nX(index1)
        triangleArray2.nY(index2) = triangleArray1.nY(index1)
        triangleArray2.nZ(index2) = triangleArray1.nZ(index1)
        triangleArray2.textureSize(index2) = triangleArray1.textureSize(index1)
        triangleArray2.materialID(index2) = triangleArray1.materialID(index1)
        '  triangleArray2.material(index2) = triangleArray1.material(index1)
        '  triangleArray2.materialUID(index2) = triangleArray1.materialUID(index1)
        triangleArray2.renderSettings(index2) = triangleArray1.renderSettings(index1)
    End Sub
End Module
