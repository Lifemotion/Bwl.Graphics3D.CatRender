Public Class Model
    Const makeMipMapsOnLoad As Integer = 0
    Private Enum LoaderXNowGoing
        none = 0
        mesh = 1
        material = 2
        materialList = 3
        textureCoords = 4
        meshNormals = 5
        textureName = 6
    End Enum
    Private maxMeshes As Integer
    Private maxMaterials As Integer
    Private loadedFrom As String
    Public boundRadius As Single
    Public boundX, boundY, boundZ As Single
    ''' <summary>
    ''' Массив сеток, хранящих их имя, иерархию, массивы треугольников и вершин.
    ''' </summary>
    ''' <remarks></remarks>
    Public meshes() As Mesh
    Public name As String
    ''' <summary>
    '''   Библиотека материалов модели
    ''' </summary>
    ''' <remarks></remarks>
    Public materialsLib() As Material
    Private Shared globalTexturesLib() As PixelSurface
    Private Shared globalTexturesLibMipMap()() As PixelSurface
    Private Shared globalTexturesCount As Integer
    Private Shared globalMipMapLevels() As Integer
    Private usingGlobalTextures As Boolean
    Public ReadOnly Property MeshesCount() As Integer
        Get
            If meshes Is Nothing Then Return 0
            Return meshes.GetUpperBound(0) + 1
        End Get
    End Property

    ''' <summary>
    ''' Загружает модель из файла *.X
    ''' </summary>
    ''' <param name="filename">Имя файла с моделью</param>
    ''' <remarks></remarks>
    Public Sub LoadFromX(ByVal filename As String, Optional ByVal useGlobalTextureLib As Boolean = True)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)
        'сбрасываем верхние границы массивов
        maxMeshes = -1
        maxMaterials = -1
        usingGlobalTextures = useGlobalTextureLib
        Dim freeFileNum As Integer = FreeFile()
        Dim errors As String = ""
        Dim notOpened As Boolean
        'пробуем открыть файл
        Try
            FileOpen(freeFileNum, filename, OpenMode.Input)
        Catch ex As Exception
            errors = "FILE:NotOpened" + vbCrLf
            notOpened = True
        End Try
        'если файл удачно открылся, читаем его
        If Not notOpened Then
            Dim fileLine As String = ""
            Dim parts() As String
            Dim partsLast As Integer
            Dim nowGoing As LoaderXNowGoing = LoaderXNowGoing.none
            Dim state As Integer
            Dim vertexCount As Integer
            Dim vertexNow As Integer
            Dim triangleCount As Integer
            Dim triangleNow As Integer
            Dim materialCount As Integer
            Dim materialNow As Integer
            Dim materialTmp2Real() As Integer
            Dim materialIDBuff() As Integer
            ReDim materialTmp2Real(0)
            ReDim materialIDBuff(0)
            Do While Not EOF(freeFileNum)
                'читаем очередную строку из файла
                fileLine = LineInput(freeFileNum)
                fileLine = Trim(fileLine).ToUpper
                If fileLine > "" Then
                    parts = Split(fileLine, " ")
                    partsLast = parts.GetUpperBound(0)
                    'определяем начало секции или конец
                    'хитрую вложенность не поддерживаем ;)
                    'текущий вид процедуры читает .X только стандартного форматирования
                    If nowGoing = LoaderXNowGoing.textureName Then
                        fileLine = Replace(fileLine, """", "")
                        fileLine = Replace(fileLine, ";", "")
                        materialsLib(maxMaterials).textureName = fileLine
                        materialsLib(maxMaterials).textureUsed = True
                        nowGoing = LoaderXNowGoing.none
                    End If
                    If nowGoing = LoaderXNowGoing.mesh Then
                        'сетка - треугольники
                        If state = 3 Then
                            fileLine = Replace(fileLine, ",", ";")
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) >= 4 AndAlso Val(parts(0)) = 3 Then
                                If triangleNow < triangleCount Then
                                    meshes(maxMeshes).Triangles.v1(triangleNow) = Val(parts(1))
                                    meshes(maxMeshes).Triangles.v2(triangleNow) = Val(parts(2))
                                    meshes(maxMeshes).Triangles.v3(triangleNow) = Val(parts(3))
                                    triangleNow += 1
                                    If triangleNow = triangleCount Then
                                        state = -1
                                        nowGoing = LoaderXNowGoing.none
                                    End If
                                Else
                                    errors += "MESH:VertexCountOrNotTriangle" + vbCrLf
                                End If
                            Else
                                triangleNow += 1
                                errors += "MESH:TriangleFormat" + vbCrLf
                            End If
                        End If

                        'сетка - количество треугольников
                        If state = 2 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) <= 1 Then
                                triangleCount = Val(parts(0))
                                triangleNow = 0
                                InitializeTriangleArrays(meshes(maxMeshes).Triangles, triangleCount)
                                state = 3
                            Else
                                errors += "MESH:NumberOfTriangles" + vbCrLf
                            End If
                        End If
                        'сетка - вершины
                        If state = 1 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) >= 3 Then
                                If vertexNow < vertexCount Then
                                    meshes(maxMeshes).Vertexes.pX(vertexNow) = Val(parts(0))
                                    meshes(maxMeshes).Vertexes.pY(vertexNow) = Val(parts(2))
                                    meshes(maxMeshes).Vertexes.pZ(vertexNow) = Val(parts(1))
                                    vertexNow += 1
                                    If vertexNow = vertexCount Then
                                        state = 2
                                    End If
                                Else
                                    errors += "MESH:VertexCount" + vbCrLf
                                End If
                            Else
                                vertexNow += 1
                                errors += "MESH:VertexFormat" + vbCrLf
                            End If
                        End If
                        'сетка координаты - количество вершин
                        If state = 0 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) <= 1 Then
                                vertexCount = Val(parts(0))
                                vertexNow = 0
                                state = 1
                                InitializeVertexArrays(meshes(maxMeshes).Vertexes, vertexCount)
                            Else
                                errors += "MESH:NumberOfVertexes" + vbCrLf
                            End If
                        End If
                    End If
                    If nowGoing = LoaderXNowGoing.textureCoords Then
                        'текстурные - вершины
                        If state = 1 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) >= 2 Then
                                If vertexNow < vertexCount Then
                                    meshes(maxMeshes).Vertexes.tU(vertexNow) = Val(parts(0))
                                    meshes(maxMeshes).Vertexes.tV(vertexNow) = Val(parts(1))
                                    vertexNow += 1
                                    If vertexNow = vertexCount Then
                                        nowGoing = LoaderXNowGoing.none
                                    End If
                                Else
                                    errors += "MESH:TexCoordsCount" + vbCrLf
                                End If
                            Else
                                vertexNow += 1
                                errors += "MESH:TexCoordsFormat" + vbCrLf
                            End If
                        End If
                        'текстурные - количество вершин
                        If state = 0 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) <= 1 Then
                                vertexCount = Val(parts(0))
                                vertexNow = 0
                                state = 1
                            Else
                                errors += "MESH:NumOfTexCoords" + vbCrLf
                            End If
                        End If
                    End If
                    If nowGoing = LoaderXNowGoing.materialList Then
                        'материал - названия материалов
                        If state = 3 Then
                            fileLine = Replace(fileLine, "{", "")
                            fileLine = Replace(fileLine, "}", "")
                            fileLine = Trim(fileLine)
                            Dim i As Integer
                            For i = 0 To maxMaterials
                                materialTmp2Real(materialNow) = -1
                                If materialsLib(i).name.ToUpper = fileLine Then
                                    materialTmp2Real(materialNow) = i
                                    Exit For
                                End If
                            Next
                            materialNow += 1
                            If triangleCount = triangleNow Then
                                nowGoing = LoaderXNowGoing.none
                                For i = 0 To triangleCount - 1
                                    meshes(maxMeshes).Triangles.materialID(i) = materialTmp2Real(materialIDBuff(i))
                                Next
                            End If
                        End If
                        'материал - индексы материалов
                        If state = 2 Then
                            fileLine = Replace(fileLine, ";", "")
                            fileLine = Replace(fileLine, ",", "")
                            If IsNumeric(fileLine) Then
                                materialIDBuff(triangleNow) = Val(fileLine)
                            Else
                                errors += "MESH:TexCoordsFormat" + vbCrLf
                            End If
                            triangleNow += 1
                            If triangleCount = triangleNow Then state = 3
                        End If
                        'материал - количество треугольников
                        If state = 1 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) <= 1 Then
                                triangleCount = Val(parts(0))
                                triangleNow = 0
                                state = 2
                                ReDim materialIDBuff(triangleCount - 1)
                            Else
                                errors += "MESH:MatListCount" + vbCrLf
                            End If
                        End If
                        'материал - количество материалов
                        If state = 0 Then
                            parts = Split(fileLine, ";")
                            If parts.GetUpperBound(0) <= 1 Then
                                ReDim materialTmp2Real(Val(parts(0)) - 1)
                                materialCount = Val(parts(0))
                                materialNow = 0
                                state = 1
                            Else
                                errors += "MESH:MatListCount" + vbCrLf
                            End If
                        End If
                    End If
                    'начинается описание материала
                    If parts(0) = "MATERIAL" Then
                        maxMaterials += 1
                        ReDim Preserve materialsLib(maxMaterials)
                        materialsLib(maxMaterials) = New Material(parts(1))
                        materialsLib(maxMaterials).textureUsed = False
                        nowGoing = LoaderXNowGoing.material
                    End If

                    'имя текстуры из описания материала
                    If parts(0) = "TEXTUREFILENAME" And nowGoing = LoaderXNowGoing.material Then nowGoing = LoaderXNowGoing.textureName

                    'описание вершин и поверхности
                    If parts(0) = "MESH" Then
                        state = 0
                        nowGoing = LoaderXNowGoing.mesh
                        maxMeshes += 1
                        ReDim Preserve meshes(maxMeshes)
                        meshes(maxMeshes).Name = parts(1)
                    End If

                    'текстурные координаты
                    If parts(0) = "MESHTEXTURECOORDS" Then
                        state = 0
                        nowGoing = LoaderXNowGoing.textureCoords
                    End If

                    'список материалов для вершин
                    If parts(0) = "MESHMATERIALLIST" Then
                        state = 0
                        nowGoing = LoaderXNowGoing.materialList
                    End If

                    'список материалов для вершин
                    If parts(0) = "MESHNORMALS" Then
                        state = 0
                        nowGoing = LoaderXNowGoing.meshNormals
                    End If
                End If
                '  Dim line As Integer
                '   line += 1
                '  If errors > "" Then Stop
            Loop
        End If
        FileClose(freeFileNum)
        'Если не было ошибок, грузим текстуры
        If errors = "" Then
            loadedFrom = filename
            LoadTextures(filename)
        Else
            MsgBox("Загрузка модели из .X прошла с ошибками:" + vbCrLf + errors)
        End If
        ComputeNormals()
    End Sub
    ''' <summary>
    ''' Загружает текстуры в материалы по указанным там именам
    ''' </summary>
    ''' <param name="modelFileName"></param>
    ''' <remarks></remarks>
    Private Sub LoadTextures(ByVal modelFileName As String)
        Dim i, j As Integer
        Dim errors As String = ""

        modelFileName = modelFileName.Replace("\", IO.Path.DirectorySeparatorChar)
        'определяем путь до файла модели
        Dim path As String = ""
        i = InStrRev(modelFileName, IO.Path.DirectorySeparatorChar)

        If i > 0 Then
            path = Mid(modelFileName, 1, i)
        End If
        'проходим по массиву матеиалов и загружаем текстуры
        'если они предусмотрены
        Dim textureFound As PixelSurface = Nothing
        Dim textureFoundNum As Integer
        For i = 0 To materialsLib.GetUpperBound(0)
            If materialsLib(i).textureUsed Then
                'если используем глобальный массив текстур
                If usingGlobalTextures Then
                    For j = 0 To globalTexturesCount - 1
                        If globalTexturesLib(j).loadedFromFile = path + materialsLib(i).textureName Then
                            textureFound = globalTexturesLib(j)
                            textureFoundNum = j
                            Exit For
                        End If
                    Next
                End If
                If textureFound Is Nothing Then
                    materialsLib(i).texturePixels(0) = New PixelSurface
                    Try
                        '  MsgBox(path)
                        '   MsgBox(Application.StartupPath)
                        materialsLib(i).texturePixels(0).LoadFromFile(path + materialsLib(i).textureName)
                        If makeMipMapsOnLoad Then materialsLib(i).MakeMipMaps(makeMipMapsOnLoad)
                        If usingGlobalTextures Then
                            ReDim Preserve globalTexturesLib(globalTexturesCount)
                            ReDim Preserve globalTexturesLibMipMap(globalTexturesCount)
                            ReDim Preserve globalMipMapLevels(globalTexturesCount)
                            globalTexturesLib(globalTexturesCount) = materialsLib(i).texturePixels(0)
                            globalMipMapLevels(globalTexturesCount) = materialsLib(i).maximumMipMapLevel
                            'ресайзим массив мипмапных текстур
                            'For i = 0 To 8
                            ReDim globalTexturesLibMipMap(globalTexturesCount)(8)
                            Dim k As Integer
                            For k = 1 To materialsLib(i).maximumMipMapLevel - 1
                                globalTexturesLibMipMap(globalTexturesCount)(k) = materialsLib(i).texturePixels(k)
                            Next k
                            globalTexturesCount += 1
                        End If
                    Catch ex As Exception
                        errors += "MESH:TextureNotFound:" + materialsLib(i).textureName + " " + ex.Message + vbCrLf
                        materialsLib(i).textureUsed = False
                    End Try
                Else
                    materialsLib(i).texturePixels(0) = textureFound
                    Dim k As Integer
                    materialsLib(i).maximumMipMapLevel = globalMipMapLevels(textureFoundNum)
                    For k = 1 To globalMipMapLevels(textureFoundNum) - 1
                        materialsLib(i).texturePixels(k) = globalTexturesLibMipMap(textureFoundNum)(k)
                    Next
                End If
            End If
        Next
        If errors = "" Then
        Else
            MsgBox("Загрузка модели из .X прошла с ошибками:" + vbCrLf + errors)
        End If
    End Sub
    ''' <summary>
    ''' Собирает массивы данных вершин в набор однородных массивов
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Prepare()
        Dim mesh, i As Integer
        boundRadius = 0
        boundX = 0
        boundY = 0
        boundZ = 0
        For mesh = 0 To MeshesCount - 1
            With meshes(mesh)
                'инициализируем подготовленный массив информации
                InitializeVertexArrays(.PreparedVertexes, .Triangles.count * 3)
                'собираем нужную информацию из массива треугольников
                For i = 0 To .Triangles.count - 1
                    Dim mID As Integer = .Triangles.materialID(i)
                    'копируем в буфер подготовленной информации
                    VertexCopy(.Vertexes, .PreparedVertexes, .Triangles.v1(i), i * 3 + 0)
                    VertexCopy(.Vertexes, .PreparedVertexes, .Triangles.v2(i), i * 3 + 1)
                    VertexCopy(.Vertexes, .PreparedVertexes, .Triangles.v3(i), i * 3 + 2)
                    'раздаем информацию о текстурах каждой вершине
                    '(хотя она им так-то нахрен не нужна ;))
                    Dim UID As Integer
                    Dim material As Material
                    If mID >= 0 Then
                        UID = materialsLib(mID).UID
                        material = materialsLib(mID)
                        .PreparedVertexes.materialUID(i * 3 + 0) = UID
                        .PreparedVertexes.materialUID(i * 3 + 1) = UID
                        .PreparedVertexes.materialUID(i * 3 + 2) = UID
                        .PreparedVertexes.material(i * 3 + 0) = material
                        .PreparedVertexes.material(i * 3 + 1) = material
                        .PreparedVertexes.material(i * 3 + 2) = material
                    End If
                Next
                Dim ax, ay, az, radius As Single
                For i = 0 To .Vertexes.count
                    ax = Abs(.Vertexes.pX(i))
                    ay = Abs(.Vertexes.pY(i))
                    az = Abs(.Vertexes.pZ(i))
                    radius = Math.Abs(ax * ax + ay * ay + az * az)
                    If ax > boundX Then boundX = ax
                    If ay > boundY Then boundY = ay
                    If az > boundZ Then boundZ = az
                    If radius > boundRadius Then boundRadius = radius
                Next
            End With
            ComputeTextureSize()
        Next
    End Sub
    Private Function Abs(ByVal value As Single) As Single
        If value > 0 Then Return value Else Return -value
    End Function
    ''' <summary>
    ''' Рассчитывает нормали к треугольникам во всех сетках
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ComputeNormals()
        Dim mesh As Integer
        For mesh = 0 To MeshesCount - 1
            ComputeNormalsMesh(mesh)
        Next
    End Sub
    ''' <summary>
    ''' Рассчитывает нормали к треугольникам в указанной сетке
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ComputeNormalsMesh(ByVal meshNumber As Integer)
        With meshes(meshNumber)
            Dim i, v1, v2, v3 As Integer
            Dim x1, x2, y1, y2, z1, z2, xR, zR, yR As Single
            'обнялем счетчики числа доавленные к вершине нормалей
            For i = 0 To .Vertexes.count - 1
                .Vertexes.temp(i) = 0
                .Vertexes.nX(i) = 0
                .Vertexes.nY(i) = 0
                .Vertexes.nZ(i) = 0
            Next
            'идем по массиву треугольников
            For i = 0 To .Triangles.count - 1
                v1 = .Triangles.v1(i)
                v2 = .Triangles.v2(i)
                v3 = .Triangles.v3(i)
                x1 = .Vertexes.pX(v2) - .Vertexes.pX(v1)
                y1 = .Vertexes.pY(v2) - .Vertexes.pY(v1)
                z1 = .Vertexes.pZ(v2) - .Vertexes.pZ(v1)
                x2 = .Vertexes.pX(v3) - .Vertexes.pX(v1)
                y2 = .Vertexes.pY(v3) - .Vertexes.pY(v1)
                z2 = .Vertexes.pZ(v3) - .Vertexes.pZ(v1)
                CrossProduct(x1, y1, z1, x2, y2, z2, xR, yR, zR, True)
                If zR > 1 Then Stop
                .Triangles.nX(i) = xR
                .Triangles.nY(i) = yR
                .Triangles.nZ(i) = zR
                .Vertexes.nX(v1) += xR
                .Vertexes.nX(v2) += xR
                .Vertexes.nX(v3) += xR
                .Vertexes.nY(v1) += yR
                .Vertexes.nY(v2) += yR
                .Vertexes.nY(v3) += yR
                .Vertexes.nZ(v1) += zR
                .Vertexes.nZ(v2) += zR
                .Vertexes.nZ(v3) += zR
                .Vertexes.temp(v1) += 1
                .Vertexes.temp(v2) += 1
                .Vertexes.temp(v3) += 1
            Next
            For i = 0 To .Vertexes.count - 1
                If .Vertexes.temp(i) > 0 Then
                    .Vertexes.nX(i) /= .Vertexes.temp(i)
                    .Vertexes.nY(i) /= .Vertexes.temp(i)
                    .Vertexes.nZ(i) /= .Vertexes.temp(i)
                    If .Vertexes.nZ(i) > 1 Then Stop
                End If
            Next
        End With
    End Sub
    ''' <summary>
    ''' Векторное произведение 3D векторов, нормализация
    ''' </summary>
    ''' <param name="x1"></param>
    ''' <param name="y1"></param>
    ''' <param name="z1"></param>
    ''' <param name="x2"></param>
    ''' <param name="y2"></param>
    ''' <param name="z2"></param>
    ''' <param name="xR"></param>
    ''' <param name="yR"></param>
    ''' <param name="zR"></param>
    ''' <param name="normalize"></param>
    ''' <remarks></remarks>
    Private Shared Sub CrossProduct(ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single, ByRef xR As Single, ByRef yR As Single, ByRef zR As Single, Optional ByVal normalize As Boolean = False)
        Dim x, y, z As Single
        Dim length As Single
        x = y1 * z2 - z1 * y2
        y = z1 * x2 - x1 * z2
        z = x1 * y2 - y1 * x2
        If normalize Then
            length = 1.0 / Math.Sqrt(x * x + y * y + z * z)
            x *= length
            y *= length
            z *= length
        End If
        xR = x
        yR = y
        zR = z
    End Sub
    ''' <summary>
    ''' Считает площадь текстуры на полигоне для всех сеток
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ComputeTextureSize()
        Dim meshNumber As Integer
        For meshNumber = 0 To MeshesCount - 1
            With meshes(meshNumber)
                Dim i, v1, v2, v3 As Integer
                Dim x1, x2, y1, y2, x3, y3 As Single
                Dim size As Single
                Dim sideA, sideB, sideC As Single
                Dim halfPerimetr As Single
                'идем по массиву треугольников
                For i = 0 To .Triangles.count - 1
                    v1 = .Triangles.v1(i)
                    v2 = .Triangles.v2(i)
                    v3 = .Triangles.v3(i)
                    x1 = .Vertexes.tU(v1)
                    y1 = .Vertexes.tV(v1)
                    x2 = .Vertexes.tU(v2)
                    y2 = .Vertexes.tV(v2)
                    x3 = .Vertexes.tU(v3)
                    y3 = .Vertexes.tV(v3)
                    sideA = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))
                    sideB = Math.Sqrt((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2))
                    sideC = Math.Sqrt((x1 - x3) * (x1 - x3) + (y1 - y3) * (y1 - y3))
                    halfPerimetr = (sideA + sideB + sideC) / 2
                    size = Math.Sqrt(halfPerimetr * (halfPerimetr - sideA) * (halfPerimetr - sideB) * (halfPerimetr - sideC))
                    .Triangles.textureSize(i) = size
                Next
            End With
        Next
    End Sub
    Public Sub MakeMipmaps()
        Dim i As Integer
        For i = 0 To materialsLib.GetUpperBound(0)
            '  materialsLib(i).MakeMipMaps()
        Next
    End Sub
    Public Function GetCopy() As Model
        Dim model As New Model
        With model
            .boundRadius = boundRadius
            .boundX = boundX
            .boundY = boundY
            .boundZ = boundZ
            .loadedFrom = loadedFrom
            ReDim .materialsLib(materialsLib.Length - 1)
            For i As Integer = 0 To materialsLib.Length - 1
                .materialsLib(i) = materialsLib(i).GetCopy
            Next
            .maxMaterials = maxMaterials
            .maxMeshes = maxMeshes
            ReDim .meshes(meshes.Length - 1)
            For i As Integer = 0 To meshes.Length - 1
                .meshes(i) = meshes(i)
                ';.meshes(i).Name = meshes(i).Name
                ' .meshes(i).Triangles.
            Next
            .name = name
            .usingGlobalTextures = usingGlobalTextures
        End With
        Return model
    End Function
End Class

