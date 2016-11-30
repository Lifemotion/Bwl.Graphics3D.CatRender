Partial Public Class Render3D
    Public Class SceneDrawClass
        Public defaults As RenderParameters
        Public settings As RenderSettings
        Public environment As EnvironmentSettings
        'константные настроечки
        Protected Const sqrtTableSize As Integer = 1000000
        Protected Const vertexSizingStep As Integer = 1024
        Protected Const spritesSizingStep As Integer = 128
        Protected Const drawSpanSize As Integer = 16
        Protected Const drawSpanSizeFast As Integer = 96
        Protected Const trianglesBiggestBuffer As Integer = 0
        Protected Const zSortMaximum As Integer = 100000
        Protected Const zHighStep As Integer = 64
        Protected Const shadowPlanesCount As Integer = 4
        Protected myParent As Render3D
        'буфер статических данных вершин
        Protected vertexes As Vertexes
        'буфер расчетных данных вершин
        ' Private vertexesDraw As VertexesDraw
        Protected vertexesDraw As VertexesDraw
        'соотв. буферы для вершин после отсечения и фильтрафии невидимых
        Protected vertexesCulled As Vertexes
        Protected vertexesCulledSwap As Vertexes
        'указатель на максимум в буфере вершин после отсечения
        Protected vertexCulledTop As Integer
        'указатели на текущий, предельный и верхний элементы буферов данных вершин
        Protected vertexPointer As Integer
        Protected vertexMaximum As Integer
        Protected vertexTop As Integer
        'буферы на треугольники
        Protected triangles As Triangles
        Protected trianglesCulled As Triangles
        Protected trianglesCulledSwap As Triangles
        Protected trianglesDraw As TrianglesDraw
        Protected trianglesBiggest As TrianglesDraw
        Protected trianglesTop As Integer
        Protected trianglesCulledTop As Integer
        Protected trianglesPointer As Integer
        'матрицы для трансформаций положения модели
        Protected matrixRotX As New TransformMatrix
        Protected matrixRotY As New TransformMatrix
        Protected matrixRotZ As New TransformMatrix
        Protected matrixScale As New TransformMatrix
        Protected matrixTransition1 As New TransformMatrix
        Protected matrixTransition2 As New TransformMatrix
        Protected matrixFinal As New TransformMatrix
        ' Private width, height As Integer
        'W-буфер, jосновной
        'Private Shared a As Integer
        Protected wBuffer() As Integer
        Protected wBufferHigh() As Integer
        'N-буфер, номер треугольника
        Protected nBuffer() As Integer
        'Z-буфер, дополнительный
        Protected zBuffer() As Single
        'Текущая текстура и материал
        Protected materialUID As Integer = -1
        Protected texturePixels() As PixelSurface
        Protected textureMipMap As Integer
        Protected textureSizeX, textureSizeY As Integer
        Protected textureUse As Boolean
        Protected currentCamera As Camera3D
        Protected cameraDist As Single
        Protected infoTrianglesDrawed As Integer
        Protected infoPixelsDrawed As Integer
        Protected infoPixelsWBuffered As Integer
        Protected sprites() As SpriteStruct
        Protected spritesTop As Integer
        Protected spritesMaximum As Integer
        'предтаблицы для тумана и эффектов
        Protected fogTable1() As Byte
        Protected fogTable2() As Byte
        Protected fogTable3() As Byte
        Protected fogColorR, fogColorB, fogColorG As Integer
        Protected tempPixels() As Byte
        Protected distortionTable() As Integer
        Protected preparedDistortionTableA As Single
        Protected preparedDistortionTableB As Single
        Protected optionsMipMapping As Boolean
        Protected optionsBilinearTextures As Boolean
        Protected optionsSortTriangles As Boolean
        'Private bilinearTable1() As Integer
        Protected cameraVectorsX() As Integer
        Protected cameraVectorsY() As Integer
        Protected cameraVectorsZ() As Integer
        'массивы для сортировки полигонов
        Protected sortZ1(), sortIndex1() As Integer
        Protected sortZ2(), sortIndex2() As Integer
        Protected lighterPoint As LighterStruct
        Protected ambientR, ambientG, ambientB As Integer
        Protected skyR, skyG, skyB As Integer
        Protected width, height As Integer
        'корни квадратные, на 16 умноженные, sqrtTableSize шт.
        Protected Shared sqrtTable() As Integer
        'карты теней
        Public shadowPlanes() As ShadowPlane
        Protected shadowsUsed As Boolean
        Public Property DebugSpritesList As New List(Of SpriteStruct)

        Public Sub New(ByRef parent As Render3D)
            myParent = parent
            '  currentCamera = New Camera3D
            vertexMaximum = -1
            spritesMaximum = -1
            'PrepareFogTables()
            'PrepareBilinearTable()
            PrepareSQRTTable()
            ambientR = 255
            ambientG = 255
            ambientB = 255
            defaults.lighting = LightingMode.full
            defaults.mipMap = MipMapMode.mipMapOff
            defaults.texturing = TexturingMode.normal
            defaults.renderer = RenderMode.normal
            defaults.normals = NormalsInterpolationMode.interpolateByMesh
            defaults.culling = NonfacialCulling.positive
            settings.drawLines = False
            settings.drawTriangles = True
            settings.sortTriangles = True
            settings.useFastCulling = True
            defaults.renderer = RenderMode.normal
            ' defaults.culling = NonfacialCulling.none
            ReDim shadowPlanes(shadowPlanesCount - 1)
            'defaults.renderer = RenderMode.fast
            'defaults.texturing = TexturingMode.bilinear
            'defaults.lighting = LightingMode.none
            'environment.fog = 2000
        End Sub
        Public Sub Init()
            width = myParent.width
            height = myParent.height
            InitializeTrianglesDraw(trianglesBiggest, trianglesBiggestBuffer)
            Clear()
            ReDim wBuffer((myParent.width + 2) * (myParent.height + 2))
            ReDim wBufferHigh((myParent.width \ zHighStep + 2) * (myParent.height + 2))
            ReDim tempPixels((myParent.width + 10) * (myParent.height + 10) * 4)
            PrepareCameraVectors()
            'ReDim pixels((myParent.width + 2) * (myParent.height + 2) * 3)
        End Sub
        ''' <summary>
        ''' Очищает сцену, буферы вершин
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear()
            'InitializeVertexArrays(vertexes, vertexMaximum)
            '     InitializeVertexDraw(vertexesDraw, vertexMaximum)
            infoTrianglesDrawed = 0
            infoPixelsWBuffered = 0
            infoPixelsDrawed = 0
            vertexPointer = -1
            'vertexMaximum = vertexSizingStep - 1
            vertexTop = -1
            spritesTop = -1
            trianglesTop = -1
            trianglesPointer = -1
            Array.Clear(trianglesBiggest.screenSize, 0, trianglesBiggestBuffer)
            DebugSpritesList.Clear()
            '  InitializeTrianglesDraw(trianglesBiggest, trianglesBiggestBuffer)
        End Sub
        Public Sub AddLighter(ByRef lighter As LighterStruct)
            If lighter.type = LighterTypeEnum.ambient Then
                ambientR = lighter.r
                ambientG = lighter.g
                ambientB = lighter.b
            End If
            If lighter.type = LighterTypeEnum.point Then
                lighterPoint = lighter
            End If
        End Sub
        Public Sub AddLighter(ByRef lighter As Lighter, ByVal x As Single, ByVal y As Single, ByVal z As Single)
            If lighter.type = LighterTypeEnum.ambient Then
                ambientR = lighter.colorR
                ambientG = lighter.colorG
                ambientB = lighter.colorB
            End If
            If lighter.type = LighterTypeEnum.sky Then
                skyR = lighter.colorR
                skyG = lighter.colorG
                skyB = lighter.colorB
            End If
            If lighter.type = LighterTypeEnum.point Then
                Dim tmpLighter As LighterStruct
                With tmpLighter
                    .r = lighter.colorR
                    .g = lighter.colorG
                    .b = lighter.colorB
                    .type = lighter.type
                    .intense = lighter.intense
                    .attenutionA = lighter.attenutionA
                    .attenutionB = lighter.attenutionB
                    .x = x
                    .y = y
                    .z = z
                End With
                lighterPoint = tmpLighter
            End If
        End Sub
        ''' <summary>
        ''' Добавляет к списку отрисовки спрайт
        ''' </summary>
        ''' <param name="sprite">Спрайт</param>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <remarks></remarks>
        Public Sub DrawSprite(ByRef sprite As Sprite, ByVal x As Single, ByVal y As Single, ByVal z As Single, Optional lighter As Lighter = Nothing, Optional UID As Integer = -1)
            spritesTop += 1
            If spritesTop > spritesMaximum Then
                spritesMaximum += spritesSizingStep
                ReDim Preserve sprites(spritesMaximum)
            End If
            With sprites(spritesTop)
                .x = x
                .y = y
                .z = z
                .UID = UID
                .sprite = sprite
                .lighter = lighter
                .px = -1
                .py = -1
            End With
        End Sub
        Public Sub DrawObject(ByRef object3D As Object3D)
            With object3D
                If .hidden Then Return
                If .type = Object3DType.model Then
                    DrawModel(.model, .modelMesh, .positionX, .positionY, .positionZ, .rotateX, .rotateY, .rotateZ, .scale, .renderMode, .UID)
                End If
                If .type = Object3DType.sprite Then
                    DrawSprite(.sprite, .positionX, .positionY, .positionZ, .light, .UID)
                End If
                If .type = Object3DType.light Then
                    AddLighter(.light, .positionX, .positionY, .positionZ)
                End If
            End With
        End Sub
        Public Overloads Sub DrawModel(ByRef model As Model, ByVal meshIndex As Integer, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal ax As Single, ByVal ay As Single, ByVal az As Single, ByVal scale As Single)
            DrawModel(model, meshIndex, x, y, z, ax, ay, az, scale, defaults)
        End Sub
        ''' <summary>
        ''' Добавить в буфер отрисовки модель с указанием координат, поворота и параметров отрисовки.
        ''' </summary>
        ''' <param name="model">Указатель на модель</param>
        ''' <param name="meshIndex">Индекс сетки модели</param>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <param name="ax"></param>
        ''' <param name="ay"></param>
        ''' <param name="az"></param>
        ''' <param name="scale"></param>
        ''' <remarks></remarks>
        Public Overloads Sub DrawModel(ByRef model As Model, ByVal meshIndex As Integer, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal ax As Single, ByVal ay As Single, ByVal az As Single, ByVal scale As Single, ByRef renderParameters As RenderParameters, Optional UID As Integer = -1)
            Dim mesh1, mesh2 As Integer
            Dim i As Integer
            If meshIndex >= 0 Then
                mesh1 = meshIndex : mesh2 = meshIndex
            Else
                mesh1 = 0
                mesh2 = model.MeshesCount - 1
            End If
            For meshIndex = mesh1 To mesh2
                'заполняем матрицы перемещения и поворотов
                matrixScale.Scaling(scale)
                matrixTransition1.Transition(x, y, z)
                matrixRotX.RotationX(ax)
                matrixRotY.RotationY(ay)
                matrixRotZ.RotationZ(az)
                'находим произведение матриц
                matrixTransition1.MulOnMatrix(matrixScale)
                matrixTransition1.MulOnMatrix(matrixRotX)
                matrixTransition1.MulOnMatrix(matrixRotZ)
                matrixTransition1.MulOnMatrix(matrixRotY)
                matrixFinal.CopyFrom(matrixTransition1)
                Dim modelVertexes As Vertexes = model.meshes(meshIndex).PreparedVertexes
                Dim modelTraingles As Triangles = model.meshes(meshIndex).Triangles
                Dim tempRenderParams As RenderParameters

                With modelVertexes
                    'увеличиваем размер общего буфера вершин, если нужно
                    Do While .count + vertexPointer > vertexMaximum
                        InitializeVertexPreserve(vertexes, vertexMaximum + vertexSizingStep)
                        InitializeVertexArrays(vertexesCulled, 2 * (vertexMaximum + vertexSizingStep))
                        InitializeVertexArrays(vertexesCulledSwap, 2 * (vertexMaximum + vertexSizingStep))
                        InitializeVertexDraw(vertexesDraw, 2 * (vertexMaximum + vertexSizingStep))
                        InitializeTrianglePreserve(triangles, (vertexMaximum + vertexSizingStep) \ 3)
                        InitializeTriangleArrays(trianglesCulled, 2 * (vertexMaximum + vertexSizingStep) \ 3)
                        InitializeTriangleArrays(trianglesCulledSwap, 2 * (vertexMaximum + vertexSizingStep) \ 3)
                        InitializeTrianglesDraw(trianglesDraw, 2 * (vertexMaximum + vertexSizingStep) \ 3)
                        ReDim sortZ1(2 * (vertexMaximum + vertexSizingStep) \ 3)
                        ReDim sortIndex1(2 * (vertexMaximum + vertexSizingStep) \ 3)
                        ReDim sortZ2(2 * (vertexMaximum + vertexSizingStep) \ 3)
                        ReDim sortIndex2(2 * (vertexMaximum + vertexSizingStep) \ 3)
                        vertexMaximum += vertexSizingStep
                    Loop
                    'начинаем производить трансформации вершин, перенося их в общий буфер
                    'для ускорения развернем матрицу в отдельные переменные
                    Dim m11, m12, m13, m21, m22, m23, m31, m32, m33, m14, m24, m34 As Single
                    m11 = matrixFinal.matrix(0, 0)
                    m12 = matrixFinal.matrix(0, 1)
                    m13 = matrixFinal.matrix(0, 2)
                    m21 = matrixFinal.matrix(1, 0)
                    m22 = matrixFinal.matrix(1, 1)
                    m23 = matrixFinal.matrix(1, 2)
                    m31 = matrixFinal.matrix(2, 0)
                    m32 = matrixFinal.matrix(2, 1)
                    m33 = matrixFinal.matrix(2, 2)
                    m14 = matrixFinal.matrix(0, 3)
                    m24 = matrixFinal.matrix(1, 3)
                    m34 = matrixFinal.matrix(2, 3)
                    'развернем также исходный вектор
                    Dim px, py, pz As Single
                    Dim nx, ny, nz As Single
                    Dim triangleNum As Integer
                    'просчитаем точки
                    For i = 0 To .count - 1
                        vertexPointer += 1
                        px = .pX(i)
                        py = .pY(i)
                        pz = .pZ(i)
                        nx = .nX(i)
                        ny = .nY(i)
                        nz = .nZ(i)
                        vertexes.pX(vertexPointer) = px * m11 + py * m12 + pz * m13 + m14
                        vertexes.pY(vertexPointer) = px * m21 + py * m22 + pz * m23 + m24
                        vertexes.pZ(vertexPointer) = px * m31 + py * m32 + pz * m33 + m34
                        vertexes.nX(vertexPointer) = (nx * m11 + ny * m12 + nz * m13) / scale
                        vertexes.nY(vertexPointer) = (nx * m21 + ny * m22 + nz * m23) / scale
                        vertexes.nZ(vertexPointer) = (nx * m31 + ny * m32 + nz * m33) / scale
                        vertexes.material(vertexPointer) = .material(i)
                        vertexes.materialUID(vertexPointer) = .materialUID(i)
                        vertexes.tU(vertexPointer) = .tU(i)
                        vertexes.tV(vertexPointer) = .tV(i)
                        If i Mod 3 = 0 Then
                            triangleNum = i \ 3
                            trianglesPointer += 1
                            TriangleCopy(modelTraingles, triangles, triangleNum, trianglesPointer)
                            nx = modelTraingles.nX(triangleNum)
                            ny = modelTraingles.nY(triangleNum)
                            nz = modelTraingles.nZ(triangleNum)
                            triangles.nX(trianglesPointer) = (nx * m11 + ny * m12 + nz * m13) / scale
                            triangles.nY(trianglesPointer) = (nx * m21 + ny * m22 + nz * m23) / scale
                            triangles.nZ(trianglesPointer) = (nx * m31 + ny * m32 + nz * m33) / scale
                            tempRenderParams = defaults
                            If Not renderParameters.renderer = RenderMode.byRender Then tempRenderParams.renderer = renderParameters.renderer
                            If Not renderParameters.lighting = LightingMode.byRender Then tempRenderParams.lighting = renderParameters.lighting
                            If Not renderParameters.mipMap = MipMapMode.byRender Then tempRenderParams.mipMap = renderParameters.mipMap
                            If Not renderParameters.texturing = TexturingMode.byRender Then tempRenderParams.texturing = renderParameters.texturing
                            If Not renderParameters.normals = NormalsInterpolationMode.byRender Then tempRenderParams.normals = renderParameters.normals
                            If Not renderParameters.culling = NonfacialCulling.byRender Then tempRenderParams.culling = renderParameters.culling
                            tempRenderParams.special = renderParameters.special
                            tempRenderParams.shadowsOn = renderParameters.shadowsOn
                            tempRenderParams.shadowedBy = renderParameters.shadowedBy
                            tempRenderParams.isSkybox = renderParameters.isSkybox
                            triangles.renderSettings(trianglesPointer) = tempRenderParams
                        End If
                    Next
                End With
                vertexTop = vertexPointer
                'копируем треугольники
                'For i = 0 To modelTraingles.count - 1

                'TriangleCopy(modelTraingles, triangles, i, trianglesPointer)
            Next
            trianglesTop = trianglesPointer

            '  vertexes.pX(0) = 0
            '  vertexes.pY(0) = 100
            '  vertexes.pZ(0) = -102
            '  vertexes.pX(1) = 200
            '  vertexes.pY(1) = 150
            '  vertexes.pZ(1) = -105

            '   vertexes.pX(2) = 150
            '   vertexes.pY(2) = -200
            '   vertexes.pZ(2) = -98
            ''   triangles.nX(0) = 0
            ''   triangles.nY(0) = 0
            '    triangles.nZ(0) = 1
            ' vertexTop = 2
            ' trianglesTop = 1
        End Sub
        ''' <summary>
        ''' Произвести проецирование для всех точек модели
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Projection()
            'Const dist As Single = 1000
            Dim i As Integer
            Dim px(), py(), pz() As Single
            Dim w() As Single
            Dim w1, w2, w3 As Single
            Dim sx1, sx2, sx3, sy1, sy2, sy3 As Single
            Dim hidden() As Boolean
            Dim size As Single
            Dim sideA, sideB, sideC As Single
            Dim halfPerimetr As Single
            Dim minZ As Single, minZi As Integer
            px = vertexesCulled.pX
            py = vertexesCulled.pY
            pz = vertexesCulled.pZ
            w = vertexesDraw.w
            hidden = vertexesDraw.hidden
            Dim xAdd, yAdd As Single
            xAdd = myParent.width \ 2
            yAdd = myParent.height \ 2
            Dim vertexNum As Integer
            Dim tmp As Single
            For i = 0 To vertexCulledTop \ 3
                vertexNum = i * 3
                minZ = pz(vertexNum)
                w1 = 1 / (minZ + cameraDist)
                sx1 = xAdd + px(vertexNum) * w1 * cameraDist '+ py(vertexNum) * 0.5
                sy1 = yAdd - py(vertexNum) * w1 * cameraDist '+ py(vertexNum) * 0.5
                w2 = 1 / (pz(vertexNum + 1) + cameraDist)
                If pz(vertexNum + 1) < minZ Then minZ = pz(vertexNum + 1)
                sx2 = xAdd + px(vertexNum + 1) * w2 * cameraDist '+ py(vertexNum) * 0.5
                sy2 = yAdd - py(vertexNum + 1) * w2 * cameraDist '+ py(vertexNum) * 0.5
                w3 = 1 / (pz(vertexNum + 2) + cameraDist)
                If pz(vertexNum + 2) < minZ Then minZ = pz(vertexNum + 2)
                sx3 = xAdd + px(vertexNum + 2) * w3 * cameraDist '+ py(vertexNum) * 0.5
                sy3 = yAdd - py(vertexNum + 2) * w3 * cameraDist '+ py(vertexNum) * 0.5
                'считаем характеристику, показывающую размер треугольника
                sideA = Math.Sqrt((sx1 - sx2) * (sx1 - sx2) + (sy1 - sy2) * (sy1 - sy2))
                sideB = Math.Sqrt((sx3 - sx2) * (sx3 - sx2) + (sy3 - sy2) * (sy3 - sy2))
                sideC = Math.Sqrt((sx1 - sx3) * (sx1 - sx3) + (sy1 - sy3) * (sy1 - sy3))
                halfPerimetr = (sideA + sideB + sideC) / 2
                size = Math.Sqrt(halfPerimetr * (halfPerimetr - sideA) * (halfPerimetr - sideB) * (halfPerimetr - sideC))
                'записываем значения
                vertexesDraw.scrX(vertexNum) = sx1
                vertexesDraw.scrX(vertexNum + 1) = sx2
                vertexesDraw.scrX(vertexNum + 2) = sx3
                vertexesDraw.scrY(vertexNum) = sy1
                vertexesDraw.scrY(vertexNum + 1) = sy2
                vertexesDraw.scrY(vertexNum + 2) = sy3
                w(vertexNum) = w1
                w(vertexNum + 1) = w2
                w(vertexNum + 2) = w3
                trianglesDraw.screenSize(i) = size
                'здесь мы сортируем вершины
                'есть или нет инлайнинг на Марсе - современной науке неведомо
                If sx2 < sx1 Then tmp = sx2 : sx2 = sx1 : sx1 = tmp
                If sx3 < sx2 Then tmp = sx3 : sx3 = sx2 : sx2 = tmp
                If sx2 < sx1 Then tmp = sx2 : sx2 = sx1 : sx1 = tmp
                If sy2 < sy1 Then tmp = sy2 : sy2 = sy1 : sy1 = tmp
                If sy3 < sy2 Then tmp = sy3 : sy3 = sy2 : sy2 = tmp
                If sy2 < sy1 Then tmp = sy2 : sy2 = sy1 : sy1 = tmp
                If w2 < w1 Then tmp = w2 : w2 = w1 : w1 = tmp
                If w3 < w2 Then tmp = w3 : w3 = w2 : w2 = tmp
                If w2 < w1 Then tmp = w2 : w2 = w1 : w1 = tmp
                trianglesDraw.minX(i) = sx1
                trianglesDraw.maxX(i) = sx3
                trianglesDraw.minY(i) = sy1
                trianglesDraw.maxY(i) = sy3
                trianglesDraw.minW(i) = w1
                trianglesDraw.maxW(i) = w3
                minZi = CInt(minZ + cameraDist)
                'сразу вносим значения в массивы для сортировки по Z
                sortZ1(i) = minZi
                ' sortZ1(i) = (pz(vertexNum + 2) + pz(vertexNum + 1) + pz(vertexNum + 0)) / 3
                sortIndex1(i) = i
            Next
        End Sub
        ''' <summary>
        ''' Сортирует полигоны с уменьшением Z
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SortTriangles()
            'сортируем массивы методом побитной сортировки
            Dim bitMask As Integer = 1
            Dim pointer As Integer
            Dim i, j As Integer
            Dim sortSourceZ, sortSourceIndex As Integer()
            Dim sortTargetZ, sortTargetIndex As Integer()
            Dim tmpPtr As Integer()
            sortSourceZ = sortZ1
            sortTargetZ = sortZ2
            sortSourceIndex = sortIndex1
            sortTargetIndex = sortIndex2
            For j = 0 To 30
                pointer = 0
                For i = 0 To trianglesCulledTop - 1
                    If (sortSourceZ(i) And bitMask) = 0 Then
                        sortTargetZ(pointer) = sortSourceZ(i)
                        sortTargetIndex(pointer) = sortSourceIndex(i)
                        pointer += 1
                    End If
                Next i
                For i = 0 To trianglesCulledTop - 1
                    If (sortSourceZ(i) And bitMask) <> 0 Then
                        sortTargetZ(pointer) = sortSourceZ(i)
                        sortTargetIndex(pointer) = sortSourceIndex(i)
                        pointer += 1
                    End If
                Next i
                tmpPtr = sortSourceZ
                sortSourceZ = sortTargetZ
                sortTargetZ = tmpPtr
                tmpPtr = sortSourceIndex
                sortSourceIndex = sortTargetIndex
                sortTargetIndex = tmpPtr
                bitMask = bitMask << 1
            Next j
            'теперь перекладываем треугольники и вершины в соотв. порядке

        End Sub
        ''' <summary>
        ''' Отрисовать модель линиями
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RenderLines()
            Dim i As Integer
            Dim sx(), sy() As Single
            sx = vertexesDraw.scrX
            sy = vertexesDraw.scrY
            For i = 0 To vertexCulledTop Step 3
                If vertexesDraw.hidden(i) = False Then
                    myParent.DirectDraw.DrawLine(sx(i + 0), sy(i + 0), sx(i + 1), sy(i + 1), Color.White)
                    myParent.DirectDraw.DrawLine(sx(i + 1), sy(i + 1), sx(i + 2), sy(i + 2), Color.White)
                    myParent.DirectDraw.DrawLine(sx(i + 2), sy(i + 2), sx(i + 0), sy(i + 0), Color.White)
                End If
            Next
        End Sub
        ''' <summary>
        ''' Отрисовать модель с текстурированием
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DrawSky()
            Dim i As Integer
            For i = 0 To myParent.drawBuffer.pixels2.GetUpperBound(0) - 3 Step 3
                myParent.drawBuffer.pixels2(i + 2) = skyR
                myParent.drawBuffer.pixels2(i + 1) = skyG
                myParent.drawBuffer.pixels2(i + 0) = skyB
            Next
        End Sub
        Private Sub RenderTexture()
            'обнуляем W-буфер
            'Array.Clear(wBufferHigh, 0, wBufferHigh.GetUpperBound(0))
            Array.Clear(wBuffer, 0, wBuffer.GetUpperBound(0))
            '  Static maxf As Integer
            '  Dim maxi As Integer
            Dim i As Integer
            Dim num As Integer
            '  maxf += 1
            '  maxi = maxf
            '  If maxi > trianglesCulledTop - 1 Then maxi = trianglesCulledTop - 1
            If settings.sortTriangles Then
                ' For i = 0 To maxi
                For i = 0 To trianglesCulledTop - 1
                    num = sortIndex1(i)
                    If trianglesCulled.renderSettings(num).renderer = RenderMode.fast Then
                        DrawTriangleSimple(num)
                        'DrawTriangleVerySimple(num)
                    Else
                        If trianglesCulled.renderSettings(i).renderer = RenderMode.special Then
                            DrawTriangleSpecial(num)
                        Else
                            DrawTriangle(num)
                        End If
                    End If
                Next i
            Else
                For i = 0 To vertexCulledTop \ 3 ' - 1
                    If trianglesCulled.renderSettings(i).renderer = RenderMode.fast Then
                        DrawTriangleSimple(i)
                        'DrawTriangleVerySimple(i)
                    Else
                        If trianglesCulled.renderSettings(i).renderer = RenderMode.special Then
                            DrawTriangleSpecial(i)
                        Else
                            DrawTriangle(i)
                        End If
                    End If
                Next i
            End If
        End Sub
        Private Sub DrawTriangleSpecial(ByVal triangleNum As Integer)
            Dim startVertex As Integer = triangleNum * 3
            Dim shadowedBy As Integer
            Dim shadowPixels As Byte()
            Dim shadowWidth, shadowHeight, shadowScale As Integer
            Dim shadowDX, shadowDY As Integer
            Dim dropTriangle As Boolean
            Dim settings As RenderParameters = trianglesCulled.renderSettings(triangleNum)
            Dim special As SpecialMode = settings.special
            Dim useLightning As Boolean
            If settings.lighting = LightingMode.full Then useLightning = True
            'если треугольник закрыт, все остальное нам нахрен не нужно ;)
            'dropTriangle = True
            If Not dropTriangle Then
                infoTrianglesDrawed += 1
                Dim x1, x2, x3, y1, y2, y3 As Integer
                Dim w1, w2, w3 As Integer
                'текстурные координаты
                Dim u1, v1, u2, v2, u3, v3 As Single
                'экранные координаты
                Dim width, height As Integer
                width = myParent.width
                height = myParent.height
                'координаты вершин (экранные)
                'если материал изменился, перезагружаем текстуру
                If vertexesCulled.materialUID(startVertex) <> materialUID Then
                    ChangeTexture(vertexesCulled.material(startVertex))
                End If
                'MipMap
                Dim mipMap As Integer = 0
                Dim textureSizeYLocal, textureSizeXLocal As Integer
                If settings.mipMap = MipMapMode.mipMapOn Then
                    Dim textureSize, scrSize As Single
                    textureSize = trianglesCulled.textureSize(triangleNum)
                    If textureMipMap > 0 And textureSize > 0 Then
                        scrSize = trianglesDraw.screenSize(triangleNum)
                        textureSize *= sqrtTable(textureSizeY * textureSizeX) >> 4
                        mipMap = Math.Floor(Math.Log(textureSize / scrSize) / Math.Log(2)) + 5
                        If mipMap < 0 Then mipMap = 0
                        If mipMap > textureMipMap - 1 Then mipMap = textureMipMap - 1
                        textureSizeXLocal = texturePixels(mipMap).Width
                        textureSizeYLocal = texturePixels(mipMap).Height
                    End If
                Else
                    textureSizeXLocal = textureSizeX
                    textureSizeYLocal = textureSizeY
                End If
                Dim vertex1, vertex2, vertex3 As Integer
                vertex1 = 0 : vertex2 = 1 : vertex3 = 2
                y1 = vertexesDraw.scrY(startVertex + 0)
                y2 = vertexesDraw.scrY(startVertex + 1)
                y3 = vertexesDraw.scrY(startVertex + 2)
                'сортируем вершины
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                If y2 > y3 Then
                    Swap(y2, y3)
                    Swap(vertex2, vertex3)
                End If
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                shadowedBy = settings.shadowedBy
                If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                    With shadowPlanes(shadowedBy - 1)
                        shadowDX = .x1 \ .scale
                        shadowDY = .y1 \ .scale
                        shadowHeight = .map.Width
                        shadowWidth = .map.Height
                        shadowPixels = .map.pixels2
                        shadowScale = .scale
                        ' shadowScale = 1
                    End With
                Else
                    shadowPixels = Nothing
                End If
                'выборка данных
                x1 = vertexesDraw.scrX(startVertex + vertex1)
                x2 = vertexesDraw.scrX(startVertex + vertex2)
                x3 = vertexesDraw.scrX(startVertex + vertex3)
                u1 = vertexesCulled.tU(startVertex + vertex1) * textureSizeXLocal
                v1 = vertexesCulled.tV(startVertex + vertex1) * textureSizeYLocal
                u2 = vertexesCulled.tU(startVertex + vertex2) * textureSizeXLocal
                v2 = vertexesCulled.tV(startVertex + vertex2) * textureSizeYLocal
                u3 = vertexesCulled.tU(startVertex + vertex3) * textureSizeXLocal
                v3 = vertexesCulled.tV(startVertex + vertex3) * textureSizeYLocal
                w1 = vertexesDraw.w(startVertex + vertex1) * 4096 * fixed
                w2 = vertexesDraw.w(startVertex + vertex2) * 4096 * fixed
                w3 = vertexesDraw.w(startVertex + vertex3) * 4096 * fixed
                'текстурные координаты, деленные на w
                Dim pvz1, pvz2, pvz3, puz1, puz2, puz3 As Single
                puz1 = u1 * w1 : puz2 = u2 * w2 : puz3 = u3 * w3
                pvz1 = v1 * w1 : pvz2 = v2 * w2 : pvz3 = v3 * w3
                'реальные координаты
                Dim rx1, rx2, rx3, ry1, ry2, ry3, rz1, rz2, rz3 As Integer
                Dim nx1, nx2, nx3, ny1, ny2, ny3, nz1, nz2, nz3 As Integer
                Dim lrx1, lrx2, lry1, lry2, lrz1, lrz2 As Integer
                Dim lnx1, lnx2, lny1, lny2, lnz1, lnz2 As Integer
                Dim dlrx1, dlrx2, rxs, rx As Integer
                Dim dlry1, dlry2, rys, ry As Integer
                Dim dlrz1, dlrz2, rzs, rz As Integer
                Dim dlnx1, dlnx2, nxs, nx As Integer
                Dim dlny1, dlny2, nys, ny As Integer
                Dim dlnz1, dlnz2, nzs, nz As Integer
                '   If y2 = y3 Then y2 += 3
                '   If y2 = y1 Then y2 -= 3
                ' ; y1 -= 3
                '  y3 += 3
                If x1 < 1 Then x1 = 1
                If x2 < 1 Then x2 = 1
                If x3 < 1 Then x3 = 1
                If y1 < 1 Then y1 = 1
                If y2 < 1 Then y2 = 1
                If y3 < 1 Then y3 = 1
                If x1 > width - 2 Then x1 = width - 2
                If x2 > width - 2 Then x2 = width - 2
                If x3 > width - 2 Then x3 = width - 2
                If y1 > height - 2 Then y1 = height - 2
                If y2 > height - 2 Then y2 = height - 2
                If y3 > height - 2 Then y3 = height - 2
                If x1 > x2 Then
                    If x1 > x3 Then
                        x1 += 1
                    Else
                        x3 += 1
                    End If
                Else
                    If x2 > x3 Then
                        x2 += 1
                    Else
                        x3 += 1
                    End If
                End If
                If x1 < x2 Then
                    If x1 < x3 Then
                        x1 -= 1
                    Else
                        x3 -= 1
                    End If
                Else
                    If x2 < x3 Then
                        x2 -= 1
                    Else
                        x3 -= 1
                    End If
                End If
                'If y1 > y2 Or y2 > y3 Then Stop
                ' Dim color As Color = Drawing.Color.Gray
                Dim pixels() As Byte = myParent.drawBuffer.pixels2
                Dim texturePixelsMipMap() As Byte = texturePixels(mipMap).pixels2
                'экранные
                Dim y As Integer
                'начало, конец, шаг строки точек
                Dim xStart As Integer
                Dim xEnd As Integer
                Dim xSpanEnd As Integer
                'края треугольника - х
                Dim dlx1, dlx2 As Single
                Dim lx1, lx2 As Single
                'W на краях треугольника
                Dim dlw1, dlw2 As Integer
                Dim lw1, lw2, wSpan, wSpanStep As Integer
                'U,V на краях треугольника
                Dim su1, su2, sv1, sv2 As Single
                Dim su, sv, us, vs, w, wStep As Integer
                Dim iu, iv As Integer
                'U\Z, V\Z
                Dim dluz1, dluz2, dlvz1, dlvz2 As Single
                Dim luz1, luz2, lvz1, lvz2 As Single
                Dim uz, vz As Single
                Dim uzs, vzs As Single
                Dim dlx2lx1 As Single
                'Y3-Y1,Y2-Y1
                Dim dy31, dy21, dy32 As Single
                dy31 = y3 - y1
                dy21 = y2 - y1
                dy32 = y3 - y2
                'остальное
                Dim widthOffset As Integer
                Dim widthOffset3 As Integer
                'освещение
                'для репроекции
                'хыыы! опять прет !
                Dim xAdd, yAdd As Integer
                Dim length1 As Integer
                Dim length2 As Single
                Dim dst_ww As Single
                xAdd = myParent.width \ 2
                yAdd = myParent.height \ 2
                'вектор на источник света в точке
                'rz = 65536 * 4096 \ w - cameraDist
                ' rx = (xStart - xAdd) * dst_w
                'ry = -(y - yAdd) * dst_w
                dst_ww = (65536 * 4096) / (cameraDist * w1)
                rx1 = (lighterPoint.x - (x1 - xAdd) * dst_ww)
                ry1 = (lighterPoint.y - (yAdd - y1) * dst_ww)
                rz1 = (lighterPoint.z - (65536 * 4096 \ w1 - cameraDist))
                dst_ww = (65536 * 4096) / (cameraDist * w2)
                rx2 = (lighterPoint.x - (x2 - xAdd) * dst_ww)
                ry2 = (lighterPoint.y - (yAdd - y2) * dst_ww)
                rz2 = (lighterPoint.z - (65536 * 4096 \ w2 - cameraDist))
                dst_ww = (65536 * 4096) / (cameraDist * w3)
                rx3 = (lighterPoint.x - (x3 - xAdd) * dst_ww)
                ry3 = (lighterPoint.y - (yAdd - y3) * dst_ww)
                rz3 = (lighterPoint.z - (65536 * 4096 \ w3 - cameraDist))

                length1 = rx1 * rx1 + ry1 * ry1 + rz1 * rz1
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx1 *= length2 : ry1 *= length2 : rz1 *= length2

                length1 = rx2 * rx2 + ry2 * ry2 + rz2 * rz2
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx2 *= length2 : ry2 *= length2 : rz2 *= length2
                length1 = rx3 * rx3 + ry3 * ry3 + rz3 * rz3
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx3 *= length2 : ry3 *= length2 : rz3 *= length2

                'нормали для поверхности, нужны для освещений
                If Not settings.lighting = LightingMode.none Then
                    If settings.normals = NormalsInterpolationMode.oneForTriangle Then
                        nx1 = trianglesCulled.nX(triangleNum) * 1024
                        ny1 = trianglesCulled.nY(triangleNum) * 1024
                        nz1 = trianglesCulled.nZ(triangleNum) * 1024
                        ';  nx1 = 0
                        '   ny1 = 1024
                        '  nz1 = 0
                        nx2 = nx1 : nx3 = nx1
                        ny2 = ny1 : ny3 = ny1
                        nz2 = nz1 : nz3 = nz1
                    Else
                        nx1 = vertexesCulled.nX(startVertex + vertex1) * 1024
                        nx2 = vertexesCulled.nX(startVertex + vertex2) * 1024
                        nx3 = vertexesCulled.nX(startVertex + vertex3) * 1024
                        ny1 = vertexesCulled.nY(startVertex + vertex1) * 1024
                        ny2 = vertexesCulled.nY(startVertex + vertex2) * 1024
                        ny3 = vertexesCulled.nY(startVertex + vertex3) * 1024
                        nz1 = vertexesCulled.nZ(startVertex + vertex1) * 1024
                        nz2 = vertexesCulled.nZ(startVertex + vertex2) * 1024
                        nz3 = vertexesCulled.nZ(startVertex + vertex3) * 1024
                    End If
                Else

                End If
                Dim r, g, b As Integer
                ' Dim bright, brightStep As Integer
                Dim brightR, brightG, brightB As Integer
                Dim brightR1, brightG1, brightB1 As Integer
                Dim brightR2, brightG2, brightB2 As Integer
                Dim brightRstep, brightGstep, brightBstep As Integer
                Dim revDrawSpanSize As Single = 1 / drawSpanSize
                Dim revDrawSpanSizeCurrent As Single = revDrawSpanSize
                Dim shX, shY As Integer
                Dim shadowResult As Integer
                If y3 > y1 Then
                    'общие параметры для обоих частей
                    If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                        'dlu2 = (u3 - u1) / dy31 : dlv2 = (v3 - v1) / dy31
                        dlx2 = (x3 - x1) / dy31 : dlw2 = (w3 - w1) / dy31
                        dlrx2 = (rx3 - rx1) / dy31
                        dlry2 = (ry3 - ry1) / dy31
                        dlrz2 = (rz3 - rz1) / dy31
                        lrx2 = rx1 : lry2 = ry1 : lrz2 = rz1
                        dlnx2 = (nx3 - nx1) / dy31
                        dlny2 = (ny3 - ny1) / dy31
                        dlnz2 = (nz3 - nz1) / dy31
                        lnx2 = nx1 : lny2 = ny1 : lnz2 = nz1
                        lx2 = x1 : lw2 = w1
                        dluz2 = (puz3 - puz1) / dy31
                        dlvz2 = (pvz3 - pvz1) / dy31
                        luz2 = puz1 : lvz2 = pvz1
                    Else
                        dlx1 = (x3 - x1) / dy31 : dlw1 = (w3 - w1) / dy31
                        'dlu1 = (u3 - u1) / dy31 : dlv1 = (v3 - v1) / dy31
                        dlrx1 = (rx3 - rx1) / dy31
                        dlry1 = (ry3 - ry1) / dy31
                        dlrz1 = (rz3 - rz1) / dy31
                        lrx1 = rx1 : lry1 = ry1 : lrz1 = rz1
                        dlnx1 = (nx3 - nx1) / dy31
                        dlny1 = (ny3 - ny1) / dy31
                        dlnz1 = (nz3 - nz1) / dy31
                        lnx1 = nx1 : lny1 = ny1 : lnz1 = nz1
                        lx1 = x1 : lw1 = w1
                        dluz1 = (puz3 - puz1) / dy31
                        dlvz1 = (pvz3 - pvz1) / dy31
                        luz1 = puz1 : lvz1 = pvz1
                    End If
                    'рисуем верхнюю часть треугольника
                    If dy21 > 0 Then
                        If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                            'dlu1 = (u2 - u1) / dy21 : dlv1 = (v2 - v1) / dy21 
                            dlx1 = (x2 - x1) / dy21 : dlw1 = (w2 - w1) / dy21
                            dlrx1 = (rx2 - rx1) / dy21
                            dlry1 = (ry2 - ry1) / dy21
                            dlrz1 = (rz2 - rz1) / dy21
                            lrx1 = rx1 : lry1 = ry1 : lrz1 = rz1
                            dlnx1 = (nx2 - nx1) / dy21
                            dlny1 = (ny2 - ny1) / dy21
                            dlnz1 = (nz2 - nz1) / dy21
                            lnx1 = nx1 : lny1 = ny1 : lnz1 = nz1
                            lx1 = x1 : lw1 = w1
                            dluz1 = (puz2 - puz1) / dy21
                            dlvz1 = (pvz2 - pvz1) / dy21
                            luz1 = puz1 : lvz1 = pvz1
                        Else
                            dlx2 = (x2 - x1) / dy21 : dlw2 = (w2 - w1) / dy21
                            'dlu2 = (u2 - u1) / dy21 : dlv2 = (v2 - v1) / dy21
                            dlrx2 = (rx2 - rx1) / dy21
                            dlry2 = (ry2 - ry1) / dy21
                            dlrz2 = (rz2 - rz1) / dy21
                            lrx2 = rx1 : lry2 = ry1 : lrz2 = rz1
                            dlnx2 = (nx2 - nx1) / dy21
                            dlny2 = (ny2 - ny1) / dy21
                            dlnz2 = (nz2 - nz1) / dy21
                            lnx2 = nx1 : lny2 = ny1 : lnz2 = nz1
                            lx2 = x1 : lw2 = w1
                            dluz2 = (puz2 - puz1) / dy21
                            dlvz2 = (pvz2 - pvz1) / dy21
                            luz2 = puz1 : lvz2 = pvz1
                        End If
                    End If
                    For y = y1 To y3 - 1
                        'проверяем. не перешли ли через границу Y2
                        If y = y2 Then
                            'рисуем нижнюю часть треугольника
                            If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                                dlx1 = (x3 - x2) / dy32 : dlw1 = (w3 - w2) / dy32
                                lx1 = x2 : lw1 = w2
                                dlrx1 = (rx3 - rx2) / dy32
                                dlry1 = (ry3 - ry2) / dy32
                                dlrz1 = (rz3 - rz2) / dy32
                                lrx1 = rx2 : lry1 = ry2 : lrz1 = rz2
                                dlnx1 = (nx3 - nx2) / dy32
                                dlny1 = (ny3 - ny2) / dy32
                                dlnz1 = (nz3 - nz2) / dy32
                                lnx1 = nx2 : lny1 = ny2 : lnz1 = nz2
                                dluz1 = (puz3 - puz2) / dy32
                                dlvz1 = (pvz3 - pvz2) / dy32
                                luz1 = puz2 : lvz1 = pvz2
                            Else
                                dlx2 = (x3 - x2) / dy32 : dlw2 = (w3 - w2) / dy32
                                lx2 = x2 : lw2 = w2
                                dlrx2 = (rx3 - rx2) / dy32
                                dlry2 = (ry3 - ry2) / dy32
                                dlrz2 = (rz3 - rz2) / dy32
                                lrx2 = rx2 : lry2 = ry2 : lrz2 = rz2
                                dlnx2 = (nx3 - nx2) / dy32
                                dlny2 = (ny3 - ny2) / dy32
                                dlnz2 = (nz3 - nz2) / dy32
                                lnx2 = nx2 : lny2 = ny2 : lnz2 = nz2
                                dluz2 = (puz3 - puz2) / dy32
                                dlvz2 = (pvz3 - pvz2) / dy32
                                luz2 = puz2 : lvz2 = pvz2
                            End If
                        End If
                        If lx2 - lx1 < 1 Then GoTo end_cycle
                        'ускоряющие замены
                        widthOffset = y * width
                        widthOffset3 = widthOffset * 3
                        ' widthOffset3Right = (y + 1) * width * 3
                        dlx2lx1 = 1 / (lx2 - lx1)
                        'начало и конец
                        xStart = lx1
                        xEnd = lx2
                        'шаги W - одиночный и через отрезок
                        wStep = (lw2 - lw1) * dlx2lx1
                        wSpanStep = wStep * drawSpanSize
                        'начальные значения для первого отрезка
                        w = lw1 : wSpan = lw1
                        uz = luz1 : vz = lvz1
                        uzs = (luz2 - luz1) * drawSpanSize * dlx2lx1
                        vzs = (lvz2 - lvz1) * drawSpanSize * dlx2lx1
                        su1 = uz / lw1
                        sv1 = vz / lw1
                        rx = lrx1
                        ry = lry1
                        rz = lrz1
                        rxs = (lrx2 - lrx1) * drawSpanSize * dlx2lx1
                        rys = (lry2 - lry1) * drawSpanSize * dlx2lx1
                        rzs = (lrz2 - lrz1) * drawSpanSize * dlx2lx1
                        nx = lnx1
                        ny = lny1
                        nz = lnz1
                        nxs = (lnx2 - lnx1) * drawSpanSize * dlx2lx1
                        nys = (lny2 - lny1) * drawSpanSize * dlx2lx1
                        nzs = (lnz2 - lnz1) * drawSpanSize * dlx2lx1
                        revDrawSpanSizeCurrent = revDrawSpanSize

                        Dim koeff1, koeff2 As Integer
                        Dim brightDiffuse, brightSpecular2 As Integer
                        Dim brightSpecular1 As Single
                        Dim dst_w As Single
                        Dim revX, revY, revZ As Integer
                        Dim camX, camY, camZ As Integer
                        Dim rrx, rry, rrz, kk As Integer
                        If useLightning Then
                            'освещение для первого отрезка - отражение и рассеивание
                            'вектор на камеру
                            camX = cameraVectorsX(widthOffset + xStart)
                            camY = cameraVectorsY(widthOffset + xStart)
                            camZ = cameraVectorsZ(widthOffset + xStart)
                            'вектор отражения
                            'Dim llll = Math.Sqrt(nx * nx + ny * ny + nz * nz)
                            'If llll > 1500 Then Stop
                            kk = (nx * rx + ny * ry + nz * rz) >> 10 'k=n*lightv
                            revX = rx - ((2 * kk * nx) >> 10)
                            revY = ry - ((2 * kk * ny) >> 10)
                            revZ = rz - ((2 * kk * nz) >> 10)
                            brightSpecular1 = CSng(-((revX * camX + revY * camY + revZ * camZ) >> 12) - 150) / 16
                            'brightSpecular1 *= 5
                            'упрощенная следующая из первой формула
                            If brightSpecular1 > 0 Then
                                brightSpecular2 = brightSpecular1 * brightSpecular1 * brightSpecular1 '* brightSpecular1
                            Else
                                brightSpecular2 = 0
                            End If
                            brightDiffuse = -(rx * nx + ry * ny + rz * nz) >> 12
                            '  If brightDiffuse < 0 Then brightSpecular2 = 0
                            If brightDiffuse < 0 Then brightDiffuse = 0
                            '  brightDiffuse = 0
                            'освещение для первого отрезка (мощность)
                            dst_w = (65536 * 4096) / (cameraDist * w)
                            rrz = 65536 * 4096 \ w - cameraDist
                            rrx = (xStart - xAdd) * dst_w
                            rry = -(y - yAdd) * dst_w
                            koeff1 = lighterPoint.intense
                            koeff1 -= lighterPoint.attenutionA * ((lighterPoint.x - rrx) * (lighterPoint.x - rrx) + (lighterPoint.y - rry) * (lighterPoint.y - rry) + (lighterPoint.z - rrz) * (lighterPoint.z - rrz))
                            If koeff1 < 0 Then koeff1 = 0
                            'все освещение для первого отрезка
                            '     brightR1 = brightDiffSpan << 8 '* koeff1
                            '     brightG1 = brightDiffSpan << 8 '* koeff1
                            '     brightB1 = brightDiffSpan << 8 '* koeff1
                            '   brightR1 = brightDiffuse << 8  '* koeff1+100000
                            '  brightG1 = brightDiffuse << 8  '* koeff1
                            '     brightB1 = brightDiffuse << 8   '* koeff1
                            If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                                shX = (rrx \ shadowScale - shadowDX)
                                shY = (rrz \ shadowScale - shadowDY)
                                If shX < 0 OrElse shY < 0 OrElse shX > shadowWidth - 1 OrElse shY > shadowHeight - 1 Then
                                Else
                                    shadowResult = 256 - shadowPixels((shY * shadowWidth + shX) * 3)
                                    koeff1 = (koeff1 * shadowResult) >> 8
                                End If
                            End If
                            brightR1 = (((brightDiffuse + brightSpecular2) * lighterPoint.r) >> 8) * koeff1 + (ambientR << 8)
                            brightG1 = (((brightDiffuse + brightSpecular2) * lighterPoint.g) >> 8) * koeff1 + (ambientG << 8)
                            brightB1 = (((brightDiffuse + brightSpecular2) * lighterPoint.b) >> 8) * koeff1 + (ambientB << 8)
                        End If
                        'рисуем отрезки

                        While xStart < xEnd
                            If xEnd - xStart <= drawSpanSize Then
                                wSpan = lw2
                                uz = luz2
                                vz = lvz2
                                rx = lrx2
                                ry = lry2
                                rz = lrz2
                                nx = lnx2
                                ny = lny2
                                nz = lnz2
                                revDrawSpanSizeCurrent = 1 / (xEnd - xStart)
                            Else
                                wSpan += wSpanStep
                                uz += uzs
                                vz += vzs
                                rx += rxs
                                ry += rys
                                rz += rzs
                                nx += nxs
                                ny += nys
                                nz += nzs
                            End If
                            'считаем значения на конце отрезка
                            su2 = (uz) / wSpan
                            sv2 = (vz) / wSpan
                            us = (su2 - su1) * revDrawSpanSizeCurrent * fixed
                            vs = (sv2 - sv1) * revDrawSpanSizeCurrent * fixed
                            su = su1 * fixed
                            sv = sv1 * fixed
                            xSpanEnd = xStart + drawSpanSize
                            If xSpanEnd > xEnd Then xSpanEnd = xEnd
                            'cчитаем освещение на конце отрезка 
                            If useLightning Then
                                'вектор на камеру
                                camX = cameraVectorsX(widthOffset + xSpanEnd)
                                camY = cameraVectorsY(widthOffset + xSpanEnd)
                                camZ = cameraVectorsZ(widthOffset + xSpanEnd)
                                ' camX = 0
                                ' camY = 0
                                ' camZ = 1024
                                'вектор отражения
                                kk = (nx * rx + ny * ry + nz * rz) >> 10 'k=n*lightv
                                revX = rx - ((2 * kk * nx) >> 10)
                                revY = ry - ((2 * kk * ny) >> 10)
                                revZ = rz - ((2 * kk * nz) >> 10)
                                brightSpecular1 = CSng(-((revX * camX + revY * camY + revZ * camZ) >> 12) - 150) / 16

                                'упрощенная следующая из первой формула
                                If brightSpecular1 > 0 Then
                                    brightSpecular2 = brightSpecular1 * brightSpecular1 * brightSpecular1 ' * brightSpecular1
                                Else
                                    brightSpecular2 = 0
                                End If
                                brightDiffuse = -(rx * nx + ry * ny + rz * nz) >> 12
                                '  If brightDiffuse < 0 Then brightSpecular2 = 0
                                If brightDiffuse < 0 Then brightDiffuse = 0
                                ' brightDiffuse = 0
                                'мощность
                                dst_w = (65536 * 4096) / (cameraDist * wSpan)
                                rrz = 65536 * 4096 \ wSpan - cameraDist
                                rrx = (xStart - xAdd) * dst_w
                                rry = -(y - yAdd) * dst_w
                                koeff2 = lighterPoint.intense
                                koeff2 -= lighterPoint.attenutionA * ((lighterPoint.x - rrx) * (lighterPoint.x - rrx) + (lighterPoint.y - rry) * (lighterPoint.y - rry) + (lighterPoint.z - rrz) * (lighterPoint.z - rrz))
                                If koeff2 < 0 Then koeff2 = 0
                                If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                                    shX = rrx \ shadowScale - shadowDX
                                    shY = rrz \ shadowScale - shadowDY
                                    If shX < 0 OrElse shY < 0 OrElse shX > shadowWidth - 1 OrElse shY > shadowHeight - 1 Then
                                    Else
                                        shadowResult = 256 - shadowPixels((shY * shadowWidth + shX) * 3)
                                        koeff2 = (koeff2 * shadowResult) >> 8

                                    End If
                                End If
                                'все освещение
                                ' brightR2 = brightDiffuse << 8 '* koeff1
                                ' brightG2 = brightDiffuse << 8  '* koeff1
                                ' brightB2 = brightDiffuse << 8  '* koeff1
                                brightR2 = (((brightDiffuse + brightSpecular2) * lighterPoint.r) >> 8) * koeff2 + (ambientR << 8)
                                brightG2 = (((brightDiffuse + brightSpecular2) * lighterPoint.g) >> 8) * koeff2 + (ambientG << 8)
                                brightB2 = (((brightDiffuse + brightSpecular2) * lighterPoint.b) >> 8) * koeff2 + (ambientB << 8)
                                'начальные значения и шаг
                                brightR = brightR1
                                brightG = brightG1
                                brightB = brightB1
                                brightRstep = (brightR2 - brightR1) * revDrawSpanSizeCurrent
                                brightGstep = (brightG2 - brightG1) * revDrawSpanSizeCurrent
                                brightBstep = (brightB2 - brightB1) * revDrawSpanSizeCurrent
                            Else
                                brightR = fixedI
                                brightG = fixedI
                                brightB = fixedI
                                brightRstep = 0
                                brightGstep = 0
                                brightBstep = 0
                            End If

                            'уходим в рисование отрезка
                            infoPixelsWBuffered += xSpanEnd - xStart + 1
                            If special = SpecialMode.nearW Then
                                Dim k As Integer
                                For k = xStart To xSpanEnd
                                    Dim wo = wBuffer(k + widthOffset)
                                    Dim wDiff As Single = 1000000000.0 / CSng(w) - 1000000000.0 / CSng(wo)
                                    If wDiff > -15 AndAlso wDiff < 15 Then
                                        iv = (sv >> 16) And (textureSizeYLocal - 1)
                                        iu = (su >> 16) And (textureSizeXLocal - 1)
                                        r = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 2) * brightR) >> 16)
                                        g = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 1) * brightG) >> 16)
                                        b = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 0) * brightB) >> 16)
                                        r = 255
                                        g = 255
                                        b = 0
                                        If r > 255 Then r = 255
                                        If g > 255 Then g = 255
                                        If b > 255 Then b = 255
                                        If r < 0 Then r = 0
                                        If g < 0 Then g = 0
                                        If b < 0 Then b = 0
                                        pixels(widthOffset3 + 3 * k + 2) = r
                                        pixels(widthOffset3 + 3 * k + 1) = g
                                        pixels(widthOffset3 + 3 * k + 0) = b
                                        wBuffer(k + widthOffset) = w
                                        infoPixelsDrawed += 1
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                    brightR += brightRstep
                                    brightG += brightGstep
                                    brightB += brightBstep
                                Next
                            End If
no_draw:
                            su1 = su2
                            sv1 = sv2
                            xStart += drawSpanSize
                            koeff1 = koeff2
                            brightR1 = brightR2
                            brightG1 = brightG2
                            brightB1 = brightB2
                        End While
end_cycle:
                        'считаем новые значения на краях строки и сами края
                        lx1 += dlx1
                        lx2 += dlx2
                        lw1 += dlw1
                        lw2 += dlw2
                        luz1 += dluz1
                        luz2 += dluz2
                        lvz1 += dlvz1
                        lvz2 += dlvz2
                        lrx1 += dlrx1
                        lrx2 += dlrx2
                        lry1 += dlry1
                        lry2 += dlry2
                        lrz1 += dlrz1
                        lrz2 += dlrz2
                        lnx1 += dlnx1
                        lnx2 += dlnx2
                        lny1 += dlny1
                        lny2 += dlny2
                        lnz1 += dlnz1
                        lnz2 += dlnz2
                    Next
                End If
            End If
        End Sub
        Protected Const fixed As Single = 65536
        Protected Const fixedI As Integer = 65536
        Private Function VertexLight(ByRef lighter As LighterStruct, ByVal x As Single, ByVal y As Single, ByVal w As Integer, ByVal nx As Single, ByVal ny As Single, ByVal nz As Single, ByRef koeff As Integer) As Integer
            Dim avgX, avgY, avgZ As Integer
            Dim lnx, lny, lnz As Integer
            Dim tnx, tny, tnz As Integer
            Dim lnLength As Integer
            Dim alpha As Integer
            Dim dst_w As Single = (65536 * 4096) / (cameraDist * w)
            tnx = CInt(nx * 256)
            tny = CInt(ny * 256)
            tnz = CInt(nz * 256)
            avgZ = CInt(65536 * 4096 \ w - cameraDist)
            avgX = CInt((x - (width >> 1)) * dst_w)
            avgY = CInt(((height >> 1) - y) * dst_w)
            lnx = (avgX - CInt(lighter.x))
            lny = (avgY - CInt(lighter.y))
            lnz = (avgZ - CInt(lighter.z))
            lnLength = (lnx * lnx + lny * lny + lnz * lnz)
            If lnLength > sqrtTableSize Then lnLength = sqrtTableSize
            If lnLength < 0 Then lnLength = 0
            lnLength = (sqrtTable(lnLength) >> 4) + 1
            alpha = (lnx * tnx + lny * tny + lnz * tnz) \ lnLength
            If alpha < 0 Then alpha = 0
            'расчет растояния
            koeff = lighterPoint.intense
            koeff -= lighterPoint.attenutionA * ((lighter.x - avgX) * (lighter.x - avgX) + (lighter.y - avgY) * (lighter.y - avgY) + (lighter.z - avgZ) * (lighter.z - avgZ))
            If koeff < 0 Then koeff = 0
            Return alpha
        End Function



        ''' <summary>
        ''' Отрисовать треугольник модели по его начальной вершине
        ''' </summary>
        ''' <param name="triangleNum"></param>
        ''' <remarks></remarks>
        Private Sub DrawTriangle(ByVal triangleNum As Integer)
            Dim startVertex As Integer = triangleNum * 3
            Dim shadowedBy As Integer
            Dim shadowPixels As Byte()
            Dim shadowWidth, shadowHeight, shadowScale As Integer
            Dim shadowDX, shadowDY As Integer
            Dim i As Integer, dropTriangle As Boolean
            Dim smallest As Single, smallestNum As Integer
            Dim settings As RenderParameters = trianglesCulled.renderSettings(triangleNum)
            Dim useBilinear As Boolean
            Dim useLightning As Boolean
            If settings.lighting = LightingMode.full Then useLightning = True
            If settings.texturing = TexturingMode.bilinear Then useBilinear = True
            'прежде всего - проверяем буфер наибольших треугольников
            smallest = trianglesBiggest.screenSize(0)
            For i = 0 To trianglesBiggestBuffer - 1
                If trianglesBiggest.minW(i) > trianglesDraw.maxW(triangleNum) Then
                    If trianglesBiggest.maxX(i) >= trianglesDraw.maxX(triangleNum) _
 AndAlso trianglesBiggest.maxY(i) >= trianglesDraw.maxY(triangleNum) _
 AndAlso trianglesBiggest.minX(i) <= trianglesDraw.minX(triangleNum) _
 AndAlso trianglesBiggest.minY(i) <= trianglesDraw.minY(triangleNum) Then dropTriangle = True
                End If
                If smallest > trianglesBiggest.screenSize(i) Then
                    smallest = trianglesBiggest.screenSize(i)
                    smallestNum = i
                End If
            Next
            'если треугольник больше наименьшего, пишем его вместо него
            '(да, криво, ага)
            If trianglesDraw.screenSize(triangleNum) > smallest Then
                trianglesBiggest.maxX(smallestNum) = trianglesDraw.maxX(triangleNum)
                trianglesBiggest.maxY(smallestNum) = trianglesDraw.maxY(triangleNum)
                trianglesBiggest.minX(smallestNum) = trianglesDraw.minX(triangleNum)
                trianglesBiggest.minY(smallestNum) = trianglesDraw.minY(triangleNum)
                trianglesBiggest.minW(smallestNum) = trianglesDraw.minW(triangleNum)
                trianglesBiggest.maxW(smallestNum) = trianglesDraw.maxW(triangleNum)
                trianglesBiggest.screenSize(smallestNum) = trianglesDraw.screenSize(triangleNum)
            End If
            'если треугольник закрыт, все остальное нам нахрен не нужно ;)
            'dropTriangle = True
            If Not dropTriangle Then
                infoTrianglesDrawed += 1
                Dim x1, x2, x3, y1, y2, y3 As Integer
                Dim w1, w2, w3 As Integer
                'текстурные координаты
                Dim u1, v1, u2, v2, u3, v3 As Single
                'экранные координаты
                Dim width, height As Integer
                width = myParent.width
                height = myParent.height
                'координаты вершин (экранные)
                'если материал изменился, перезагружаем текстуру
                If vertexesCulled.materialUID(startVertex) <> materialUID Then
                    ChangeTexture(vertexesCulled.material(startVertex))
                End If
                'MipMap
                Dim mipMap As Integer = 0
                Dim textureSizeYLocal, textureSizeXLocal As Integer
                If settings.mipMap = MipMapMode.mipMapOn Then
                    Dim textureSize, scrSize As Single
                    textureSize = trianglesCulled.textureSize(triangleNum)
                    If textureMipMap > 0 And textureSize > 0 Then
                        scrSize = trianglesDraw.screenSize(triangleNum)
                        textureSize *= sqrtTable(textureSizeY * textureSizeX) >> 4
                        mipMap = Math.Floor(Math.Log(textureSize / scrSize) / Math.Log(2)) + 5
                        If mipMap < 0 Then mipMap = 0
                        If mipMap > textureMipMap - 1 Then mipMap = textureMipMap - 1
                        textureSizeXLocal = texturePixels(mipMap).Width
                        textureSizeYLocal = texturePixels(mipMap).Height
                    End If
                Else
                    textureSizeXLocal = textureSizeX
                    textureSizeYLocal = textureSizeY
                End If
                Dim vertex1, vertex2, vertex3 As Integer
                vertex1 = 0 : vertex2 = 1 : vertex3 = 2
                y1 = vertexesDraw.scrY(startVertex + 0)
                y2 = vertexesDraw.scrY(startVertex + 1)
                y3 = vertexesDraw.scrY(startVertex + 2)
                'сортируем вершины
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                If y2 > y3 Then
                    Swap(y2, y3)
                    Swap(vertex2, vertex3)
                End If
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                shadowedBy = settings.shadowedBy
                If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                    With shadowPlanes(shadowedBy - 1)
                        shadowDX = .x1 \ .scale
                        shadowDY = .y1 \ .scale
                        shadowHeight = .map.Width
                        shadowWidth = .map.Height
                        shadowPixels = .map.pixels2
                        shadowScale = .scale
                        ' shadowScale = 1
                    End With
                Else
                    shadowPixels = Nothing
                End If
                'выборка данных
                x1 = vertexesDraw.scrX(startVertex + vertex1)
                x2 = vertexesDraw.scrX(startVertex + vertex2)
                x3 = vertexesDraw.scrX(startVertex + vertex3)
                u1 = vertexesCulled.tU(startVertex + vertex1) * textureSizeXLocal
                v1 = vertexesCulled.tV(startVertex + vertex1) * textureSizeYLocal
                u2 = vertexesCulled.tU(startVertex + vertex2) * textureSizeXLocal
                v2 = vertexesCulled.tV(startVertex + vertex2) * textureSizeYLocal
                u3 = vertexesCulled.tU(startVertex + vertex3) * textureSizeXLocal
                v3 = vertexesCulled.tV(startVertex + vertex3) * textureSizeYLocal
                w1 = vertexesDraw.w(startVertex + vertex1) * 4096 * fixed
                w2 = vertexesDraw.w(startVertex + vertex2) * 4096 * fixed
                w3 = vertexesDraw.w(startVertex + vertex3) * 4096 * fixed
                'текстурные координаты, деленные на w
                Dim pvz1, pvz2, pvz3, puz1, puz2, puz3 As Single
                puz1 = u1 * w1 : puz2 = u2 * w2 : puz3 = u3 * w3
                pvz1 = v1 * w1 : pvz2 = v2 * w2 : pvz3 = v3 * w3
                'реальные координаты
                Dim rx1, rx2, rx3, ry1, ry2, ry3, rz1, rz2, rz3 As Integer
                Dim nx1, nx2, nx3, ny1, ny2, ny3, nz1, nz2, nz3 As Integer
                Dim lrx1, lrx2, lry1, lry2, lrz1, lrz2 As Integer
                Dim lnx1, lnx2, lny1, lny2, lnz1, lnz2 As Integer
                Dim dlrx1, dlrx2, rxs, rx As Integer
                Dim dlry1, dlry2, rys, ry As Integer
                Dim dlrz1, dlrz2, rzs, rz As Integer
                Dim dlnx1, dlnx2, nxs, nx As Integer
                Dim dlny1, dlny2, nys, ny As Integer
                Dim dlnz1, dlnz2, nzs, nz As Integer
                '   If y2 = y3 Then y2 += 3
                '   If y2 = y1 Then y2 -= 3
                ' ; y1 -= 3
                '  y3 += 3
                If x1 < 1 Then x1 = 1
                If x2 < 1 Then x2 = 1
                If x3 < 1 Then x3 = 1
                If y1 < 1 Then y1 = 1
                If y2 < 1 Then y2 = 1
                If y3 < 1 Then y3 = 1
                If x1 > width - 2 Then x1 = width - 2
                If x2 > width - 2 Then x2 = width - 2
                If x3 > width - 2 Then x3 = width - 2
                If y1 > height - 2 Then y1 = height - 2
                If y2 > height - 2 Then y2 = height - 2
                If y3 > height - 2 Then y3 = height - 2
                If x1 > x2 Then
                    If x1 > x3 Then
                        x1 += 1
                    Else
                        x3 += 1
                    End If
                Else
                    If x2 > x3 Then
                        x2 += 1
                    Else
                        x3 += 1
                    End If
                End If
                If x1 < x2 Then
                    If x1 < x3 Then
                        x1 -= 1
                    Else
                        x3 -= 1
                    End If
                Else
                    If x2 < x3 Then
                        x2 -= 1
                    Else
                        x3 -= 1
                    End If
                End If
                'If y1 > y2 Or y2 > y3 Then Stop
                ' Dim color As Color = Drawing.Color.Gray
                Dim pixels() As Byte = myParent.drawBuffer.pixels2
                Dim texturePixelsMipMap() As Byte = texturePixels(mipMap).pixels2
                'экранные
                Dim y, x As Integer
                'начало, конец, шаг строки точек
                Dim xStart As Integer
                Dim xEnd As Integer
                Dim xSpanEnd As Integer
                'края треугольника - х
                Dim dlx1, dlx2 As Single
                Dim lx1, lx2 As Single
                'W на краях треугольника
                Dim dlw1, dlw2 As Integer
                Dim lw1, lw2, wSpan, wSpanStep As Integer
                'U,V на краях треугольника
                Dim su1, su2, sv1, sv2 As Single
                Dim su, sv, us, vs, w, wStep As Integer
                Dim iu, iv As Integer
                'U\Z, V\Z
                Dim dluz1, dluz2, dlvz1, dlvz2 As Single
                Dim luz1, luz2, lvz1, lvz2 As Single
                Dim uz, vz As Single
                Dim uzs, vzs As Single
                Dim dlx2lx1 As Single
                'Y3-Y1,Y2-Y1
                Dim dy31, dy21, dy32 As Single
                dy31 = y3 - y1
                dy21 = y2 - y1
                dy32 = y3 - y2
                'для билинейной интерполяции
                Dim texWidthOffset1, texWidthOffset2 As Integer
                Dim texPixelOffset1, texPixelOffset2 As Integer
                Dim tex1, tex2, tex3, tex4 As Integer
                Dim tex12, tex34 As Integer
                Dim fu, fv As Integer
                'остальное
                Dim widthOffset As Integer
                Dim widthOffset3 As Integer
                'освещение
                'для репроекции
                'хыыы! опять прет !
                Dim xAdd, yAdd As Integer
                Dim length1 As Integer
                Dim length2 As Single
                Dim dst_ww As Single
                xAdd = myParent.width \ 2
                yAdd = myParent.height \ 2
                'вектор на источник света в точке
                'rz = 65536 * 4096 \ w - cameraDist
                ' rx = (xStart - xAdd) * dst_w
                'ry = -(y - yAdd) * dst_w
                dst_ww = (65536 * 4096) / (cameraDist * w1)
                rx1 = (lighterPoint.x - (x1 - xAdd) * dst_ww)
                ry1 = (lighterPoint.y - (yAdd - y1) * dst_ww)
                rz1 = (lighterPoint.z - (65536 * 4096 \ w1 - cameraDist))
                dst_ww = (65536 * 4096) / (cameraDist * w2)
                rx2 = (lighterPoint.x - (x2 - xAdd) * dst_ww)
                ry2 = (lighterPoint.y - (yAdd - y2) * dst_ww)
                rz2 = (lighterPoint.z - (65536 * 4096 \ w2 - cameraDist))
                dst_ww = (65536 * 4096) / (cameraDist * w3)
                rx3 = (lighterPoint.x - (x3 - xAdd) * dst_ww)
                ry3 = (lighterPoint.y - (yAdd - y3) * dst_ww)
                rz3 = (lighterPoint.z - (65536 * 4096 \ w3 - cameraDist))

                length1 = rx1 * rx1 + ry1 * ry1 + rz1 * rz1
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx1 *= length2 : ry1 *= length2 : rz1 *= length2

                length1 = rx2 * rx2 + ry2 * ry2 + rz2 * rz2
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx2 *= length2 : ry2 *= length2 : rz2 *= length2
                length1 = rx3 * rx3 + ry3 * ry3 + rz3 * rz3
                If length1 > sqrtTableSize Then length1 = sqrtTableSize
                If length1 < 0 Then length1 = 0
                length2 = 16384 / (sqrtTable(length1))
                rx3 *= length2 : ry3 *= length2 : rz3 *= length2

                'нормали для поверхности, нужны для освещений
                If Not settings.lighting = LightingMode.none Then
                    If settings.normals = NormalsInterpolationMode.oneForTriangle Then
                        nx1 = trianglesCulled.nX(triangleNum) * 1024
                        ny1 = trianglesCulled.nY(triangleNum) * 1024
                        nz1 = trianglesCulled.nZ(triangleNum) * 1024
                        ';  nx1 = 0
                        '   ny1 = 1024
                        '  nz1 = 0
                        nx2 = nx1 : nx3 = nx1
                        ny2 = ny1 : ny3 = ny1
                        nz2 = nz1 : nz3 = nz1
                    Else
                        nx1 = vertexesCulled.nX(startVertex + vertex1) * 1024
                        nx2 = vertexesCulled.nX(startVertex + vertex2) * 1024
                        nx3 = vertexesCulled.nX(startVertex + vertex3) * 1024
                        ny1 = vertexesCulled.nY(startVertex + vertex1) * 1024
                        ny2 = vertexesCulled.nY(startVertex + vertex2) * 1024
                        ny3 = vertexesCulled.nY(startVertex + vertex3) * 1024
                        nz1 = vertexesCulled.nZ(startVertex + vertex1) * 1024
                        nz2 = vertexesCulled.nZ(startVertex + vertex2) * 1024
                        nz3 = vertexesCulled.nZ(startVertex + vertex3) * 1024
                    End If
                Else

                End If
                Dim r, g, b As Integer
                ' Dim bright, brightStep As Integer
                Dim brightR, brightG, brightB As Integer
                Dim brightR1, brightG1, brightB1 As Integer
                Dim brightR2, brightG2, brightB2 As Integer
                Dim brightRstep, brightGstep, brightBstep As Integer
                Dim revDrawSpanSize As Single = 1 / drawSpanSize
                Dim revDrawSpanSizeCurrent As Single = revDrawSpanSize
                Dim shX, shY As Integer
                Dim shadowResult As Integer
                If y3 > y1 Then
                    'общие параметры для обоих частей
                    If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                        'dlu2 = (u3 - u1) / dy31 : dlv2 = (v3 - v1) / dy31
                        dlx2 = (x3 - x1) / dy31 : dlw2 = (w3 - w1) / dy31
                        dlrx2 = (rx3 - rx1) / dy31
                        dlry2 = (ry3 - ry1) / dy31
                        dlrz2 = (rz3 - rz1) / dy31
                        lrx2 = rx1 : lry2 = ry1 : lrz2 = rz1
                        dlnx2 = (nx3 - nx1) / dy31
                        dlny2 = (ny3 - ny1) / dy31
                        dlnz2 = (nz3 - nz1) / dy31
                        lnx2 = nx1 : lny2 = ny1 : lnz2 = nz1
                        lx2 = x1 : lw2 = w1
                        dluz2 = (puz3 - puz1) / dy31
                        dlvz2 = (pvz3 - pvz1) / dy31
                        luz2 = puz1 : lvz2 = pvz1
                    Else
                        dlx1 = (x3 - x1) / dy31 : dlw1 = (w3 - w1) / dy31
                        'dlu1 = (u3 - u1) / dy31 : dlv1 = (v3 - v1) / dy31
                        dlrx1 = (rx3 - rx1) / dy31
                        dlry1 = (ry3 - ry1) / dy31
                        dlrz1 = (rz3 - rz1) / dy31
                        lrx1 = rx1 : lry1 = ry1 : lrz1 = rz1
                        dlnx1 = (nx3 - nx1) / dy31
                        dlny1 = (ny3 - ny1) / dy31
                        dlnz1 = (nz3 - nz1) / dy31
                        lnx1 = nx1 : lny1 = ny1 : lnz1 = nz1
                        lx1 = x1 : lw1 = w1
                        dluz1 = (puz3 - puz1) / dy31
                        dlvz1 = (pvz3 - pvz1) / dy31
                        luz1 = puz1 : lvz1 = pvz1
                    End If
                    'рисуем верхнюю часть треугольника
                    If dy21 > 0 Then
                        If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                            'dlu1 = (u2 - u1) / dy21 : dlv1 = (v2 - v1) / dy21 
                            dlx1 = (x2 - x1) / dy21 : dlw1 = (w2 - w1) / dy21
                            dlrx1 = (rx2 - rx1) / dy21
                            dlry1 = (ry2 - ry1) / dy21
                            dlrz1 = (rz2 - rz1) / dy21
                            lrx1 = rx1 : lry1 = ry1 : lrz1 = rz1
                            dlnx1 = (nx2 - nx1) / dy21
                            dlny1 = (ny2 - ny1) / dy21
                            dlnz1 = (nz2 - nz1) / dy21
                            lnx1 = nx1 : lny1 = ny1 : lnz1 = nz1
                            lx1 = x1 : lw1 = w1
                            dluz1 = (puz2 - puz1) / dy21
                            dlvz1 = (pvz2 - pvz1) / dy21
                            luz1 = puz1 : lvz1 = pvz1
                        Else
                            dlx2 = (x2 - x1) / dy21 : dlw2 = (w2 - w1) / dy21
                            'dlu2 = (u2 - u1) / dy21 : dlv2 = (v2 - v1) / dy21
                            dlrx2 = (rx2 - rx1) / dy21
                            dlry2 = (ry2 - ry1) / dy21
                            dlrz2 = (rz2 - rz1) / dy21
                            lrx2 = rx1 : lry2 = ry1 : lrz2 = rz1
                            dlnx2 = (nx2 - nx1) / dy21
                            dlny2 = (ny2 - ny1) / dy21
                            dlnz2 = (nz2 - nz1) / dy21
                            lnx2 = nx1 : lny2 = ny1 : lnz2 = nz1
                            lx2 = x1 : lw2 = w1
                            dluz2 = (puz2 - puz1) / dy21
                            dlvz2 = (pvz2 - pvz1) / dy21
                            luz2 = puz1 : lvz2 = pvz1
                        End If
                    End If
                    For y = y1 To y3 - 1
                        'проверяем. не перешли ли через границу Y2
                        If y = y2 Then
                            'рисуем нижнюю часть треугольника
                            If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                                dlx1 = (x3 - x2) / dy32 : dlw1 = (w3 - w2) / dy32
                                lx1 = x2 : lw1 = w2
                                dlrx1 = (rx3 - rx2) / dy32
                                dlry1 = (ry3 - ry2) / dy32
                                dlrz1 = (rz3 - rz2) / dy32
                                lrx1 = rx2 : lry1 = ry2 : lrz1 = rz2
                                dlnx1 = (nx3 - nx2) / dy32
                                dlny1 = (ny3 - ny2) / dy32
                                dlnz1 = (nz3 - nz2) / dy32
                                lnx1 = nx2 : lny1 = ny2 : lnz1 = nz2
                                dluz1 = (puz3 - puz2) / dy32
                                dlvz1 = (pvz3 - pvz2) / dy32
                                luz1 = puz2 : lvz1 = pvz2
                            Else
                                dlx2 = (x3 - x2) / dy32 : dlw2 = (w3 - w2) / dy32
                                lx2 = x2 : lw2 = w2
                                dlrx2 = (rx3 - rx2) / dy32
                                dlry2 = (ry3 - ry2) / dy32
                                dlrz2 = (rz3 - rz2) / dy32
                                lrx2 = rx2 : lry2 = ry2 : lrz2 = rz2
                                dlnx2 = (nx3 - nx2) / dy32
                                dlny2 = (ny3 - ny2) / dy32
                                dlnz2 = (nz3 - nz2) / dy32
                                lnx2 = nx2 : lny2 = ny2 : lnz2 = nz2
                                dluz2 = (puz3 - puz2) / dy32
                                dlvz2 = (pvz3 - pvz2) / dy32
                                luz2 = puz2 : lvz2 = pvz2
                            End If
                        End If
                        If lx2 - lx1 < 1 Then GoTo end_cycle
                        'ускоряющие замены
                        widthOffset = y * width
                        widthOffset3 = widthOffset * 3
                        ' widthOffset3Right = (y + 1) * width * 3
                        dlx2lx1 = 1 / (lx2 - lx1)
                        'начало и конец
                        xStart = lx1
                        xEnd = lx2
                        'шаги W - одиночный и через отрезок
                        wStep = (lw2 - lw1) * dlx2lx1
                        wSpanStep = wStep * drawSpanSize
                        'начальные значения для первого отрезка
                        w = lw1 : wSpan = lw1
                        uz = luz1 : vz = lvz1
                        uzs = (luz2 - luz1) * drawSpanSize * dlx2lx1
                        vzs = (lvz2 - lvz1) * drawSpanSize * dlx2lx1
                        su1 = uz / lw1
                        sv1 = vz / lw1
                        rx = lrx1
                        ry = lry1
                        rz = lrz1
                        rxs = (lrx2 - lrx1) * drawSpanSize * dlx2lx1
                        rys = (lry2 - lry1) * drawSpanSize * dlx2lx1
                        rzs = (lrz2 - lrz1) * drawSpanSize * dlx2lx1
                        nx = lnx1
                        ny = lny1
                        nz = lnz1
                        nxs = (lnx2 - lnx1) * drawSpanSize * dlx2lx1
                        nys = (lny2 - lny1) * drawSpanSize * dlx2lx1
                        nzs = (lnz2 - lnz1) * drawSpanSize * dlx2lx1
                        revDrawSpanSizeCurrent = revDrawSpanSize

                        Dim koeff1, koeff2 As Integer
                        Dim brightDiffuse, brightSpecular2 As Integer
                        Dim brightSpecular1 As Single
                        Dim dst_w As Single
                        Dim revX, revY, revZ As Integer
                        Dim camX, camY, camZ As Integer
                        Dim rrx, rry, rrz, kk As Integer
                        If useLightning Then
                            'освещение для первого отрезка - отражение и рассеивание
                            'вектор на камеру
                            camX = cameraVectorsX(widthOffset + xStart)
                            camY = cameraVectorsY(widthOffset + xStart)
                            camZ = cameraVectorsZ(widthOffset + xStart)
                            'вектор отражения
                            'Dim llll = Math.Sqrt(nx * nx + ny * ny + nz * nz)
                            'If llll > 1500 Then Stop
                            kk = (nx * rx + ny * ry + nz * rz) >> 10 'k=n*lightv
                            revX = rx - ((2 * kk * nx) >> 10)
                            revY = ry - ((2 * kk * ny) >> 10)
                            revZ = rz - ((2 * kk * nz) >> 10)
                            brightSpecular1 = CSng(-((revX * camX + revY * camY + revZ * camZ) >> 12) - 150) / 16
                            'brightSpecular1 *= 5
                            'упрощенная следующая из первой формула
                            If brightSpecular1 > 0 Then
                                brightSpecular2 = brightSpecular1 * brightSpecular1 * brightSpecular1 '* brightSpecular1
                            Else
                                brightSpecular2 = 0
                            End If
                            brightDiffuse = -(rx * nx + ry * ny + rz * nz) >> 12
                            '  If brightDiffuse < 0 Then brightSpecular2 = 0
                            If brightDiffuse < 0 Then brightDiffuse = 0
                            '  brightDiffuse = 0
                            'освещение для первого отрезка (мощность)
                            dst_w = (65536 * 4096) / (cameraDist * w)
                            rrz = 65536 * 4096 \ w - cameraDist
                            rrx = (xStart - xAdd) * dst_w
                            rry = -(y - yAdd) * dst_w
                            koeff1 = lighterPoint.intense
                            koeff1 -= lighterPoint.attenutionA * ((lighterPoint.x - rrx) * (lighterPoint.x - rrx) + (lighterPoint.y - rry) * (lighterPoint.y - rry) + (lighterPoint.z - rrz) * (lighterPoint.z - rrz))
                            If koeff1 < 0 Then koeff1 = 0
                            'все освещение для первого отрезка
                            '     brightR1 = brightDiffSpan << 8 '* koeff1
                            '     brightG1 = brightDiffSpan << 8 '* koeff1
                            '     brightB1 = brightDiffSpan << 8 '* koeff1
                            '   brightR1 = brightDiffuse << 8  '* koeff1+100000
                            '  brightG1 = brightDiffuse << 8  '* koeff1
                            '     brightB1 = brightDiffuse << 8   '* koeff1
                            If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                                shX = (rrx \ shadowScale - shadowDX)
                                shY = (rrz \ shadowScale - shadowDY)
                                If shX < 0 OrElse shY < 0 OrElse shX > shadowWidth - 1 OrElse shY > shadowHeight - 1 Then
                                Else
                                    shadowResult = 256 - shadowPixels((shY * shadowWidth + shX) * 3)
                                    koeff1 = (koeff1 * shadowResult) >> 8
                                End If
                            End If
                            brightR1 = (((brightDiffuse + brightSpecular2) * lighterPoint.r) >> 8) * koeff1 + (ambientR << 8)
                            brightG1 = (((brightDiffuse + brightSpecular2) * lighterPoint.g) >> 8) * koeff1 + (ambientG << 8)
                            brightB1 = (((brightDiffuse + brightSpecular2) * lighterPoint.b) >> 8) * koeff1 + (ambientB << 8)
                        End If
                        'рисуем отрезки

                        While xStart < xEnd
                            If xEnd - xStart <= drawSpanSize Then
                                wSpan = lw2
                                uz = luz2
                                vz = lvz2
                                rx = lrx2
                                ry = lry2
                                rz = lrz2
                                nx = lnx2
                                ny = lny2
                                nz = lnz2
                                revDrawSpanSizeCurrent = 1 / (xEnd - xStart)
                            Else
                                wSpan += wSpanStep
                                uz += uzs
                                vz += vzs
                                rx += rxs
                                ry += rys
                                rz += rzs
                                nx += nxs
                                ny += nys
                                nz += nzs
                            End If
                            'считаем значения на конце отрезка
                            su2 = (uz) / wSpan
                            sv2 = (vz) / wSpan
                            us = (su2 - su1) * revDrawSpanSizeCurrent * fixed
                            vs = (sv2 - sv1) * revDrawSpanSizeCurrent * fixed
                            su = su1 * fixed
                            sv = sv1 * fixed
                            xSpanEnd = xStart + drawSpanSize
                            If xSpanEnd > xEnd Then xSpanEnd = xEnd
                            'cчитаем освещение на конце отрезка 
                            If useLightning Then
                                'вектор на камеру
                                camX = cameraVectorsX(widthOffset + xSpanEnd)
                                camY = cameraVectorsY(widthOffset + xSpanEnd)
                                camZ = cameraVectorsZ(widthOffset + xSpanEnd)
                                ' camX = 0
                                ' camY = 0
                                ' camZ = 1024
                                'вектор отражения
                                kk = (nx * rx + ny * ry + nz * rz) >> 10 'k=n*lightv
                                revX = rx - ((2 * kk * nx) >> 10)
                                revY = ry - ((2 * kk * ny) >> 10)
                                revZ = rz - ((2 * kk * nz) >> 10)
                                brightSpecular1 = CSng(-((revX * camX + revY * camY + revZ * camZ) >> 12) - 150) / 16

                                'упрощенная следующая из первой формула
                                If brightSpecular1 > 0 Then
                                    brightSpecular2 = brightSpecular1 * brightSpecular1 * brightSpecular1 ' * brightSpecular1
                                Else
                                    brightSpecular2 = 0
                                End If
                                brightDiffuse = -(rx * nx + ry * ny + rz * nz) >> 12
                                '  If brightDiffuse < 0 Then brightSpecular2 = 0
                                If brightDiffuse < 0 Then brightDiffuse = 0
                                ' brightDiffuse = 0
                                'мощность
                                dst_w = (65536 * 4096) / (cameraDist * wSpan)
                                rrz = 65536 * 4096 \ wSpan - cameraDist
                                rrx = (xStart - xAdd) * dst_w
                                rry = -(y - yAdd) * dst_w
                                koeff2 = lighterPoint.intense
                                koeff2 -= lighterPoint.attenutionA * ((lighterPoint.x - rrx) * (lighterPoint.x - rrx) + (lighterPoint.y - rry) * (lighterPoint.y - rry) + (lighterPoint.z - rrz) * (lighterPoint.z - rrz))
                                If koeff2 < 0 Then koeff2 = 0
                                If shadowedBy > 0 AndAlso shadowPlanes(shadowedBy - 1).used Then
                                    shX = rrx \ shadowScale - shadowDX
                                    shY = rrz \ shadowScale - shadowDY
                                    If shX < 0 OrElse shY < 0 OrElse shX > shadowWidth - 1 OrElse shY > shadowHeight - 1 Then
                                    Else
                                        shadowResult = 256 - shadowPixels((shY * shadowWidth + shX) * 3)
                                        koeff2 = (koeff2 * shadowResult) >> 8

                                    End If
                                End If
                                'все освещение
                                ' brightR2 = brightDiffuse << 8 '* koeff1
                                ' brightG2 = brightDiffuse << 8  '* koeff1
                                ' brightB2 = brightDiffuse << 8  '* koeff1
                                brightR2 = (((brightDiffuse + brightSpecular2) * lighterPoint.r) >> 8) * koeff2 + (ambientR << 8)
                                brightG2 = (((brightDiffuse + brightSpecular2) * lighterPoint.g) >> 8) * koeff2 + (ambientG << 8)
                                brightB2 = (((brightDiffuse + brightSpecular2) * lighterPoint.b) >> 8) * koeff2 + (ambientB << 8)
                                'начальные значения и шаг
                                brightR = brightR1
                                brightG = brightG1
                                brightB = brightB1
                                brightRstep = (brightR2 - brightR1) * revDrawSpanSizeCurrent
                                brightGstep = (brightG2 - brightG1) * revDrawSpanSizeCurrent
                                brightBstep = (brightB2 - brightB1) * revDrawSpanSizeCurrent
                            Else
                                brightR = fixedI
                                brightG = fixedI
                                brightB = fixedI
                                brightRstep = 0
                                brightGstep = 0
                                brightBstep = 0
                            End If
                            'проверяем highW
                            '     Dim wmin, wmax, wtmp As Integer
                            '     Dim wh1, wh2 As Integer
                            '     If wSpanStep > 0 Then
                            'wmin = wSpan - wSpanStep : wmax = wSpan
                            '     Else
                            '     wmin = wSpan : wmax = wSpan - wSpanStep
                            '    End If
                            'Dim wBHoffset As Integer
                            '       wBHoffset = (myParent.width / 64) * y
                            '     wh1 = wBufferHigh(wBHoffset + (xStart >> 6))
                            '     wh2 = wBufferHigh(wBHoffset + (xSpanEnd >> 6))
                            '     If wh1 > wmax AndAlso wh2 > wmax Then
                            'w += wStep * (xSpanEnd - xStart)
                            '    GoTo no_draw
                            '   Else
                            '    '  If wmin > wmax Then wmin = wmax
                            '  If wh1 = 0 Or wh1 > wmin Then wBufferHigh(wBHoffset + (xStart >> 6)) = wmin
                            '    If wh2 = 0 Or wh2 > wmin Then wBufferHigh(wBHoffset + (xSpanEnd >> 6)) = wmin
                            '  End If
                            '  brightR = fixedI
                            '   brightG = fixedI
                            '   brightB = fixedI
                            'уходим в рисование отрезка
                            infoPixelsWBuffered += xSpanEnd - xStart + 1
                            If useBilinear Then
                                For x = xStart To xSpanEnd
                                    If w > wBuffer(x + widthOffset) Then
                                        iu = su >> 8
                                        iv = sv >> 8
                                        ' iv = iv And (textureSizeYLocal - 1)
                                        ' iu = iu And (textureSizeXLocal - 1)
                                        fu = iu And 255
                                        fv = iv And 255
                                        iu = iu >> 8
                                        iv = iv >> 8
                                        iv = iv And (textureSizeYLocal - 1)
                                        iu = iu And (textureSizeXLocal - 1)
                                        texWidthOffset1 = iv * textureSizeXLocal * 3
                                        texWidthOffset2 = (iv + 1) * textureSizeXLocal * 3
                                        texPixelOffset1 = iu * 3
                                        texPixelOffset2 = texPixelOffset1 + 3
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 0)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 0)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 0)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 0)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        r = ((255 - fv) * tex12 + fv * tex34) >> 16
                                        'pixels(widthOffset3 + 3 * x + 0) = tex
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 1)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 1)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 1)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 1)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        g = ((255 - fv) * tex12 + fv * tex34) >> 16
                                        'pixels(widthOffset3 + 3 * x + 1) = tex
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 2)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 2)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 2)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 2)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        b = ((255 - fv) * tex12 + fv * tex34) >> 16
                                        ' pixels(widthOffset3 + 3 * x + 2) = tex
                                        r = ((r * brightR) >> 16)
                                        g = ((g * brightG) >> 16)
                                        b = ((b * brightB) >> 16)
                                        If r > 255 Then r = 255
                                        If g > 255 Then g = 255
                                        If b > 255 Then b = 255
                                        If r < 0 Then r = 0
                                        If g < 0 Then g = 0
                                        If b < 0 Then b = 0
                                        pixels(widthOffset3 + 3 * x + 2) = r
                                        pixels(widthOffset3 + 3 * x + 1) = g '((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 1) * bg) >> 8)
                                        pixels(widthOffset3 + 3 * x + 0) = b '((texturePixelsMipMap(iv
                                        wBuffer(x + widthOffset) = w
                                        infoPixelsDrawed += 1
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                    brightR += brightRstep
                                    brightG += brightGstep
                                    brightB += brightBstep
                                Next
                            Else
                                Dim k As Integer
                                For k = xStart To xSpanEnd
                                    If w > wBuffer(k + widthOffset) Then
                                        iv = (sv >> 16) And (textureSizeYLocal - 1)
                                        iu = (su >> 16) And (textureSizeXLocal - 1)
                                        r = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 2) * brightR) >> 16)
                                        g = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 1) * brightG) >> 16)
                                        b = ((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 0) * brightB) >> 16)
                                        'r = (texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 0))
                                        'g = (texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 1))
                                        'b = (texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 2))
                                        If r > 255 Then r = 255
                                        If g > 255 Then g = 255
                                        If b > 255 Then b = 255
                                        If r < 0 Then r = 0
                                        If g < 0 Then g = 0
                                        If b < 0 Then b = 0
                                        pixels(widthOffset3 + 3 * k + 2) = r
                                        pixels(widthOffset3 + 3 * k + 1) = g '((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 1) * bg) >> 8)
                                        pixels(widthOffset3 + 3 * k + 0) = b '((texturePixelsMipMap(iv * textureSizeXLocal * 3 + iu * 3 + 2) * bb) >> 8)
                                        wBuffer(k + widthOffset) = w
                                        infoPixelsDrawed += 1
                                        '     If w < wBufferHigh(wBHoffset + (k >> 6)) Or wBufferHigh(wBHoffset + (k >> 6)) = 0 Then
                                        '      wBufferHigh(wBHoffset + (k >> 6)) = w
                                        ' End If
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                    brightR += brightRstep
                                    brightG += brightGstep
                                    brightB += brightBstep
                                Next
                            End If
no_draw:
                            su1 = su2
                            sv1 = sv2
                            xStart += drawSpanSize
                            koeff1 = koeff2
                            brightR1 = brightR2
                            brightG1 = brightG2
                            brightB1 = brightB2
                        End While
end_cycle:
                        'считаем новые значения на краях строки и сами края
                        lx1 += dlx1
                        lx2 += dlx2
                        lw1 += dlw1
                        lw2 += dlw2
                        luz1 += dluz1
                        luz2 += dluz2
                        lvz1 += dlvz1
                        lvz2 += dlvz2
                        lrx1 += dlrx1
                        lrx2 += dlrx2
                        lry1 += dlry1
                        lry2 += dlry2
                        lrz1 += dlrz1
                        lrz2 += dlrz2
                        lnx1 += dlnx1
                        lnx2 += dlnx2
                        lny1 += dlny1
                        lny2 += dlny2
                        lnz1 += dlnz1
                        lnz2 += dlnz2
                    Next
                End If
            End If
        End Sub
        Private Sub DrawTriangleSimple(ByVal triangleNum As Integer)
            Dim startVertex As Integer = triangleNum * 3
            Dim settings As RenderParameters = trianglesCulled.renderSettings(triangleNum)
            Dim drawMethod As Integer
            If settings.texturing = TexturingMode.bilinear Then
                drawMethod = 1
            Else
                If settings.lighting = LightingMode.none Then drawMethod = 3 Else drawMethod = 2
            End If
            If settings.texturing = TexturingMode.transparent Then drawMethod = 4
            Dim i As Integer, dropTriangle As Boolean
            Dim smallest As Single, smallestNum As Integer
            'прежде всего - проверяем буфер наибольших треугольников
            smallest = trianglesBiggest.screenSize(0)
            For i = 0 To trianglesBiggestBuffer - 1
                If trianglesBiggest.minW(i) > trianglesDraw.maxW(triangleNum) Then
                    If trianglesBiggest.maxX(i) >= trianglesDraw.maxX(triangleNum) _
 AndAlso trianglesBiggest.maxY(i) >= trianglesDraw.maxY(triangleNum) _
 AndAlso trianglesBiggest.minX(i) <= trianglesDraw.minX(triangleNum) _
 AndAlso trianglesBiggest.minY(i) <= trianglesDraw.minY(triangleNum) Then dropTriangle = True
                End If
                If smallest > trianglesBiggest.screenSize(i) Then
                    smallest = trianglesBiggest.screenSize(i)
                    smallestNum = i
                End If
            Next
            'если треугольник больше наименьшего, пишем его вместо него
            '(да, криво, ага)
            If trianglesDraw.screenSize(triangleNum) > smallest And drawMethod <> 4 Then
                trianglesBiggest.maxX(smallestNum) = trianglesDraw.maxX(triangleNum)
                trianglesBiggest.maxY(smallestNum) = trianglesDraw.maxY(triangleNum)
                trianglesBiggest.minX(smallestNum) = trianglesDraw.minX(triangleNum)
                trianglesBiggest.minY(smallestNum) = trianglesDraw.minY(triangleNum)
                trianglesBiggest.minW(smallestNum) = trianglesDraw.minW(triangleNum)
                trianglesBiggest.maxW(smallestNum) = trianglesDraw.maxW(triangleNum)
                trianglesBiggest.screenSize(smallestNum) = trianglesDraw.screenSize(triangleNum)
            End If
            'если треугольник закрыт, все остальное нам нахрен не нужно ;)
            'dropTriangle = True
            If Not dropTriangle Then
                infoTrianglesDrawed += 1
                Dim x1, x2, x3, y1, y2, y3 As Integer
                Dim w1, w2, w3 As Integer
                'текстурные координаты
                Dim u1, v1, u2, v2, u3, v3 As Single
                'координаты вершин (экранные)
                'если материал изменился, перезагружаем текстуру
                If vertexesCulled.materialUID(startVertex) <> materialUID Then
                    ChangeTexture(vertexesCulled.material(startVertex))
                End If
                Dim vertex1, vertex2, vertex3 As Integer
                vertex1 = 0 : vertex2 = 1 : vertex3 = 2
                y1 = vertexesDraw.scrY(startVertex + 0)
                y2 = vertexesDraw.scrY(startVertex + 1)
                y3 = vertexesDraw.scrY(startVertex + 2)
                'сортируем вершины
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                If y2 > y3 Then
                    Swap(y2, y3)
                    Swap(vertex2, vertex3)
                End If
                If y1 > y2 Then
                    Swap(y1, y2)
                    Swap(vertex1, vertex2)
                End If
                'выборка данных
                x1 = vertexesDraw.scrX(startVertex + vertex1)
                x2 = vertexesDraw.scrX(startVertex + vertex2)
                x3 = vertexesDraw.scrX(startVertex + vertex3)
                u1 = vertexesCulled.tU(startVertex + vertex1) * textureSizeX
                v1 = vertexesCulled.tV(startVertex + vertex1) * textureSizeY
                u2 = vertexesCulled.tU(startVertex + vertex2) * textureSizeX
                v2 = vertexesCulled.tV(startVertex + vertex2) * textureSizeY
                u3 = vertexesCulled.tU(startVertex + vertex3) * textureSizeX
                v3 = vertexesCulled.tV(startVertex + vertex3) * textureSizeY
                w1 = vertexesDraw.w(startVertex + vertex1) * 4096 * fixed
                w2 = vertexesDraw.w(startVertex + vertex2) * 4096 * fixed
                w3 = vertexesDraw.w(startVertex + vertex3) * 4096 * fixed
                'текстурные координаты, деленные на w
                Dim pvz1, pvz2, pvz3, puz1, puz2, puz3 As Single
                puz1 = u1 * w1 : puz2 = u2 * w2 : puz3 = u3 * w3
                pvz1 = v1 * w1 : pvz2 = v2 * w2 : pvz3 = v3 * w3
                'реальные координаты
                If y2 = y3 Then y2 += 1
                If y2 = y1 Then y2 -= 1
                y1 -= 1
                y3 += 1
                If x1 < 1 Then x1 = 1
                If x2 < 1 Then x2 = 1
                If x3 < 1 Then x3 = 1
                If y1 < 1 Then y1 = 1
                If y2 < 1 Then y2 = 1
                If y3 < 1 Then y3 = 1
                If x1 > width - 2 Then x1 = width - 2
                If x2 > width - 2 Then x2 = width - 2
                If x3 > width - 2 Then x3 = width - 2
                If y1 > height - 2 Then y1 = height - 2
                If y2 > height - 2 Then y2 = height - 2
                If y3 > height - 2 Then y3 = height - 2
                Dim pixels() As Byte = myParent.drawBuffer.pixels2
                Dim texturePixelsMipMap() As Byte = texturePixels(0).pixels2
                'экранные
                Dim y, x As Integer
                'начало, конец, шаг строки точек
                Dim xStart As Integer
                Dim xEnd As Integer
                'Dim xLength As Integer
                Dim xSpanEnd As Integer
                'края треугольника - х
                Dim dlx1, dlx2 As Single
                Dim lx1, lx2 As Single
                'W на краях треугольника
                Dim dlw1, dlw2 As Integer
                Dim lw1, lw2, wSpan, wSpanStep As Integer
                'U,V на краях треугольника
                Dim su1, su2, sv1, sv2 As Single
                Dim su, sv, us, vs, w, wStep As Integer
                Dim iu, iv As Integer
                'U\Z, V\Z
                Dim dluz1, dluz2, dlvz1, dlvz2 As Single
                Dim luz1, luz2, lvz1, lvz2 As Single
                Dim uz, vz As Single
                Dim uzs, vzs As Single
                Dim dlx2lx1 As Single
                'Y3-Y1,Y2-Y1
                Dim dy31, dy21, dy32 As Single
                dy31 = y3 - y1
                dy21 = y2 - y1
                dy32 = y3 - y2
                'для билинейной интерполяции
                Dim texWidthOffset1, texWidthOffset2 As Integer
                Dim texPixelOffset1, texPixelOffset2 As Integer
                Dim tex1, tex2, tex3, tex4 As Integer
                Dim tex12, tex34, tex As Integer
                Dim fu, fv As Integer
                'остальное
                Dim widthOffset As Integer
                Dim widthOffset3 As Integer
                'Dim widthOffset3Right As Integer
                'считам освещение по компонентам только для трех точек
                Dim brightDiff1, brightDiff2, brightDiff3 As Integer
                Dim brightR1, brightG1, brightB1 As Integer
                Dim brightR2, brightG2, brightB2 As Integer
                Dim brightR3, brightG3, brightB3 As Integer
                Dim koeff1, koeff2, koeff3 As Integer
                If settings.lighting <> LightingMode.none Then
                    If settings.lighting = LightingMode.ambientOnly Then
                        brightR1 = ambientR << 8 : brightG1 = ambientG << 8 : brightB1 = ambientB << 8
                        brightR2 = ambientR << 8 : brightG2 = ambientG << 8 : brightB2 = ambientB << 8
                        brightR3 = ambientR << 8 : brightG3 = ambientG << 8 : brightB3 = ambientB << 8
                    Else
                        If settings.normals = NormalsInterpolationMode.oneForTriangle Then
                            Dim nx, ny, nz As Single
                            nx = trianglesCulled.nX(triangleNum)
                            ny = trianglesCulled.nY(triangleNum)
                            nz = trianglesCulled.nZ(triangleNum)
                            brightDiff1 = VertexLight(lighterPoint, x1, y1, w1, nx, ny, nz, koeff1)
                            brightDiff2 = VertexLight(lighterPoint, x2, y2, w2, nx, ny, nz, koeff2)
                            brightDiff3 = VertexLight(lighterPoint, x3, y3, w3, nx, ny, nz, koeff3)
                        Else
                            brightDiff1 = VertexLight(lighterPoint, x1, y1, w1, vertexesCulled.nX(startVertex + vertex1), vertexesCulled.nY(startVertex + vertex1), vertexesCulled.nZ(startVertex + vertex1), koeff1)
                            brightDiff2 = VertexLight(lighterPoint, x2, y2, w2, vertexesCulled.nX(startVertex + vertex2), vertexesCulled.nY(startVertex + vertex2), vertexesCulled.nZ(startVertex + vertex2), koeff2)
                            brightDiff3 = VertexLight(lighterPoint, x3, y3, w3, vertexesCulled.nX(startVertex + vertex3), vertexesCulled.nY(startVertex + vertex3), vertexesCulled.nZ(startVertex + vertex3), koeff3)
                        End If
                        brightR1 = ambientR << 8 : brightG1 = ambientG << 8 : brightB1 = ambientB << 8
                        brightR2 = ambientR << 8 : brightG2 = ambientG << 8 : brightB2 = ambientB << 8
                        brightR3 = ambientR << 8 : brightG3 = ambientG << 8 : brightB3 = ambientB << 8
                        brightR1 += ((brightDiff1 * lighterPoint.r) >> 8) * koeff1
                        brightG1 += ((brightDiff1 * lighterPoint.g) >> 8) * koeff1
                        brightB1 += ((brightDiff1 * lighterPoint.b) >> 8) * koeff1
                        brightR2 += ((brightDiff2 * lighterPoint.r) >> 8) * koeff2
                        brightG2 += ((brightDiff2 * lighterPoint.g) >> 8) * koeff2
                        brightB2 += ((brightDiff2 * lighterPoint.b) >> 8) * koeff2
                        brightR3 += ((brightDiff3 * lighterPoint.r) >> 8) * koeff3
                        brightG3 += ((brightDiff3 * lighterPoint.g) >> 8) * koeff3
                        brightB3 += ((brightDiff3 * lighterPoint.b) >> 8) * koeff3
                        If brightR1 > fixedI Then brightR1 = fixedI
                        If brightR2 > fixedI Then brightR2 = fixedI
                        If brightR3 > fixedI Then brightR3 = fixedI
                        If brightG1 > fixedI Then brightG1 = fixedI
                        If brightG2 > fixedI Then brightG2 = fixedI
                        If brightG3 > fixedI Then brightG3 = fixedI
                        If brightB1 > fixedI Then brightB1 = fixedI
                        If brightB2 > fixedI Then brightB2 = fixedI
                        If brightB3 > fixedI Then brightB3 = fixedI
                    End If
                Else
                    brightR1 = fixedI : brightG1 = fixedI : brightB1 = fixedI
                    brightR2 = fixedI : brightG2 = fixedI : brightB2 = fixedI
                    brightR3 = fixedI : brightG3 = fixedI : brightB3 = fixedI
                End If
                'посчитали, теперь будем таскать за собой приращения
                Dim lBrightR1, lBrightR2, lBrightG1, lBrightG2, lBrightB1, lBrightB2 As Integer
                Dim dlBrightR1, dlBrightR2, dlBrightG1, dlBrightG2, dlBrightB1, dlBrightB2 As Integer
                Dim brightR, brightG, brightB As Integer
                Dim brightRstep, brightGstep, brightBstep As Integer
                'ускоряшки
                Dim revDrawSpanSize As Single = 1 / drawSpanSizeFast
                Dim revDrawSpanSizeCurrent As Single = revDrawSpanSize
                If y3 > y1 Then
                    'общие параметры для обоих частей
                    If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                        dlx2 = (x3 - x1) / dy31 : dlw2 = (w3 - w1) / dy31
                        lx2 = x1 : lw2 = w1
                        dluz2 = (puz3 - puz1) / dy31
                        dlvz2 = (pvz3 - pvz1) / dy31
                        luz2 = puz1 : lvz2 = pvz1
                        dlBrightR2 = (brightR3 - brightR1) / dy31 : lBrightR2 = brightR1
                        dlBrightG2 = (brightG3 - brightG1) / dy31 : lBrightG2 = brightG1
                        dlBrightB2 = (brightB3 - brightB1) / dy31 : lBrightB2 = brightB1
                    Else
                        dlx1 = (x3 - x1) / dy31 : dlw1 = (w3 - w1) / dy31
                        lx1 = x1 : lw1 = w1
                        dluz1 = (puz3 - puz1) / dy31
                        dlvz1 = (pvz3 - pvz1) / dy31
                        luz1 = puz1 : lvz1 = pvz1
                        dlBrightR1 = (brightR3 - brightR1) / dy31 : lBrightR1 = brightR1
                        dlBrightG1 = (brightG3 - brightG1) / dy31 : lBrightG1 = brightG1
                        dlBrightB1 = (brightB3 - brightB1) / dy31 : lBrightB1 = brightB1
                    End If
                    'рисуем верхнюю часть треугольника
                    If dy21 > 0 Then
                        If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                            dlx1 = (x2 - x1) / dy21 : dlw1 = (w2 - w1) / dy21
                            lx1 = x1 : lw1 = w1
                            dluz1 = (puz2 - puz1) / dy21
                            dlvz1 = (pvz2 - pvz1) / dy21
                            luz1 = puz1 : lvz1 = pvz1
                            dlBrightR1 = (brightR2 - brightR1) / dy21 : lBrightR1 = brightR1
                            dlBrightG1 = (brightG2 - brightG1) / dy21 : lBrightG1 = brightG1
                            dlBrightB1 = (brightB2 - brightB1) / dy21 : lBrightB1 = brightB1
                        Else
                            dlx2 = (x2 - x1) / dy21 : dlw2 = (w2 - w1) / dy21
                            lx2 = x1 : lw2 = w1
                            dluz2 = (puz2 - puz1) / dy21
                            dlvz2 = (pvz2 - pvz1) / dy21
                            luz2 = puz1 : lvz2 = pvz1
                            dlBrightR2 = (brightR2 - brightR1) / dy21 : lBrightR2 = brightR1
                            dlBrightG2 = (brightG2 - brightG1) / dy21 : lBrightG2 = brightG1
                            dlBrightB2 = (brightB2 - brightB1) / dy21 : lBrightB2 = brightB1
                        End If
                    End If
                    For y = y1 To y3 - 1
                        'проверяем. не перешли ли через границу Y2
                        If y = y2 Then
                            'рисуем нижнюю часть треугольника
                            If (x3 - x1) / dy31 > (x2 - x1) / dy21 Then
                                dlx1 = (x3 - x2) / dy32 : dlw1 = (w3 - w2) / dy32
                                lx1 = x2 : lw1 = w2
                                dluz1 = (puz3 - puz2) / dy32
                                dlvz1 = (pvz3 - pvz2) / dy32
                                luz1 = puz2 : lvz1 = pvz2
                                dlBrightR1 = (brightR3 - brightR2) / dy32 : lBrightR1 = brightR2
                                dlBrightG1 = (brightG3 - brightG2) / dy32 : lBrightG1 = brightG2
                                dlBrightB1 = (brightB3 - brightB2) / dy32 : lBrightB1 = brightB2
                            Else
                                dlx2 = (x3 - x2) / dy32 : dlw2 = (w3 - w2) / dy32
                                lx2 = x2 : lw2 = w2
                                dluz2 = (puz3 - puz2) / dy32
                                dlvz2 = (pvz3 - pvz2) / dy32
                                luz2 = puz2 : lvz2 = pvz2
                                dlBrightR2 = (brightR3 - brightR2) / dy32 : lBrightR2 = brightR2
                                dlBrightG2 = (brightG3 - brightG2) / dy32 : lBrightG2 = brightG2
                                dlBrightB2 = (brightB3 - brightB2) / dy32 : lBrightB2 = brightB2
                            End If
                        End If
                        If lx2 - lx1 < 1 Then GoTo end_cycle
                        'ускоряющие замены
                        widthOffset = y * width
                        widthOffset3 = widthOffset * 3
                        ' widthOffset3Right = (y + 1) * width * 3
                        dlx2lx1 = 1 / (lx2 - lx1)
                        'начало и конец
                        xStart = lx1 : xEnd = lx2
                        'шаги W - одиночный и через отрезок
                        wStep = (lw2 - lw1) * dlx2lx1
                        wSpanStep = wStep * drawSpanSizeFast
                        'освещенческие шаги
                        brightR = lBrightR1 : brightG = lBrightG1 : brightB = lBrightB1
                        brightRstep = (lBrightR2 - lBrightR1) * dlx2lx1
                        brightGstep = (lBrightG2 - lBrightG1) * dlx2lx1
                        brightBstep = (lBrightB2 - lBrightB1) * dlx2lx1
                        'начальные значения для первого отрезка
                        w = lw1 : wSpan = lw1
                        uz = luz1 : vz = lvz1
                        uzs = (luz2 - luz1) * drawSpanSizeFast * dlx2lx1
                        vzs = (lvz2 - lvz1) * drawSpanSizeFast * dlx2lx1
                        su1 = uz / lw1
                        sv1 = vz / lw1
                        revDrawSpanSizeCurrent = revDrawSpanSize
                        'рисуем отрезки
                        While xStart < xEnd
                            If xEnd - xStart <= drawSpanSizeFast Then
                                wSpan = lw2
                                uz = luz2
                                vz = lvz2
                                revDrawSpanSizeCurrent = 1 / (xEnd - xStart)
                            Else
                                wSpan += wSpanStep
                                uz += uzs
                                vz += vzs
                            End If
                            'считаем значения на конце отрезка
                            su2 = (uz) / wSpan
                            sv2 = (vz) / wSpan
                            us = (su2 - su1) * revDrawSpanSizeCurrent * fixed
                            vs = (sv2 - sv1) * revDrawSpanSizeCurrent * fixed
                            su = su1 * fixed
                            sv = sv1 * fixed
                            xSpanEnd = xStart + drawSpanSizeFast
                            If xSpanEnd > xEnd Then xSpanEnd = xEnd
                            'уходим в рисование отрезка
                            infoPixelsWBuffered += xSpanEnd - xStart + 1
                            If drawMethod = 1 Then
                                For x = xStart To xSpanEnd
                                    If w > wBuffer(x + widthOffset) Then
                                        iu = su >> 8
                                        iv = sv >> 8
                                        fu = iu And 255
                                        fv = iv And 255
                                        iu = iu >> 8
                                        iv = iv >> 8
                                        iv = iv And (textureSizeY - 1)
                                        iu = iu And (textureSizeX - 1)
                                        texWidthOffset1 = iv * textureSizeX * 3
                                        texWidthOffset2 = (iv + 1) * textureSizeX * 3
                                        texPixelOffset1 = iu * 3
                                        texPixelOffset2 = texPixelOffset1 + 3
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 0)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 0)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 0)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 0)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        tex = ((((255 - fv) * tex12 + fv * tex34) >> 16) * brightB) >> 16
                                        pixels(widthOffset3 + 3 * x + 0) = tex
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 1)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 1)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 1)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 1)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        tex = ((((255 - fv) * tex12 + fv * tex34) >> 16) * brightG) >> 16
                                        pixels(widthOffset3 + 3 * x + 1) = tex
                                        tex1 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset1 + 2)
                                        tex2 = texturePixelsMipMap(texWidthOffset1 + texPixelOffset2 + 2)
                                        tex3 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset1 + 2)
                                        tex4 = texturePixelsMipMap(texWidthOffset2 + texPixelOffset2 + 2)
                                        tex12 = ((255 - fu) * tex1 + fu * tex2) '>> 8
                                        tex34 = ((255 - fu) * tex3 + fu * tex4) ' >> 8
                                        tex = ((((255 - fv) * tex12 + fv * tex34) >> 16) * brightR) >> 16
                                        pixels(widthOffset3 + 3 * x + 2) = tex
                                        wBuffer(x + widthOffset) = w
                                        infoPixelsDrawed += 1
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                Next
                            End If
                            If drawMethod = 2 Then
                                Dim k As Integer
                                For k = xStart To xSpanEnd
                                    If w > wBuffer(k + widthOffset) Then
                                        iv = (sv >> 16) And (textureSizeY - 1)
                                        iu = (su >> 16) And (textureSizeX - 1)
                                        pixels(widthOffset3 + 3 * k + 2) = ((texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 2) * brightR) >> 16)
                                        pixels(widthOffset3 + 3 * k + 1) = ((texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 1) * brightG) >> 16)
                                        pixels(widthOffset3 + 3 * k + 0) = ((texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 0) * brightB) >> 16)
                                        wBuffer(k + widthOffset) = w
                                        infoPixelsDrawed += 1
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                    brightR += brightRstep
                                    brightG += brightGstep
                                    brightB += brightBstep
                                Next
                            End If
                            'с прозрачным белым
                            Dim txr, txg, txb As Integer
                            If drawMethod = 4 Then
                                Dim k As Integer
                                For k = xStart To xSpanEnd
                                    If w > wBuffer(k + widthOffset) Then
                                        iv = (sv >> 16) And (textureSizeY - 1)
                                        iu = (su >> 16) And (textureSizeX - 1)
                                        txr = (texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 2))
                                        txg = (texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 1))
                                        txb = (texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 0))
                                        ' If Not (txr > 128 AndAlso txg > 128 AndAlso txb > 128) Then
                                        If (txr + txg + txb < 200 * 3) Then
                                            pixels(widthOffset3 + 3 * k + 2) = ((txr * brightR) >> 16)
                                            pixels(widthOffset3 + 3 * k + 1) = ((txg * brightG) >> 16)
                                            pixels(widthOffset3 + 3 * k + 0) = ((txb * brightB) >> 16)
                                            wBuffer(k + widthOffset) = w
                                            infoPixelsDrawed += 1
                                        End If
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                    brightR += brightRstep
                                    brightG += brightGstep
                                    brightB += brightBstep
                                Next
                            End If
                            'быстрое без освещения
                            If drawMethod = 3 Then
                                Dim l As Integer
                                For l = xStart To xSpanEnd
                                    If w > wBuffer(l + widthOffset) Then
                                        iv = (sv >> 16) And (textureSizeY - 1)
                                        iu = (su >> 16) And (textureSizeX - 1)
                                        pixels(widthOffset3 + 3 * l + 0) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 0)
                                        pixels(widthOffset3 + 3 * l + 1) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 1)
                                        pixels(widthOffset3 + 3 * l + 2) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 2)
                                        wBuffer(l + widthOffset) = w
                                        infoPixelsDrawed += 1
                                    End If
                                    su += us
                                    sv += vs
                                    w += wStep
                                Next
                            End If
                            su1 = su2
                            sv1 = sv2
                            xStart += drawSpanSizeFast
                        End While
end_cycle:
                        'считаем новые значения на краях строки и сами края
                        lx1 += dlx1
                        lx2 += dlx2
                        lw1 += dlw1
                        lw2 += dlw2
                        luz1 += dluz1
                        luz2 += dluz2
                        lvz1 += dlvz1
                        lvz2 += dlvz2
                        lBrightR1 += dlBrightR1
                        lBrightG1 += dlBrightG1
                        lBrightB1 += dlBrightB1
                        lBrightR2 += dlBrightR2
                        lBrightG2 += dlBrightG2
                        lBrightB2 += dlBrightB2
                    Next
                End If
            End If
        End Sub
        Private Sub DrawTriangleVerySimple(ByVal triangleNum As Integer)
            Dim startVertex As Integer = triangleNum * 3
            infoTrianglesDrawed += 1
            Dim x1, x2, x3, y1, y2, y3 As Integer
            Dim w1, w2, w3 As Integer
            'текстурные координаты
            ' Dim u1, v1, u2, v2, u3, v3 As Single
            'координаты вершин (экранные)
            'если материал изменился, перезагружаем текстуру
            If vertexesCulled.materialUID(startVertex) <> materialUID Then
                ChangeTexture(vertexesCulled.material(startVertex))
            End If
            Dim vertex1, vertex2, vertex3 As Integer
            vertex1 = 0 : vertex2 = 1 : vertex3 = 2
            y1 = vertexesDraw.scrY(startVertex + 0)
            y2 = vertexesDraw.scrY(startVertex + 1)
            y3 = vertexesDraw.scrY(startVertex + 2)
            'сортируем вершины
            If y1 > y2 Then
                Swap(y1, y2)
                Swap(vertex1, vertex2)
            End If
            If y2 > y3 Then
                Swap(y2, y3)
                Swap(vertex2, vertex3)
            End If
            If y1 > y2 Then
                Swap(y1, y2)
                Swap(vertex1, vertex2)
            End If
            'текстурные координаты, деленные на w
            Dim pvz1, pvz2, pvz3, puz1, puz2, puz3 As Single
            puz1 = vertexesCulled.tU(startVertex + vertex1) * textureSizeX * w1
            puz2 = vertexesCulled.tU(startVertex + vertex2) * textureSizeX * w2
            puz3 = vertexesCulled.tU(startVertex + vertex2) * textureSizeY * w3
            pvz1 = vertexesCulled.tV(startVertex + vertex1) * textureSizeY * w1
            pvz2 = vertexesCulled.tV(startVertex + vertex2) * textureSizeY * w2
            pvz3 = vertexesCulled.tV(startVertex + vertex3) * textureSizeY * w3
            'реальные координаты
            If y2 = y3 Then y2 += 1
            If y2 = y1 Then y2 -= 1
            y1 -= 1
            y3 += 1
            If x1 < 1 Then x1 = 1
            If x2 < 1 Then x2 = 1
            If x3 < 1 Then x3 = 1
            If y1 < 1 Then y1 = 1
            If y2 < 1 Then y2 = 1
            If y3 < 1 Then y3 = 1
            If x1 > width - 2 Then x1 = width - 2
            If x2 > width - 2 Then x2 = width - 2
            If x3 > width - 2 Then x3 = width - 2
            If y1 > height - 2 Then y1 = height - 2
            If y2 > height - 2 Then y2 = height - 2
            If y3 > height - 2 Then y3 = height - 2
            Dim pixels() As Byte = myParent.drawBuffer.pixels2
            Dim texturePixelsMipMap() As Byte = texturePixels(0).pixels2
            'экранные
            Dim y, x As Integer
            'начало, конец, шаг строки точек
            Dim xStart As Integer
            Dim xEnd As Integer
            'Dim xLength As Integer
            Dim xSpanEnd As Integer
            'края треугольника - х
            Dim dlx1, dlx2 As Single
            Dim lx1, lx2 As Single
            'W на краях треугольника
            Dim dlw1, dlw2 As Integer
            Dim lw1, lw2, wSpan, wSpanStep As Integer
            'U,V на краях треугольника
            Dim su1, su2, sv1, sv2 As Single
            Dim su, sv, us, vs, w, wStep As Integer
            Dim iu, iv As Integer
            'U\Z, V\Z
            Dim dluz1, dluz2, dlvz1, dlvz2 As Single
            Dim luz1, luz2, lvz1, lvz2 As Single
            Dim uz, vz As Single
            Dim uzs, vzs As Single
            Dim dlx2lx1 As Single
            'Y3-Y1,Y2-Y1
            'остальное
            Dim widthOffset As Integer
            ' Dim widthOffset3 As Integer
            'ускоряшки
            ' Dim revDrawSpanSize As Single =
            If y3 > y1 Then
                'общие параметры для обоих частей
                If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                    dlx2 = (x3 - x1) / (y3 - y1) : dlw2 = (w3 - w1) / (y3 - y1)
                    lx2 = x1 : lw2 = w1
                    dluz2 = (puz3 - puz1) / (y3 - y1)
                    dlvz2 = (pvz3 - pvz1) / (y3 - y1)
                    luz2 = puz1 : lvz2 = pvz1
                Else
                    dlx1 = (x3 - x1) / (y3 - y1) : dlw1 = (w3 - w1) / (y3 - y1)
                    lx1 = x1 : lw1 = w1
                    dluz1 = (puz3 - puz1) / (y3 - y1)
                    dlvz1 = (pvz3 - pvz1) / (y3 - y1)
                    luz1 = puz1 : lvz1 = pvz1
                End If
                'рисуем верхнюю часть треугольника
                If y2 - y1 > 0 Then
                    If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                        dlx1 = (x2 - x1) / (y2 - y1) : dlw1 = (w2 - w1) / (y2 - y1)
                        lx1 = x1 : lw1 = w1
                        dluz1 = (puz2 - puz1) / (y2 - y1)
                        dlvz1 = (pvz2 - pvz1) / (y2 - y1)
                        luz1 = puz1 : lvz1 = pvz1
                    Else
                        dlx2 = (x2 - x1) / (y2 - y1) : dlw2 = (w2 - w1) / (y2 - y1)
                        lx2 = x1 : lw2 = w1
                        dluz2 = (puz2 - puz1) / y2 - y1
                        dlvz2 = (pvz2 - pvz1) / y2 - y1
                        luz2 = puz1 : lvz2 = pvz1
                    End If
                End If
                For y = y1 To y3 - 1
                    'проверяем. не перешли ли через границу Y2
                    If y = y2 Then
                        'рисуем нижнюю часть треугольника
                        If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                            dlx1 = (x3 - x2) / (y3 - y2) : dlw1 = (w3 - w2) / (y3 - y2)
                            lx1 = x2 : lw1 = w2
                            dluz1 = (puz3 - puz2) / (y3 - y2)
                            dlvz1 = (pvz3 - pvz2) / (y3 - y2)
                            luz1 = puz2 : lvz1 = pvz2
                        Else
                            dlx2 = (x3 - x2) / (y3 - y2) : dlw2 = (w3 - w2) / (y3 - y2)
                            lx2 = x2 : lw2 = w2
                            dluz2 = (puz3 - puz2) / (y3 - y2)
                            dlvz2 = (pvz3 - pvz2) / (y3 - y2)
                            luz2 = puz2 : lvz2 = pvz2
                        End If
                    End If
                    If lx2 - lx1 < 1 Then GoTo end_cycle
                    'ускоряющие замены
                    widthOffset = y * width
                    ' widthOffset3Right = (y + 1) * width * 3
                    dlx2lx1 = 1 / (lx2 - lx1)
                    'начало и конец
                    xStart = lx1 : xEnd = lx2
                    'шаги W - одиночный и через отрезок
                    wStep = (lw2 - lw1) * dlx2lx1
                    wSpanStep = wStep * drawSpanSizeFast
                    'начальные значения для первого отрезка
                    w = lw1 : wSpan = lw1
                    uz = luz1 : vz = lvz1
                    uzs = (luz2 - luz1) * drawSpanSizeFast * dlx2lx1
                    vzs = (lvz2 - lvz1) * drawSpanSizeFast * dlx2lx1
                    su1 = uz / lw1
                    sv1 = vz / lw1
                    'рисуем отрезки
                    While xStart < xEnd
                        If xEnd - xStart <= drawSpanSizeFast Then
                            wSpan = lw2
                            uz = luz2
                            vz = lvz2
                        Else
                            wSpan += wSpanStep
                            uz += uzs
                            vz += vzs
                        End If
                        'считаем значения на конце отрезка
                        su2 = (uz) / wSpan
                        sv2 = (vz) / wSpan
                        us = (su2 - su1) * (1 / drawSpanSizeFast) * fixed
                        vs = (sv2 - sv1) * (1 / drawSpanSizeFast) * fixed
                        su = su1 * fixed
                        sv = sv1 * fixed
                        xSpanEnd = xStart + drawSpanSizeFast
                        If xSpanEnd > xEnd Then xSpanEnd = xEnd
                        'уходим в рисование отрезка
                        infoPixelsWBuffered += xSpanEnd - xStart + 1
                        For x = xStart To xSpanEnd
                            If w > wBuffer(x + widthOffset) Then
                                iv = (sv >> 16) And (textureSizeY - 1)
                                iu = (su >> 16) And (textureSizeX - 1)
                                pixels(widthOffset * 3 + 3 * x + 0) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 0)
                                pixels(widthOffset * 3 + 3 * x + 1) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 1)
                                pixels(widthOffset * 3 + 3 * x + 2) = texturePixelsMipMap(iv * textureSizeX * 3 + iu * 3 + 2)
                                wBuffer(x + widthOffset) = w
                                infoPixelsDrawed += 1
                            End If
                            su += us
                            sv += vs
                            w += wStep
                        Next
                        su1 = su2
                        sv1 = sv2
                        xStart += drawSpanSizeFast
                    End While
end_cycle:
                    'считаем новые значения на краях строки и сами края
                    lx1 += dlx1
                    lx2 += dlx2
                    lw1 += dlw1
                    lw2 += dlw2
                    luz1 += dluz1
                    luz2 += dluz2
                    lvz1 += dlvz1
                    lvz2 += dlvz2
                Next
            End If

        End Sub
        Protected Sub ChangeTexture(ByVal material As Material)
            materialUID = material.UID
            texturePixels = material.texturePixels
            textureUse = material.textureUsed
            textureMipMap = material.maximumMipMapLevel
            texturePixels(0).GetSize(textureSizeX, textureSizeY)
        End Sub
        ''' <summary>
        ''' Обнулить W и Z-буфер в начале рисования сцены
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overloads Shared Sub Swap(ByRef var1 As Integer, ByRef var2 As Integer)
            Dim varTemp As Integer
            varTemp = var1
            var1 = var2
            var2 = varTemp
        End Sub

        Protected Overloads Shared Sub Swap(ByRef var1 As Single, ByRef var2 As Single)
            Dim varTemp As Single
            varTemp = var1
            var1 = var2
            var2 = varTemp
        End Sub
        ''' <summary>
        ''' Отрисовать в кадр сцену, используя переданные модели, источники света, камеру и т.д.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Render()
            Dim i As Integer
            shadowsUsed = False
            For i = 0 To shadowPlanesCount - 1
                If shadowPlanes(i).used Then shadowsUsed = True
            Next
            ComputeDistance()
            TransformCamera()
            If shadowsUsed Then
                ClearShadows()
                CastShadows()
            End If

            If vertexTop > 0 Then
                Culling()
                Projection()
                If settings.sortTriangles Then SortTriangles()
                If skyG > 0 AndAlso skyB > 0 AndAlso skyR > 0 Then DrawSky()
                If settings.drawTriangles Then RenderTexture()
                If settings.drawLines Then RenderLines()
                '  RenderLines()

            End If

            RenderSprites()
            'как ты относишься к кусочно-линейной интерполяции?
            If environment.fog > 0 Or environment.distortionA <> 0 Then
                PostProcess()
            End If

        End Sub
        Public ReadOnly Property InfoTrianglesCount() As Integer
            Get
                Return ((vertexTop + 1) \ 3)
            End Get
        End Property
        Public ReadOnly Property InfoTrianglesDrawnCount() As Integer
            Get
                Return infoTrianglesDrawed
            End Get
        End Property
        Public ReadOnly Property InfoPixelsDrawn() As Integer
            Get
                Return infoPixelsDrawed
            End Get
        End Property
        Public ReadOnly Property InfoPixelsDrawnRaw() As Integer
            Get
                Return infoPixelsWBuffered
            End Get
        End Property
        Public Property Camera() As Camera3D
            Get
                Return currentCamera
            End Get
            Set(ByVal value As Camera3D)
                currentCamera = value
                ComputeDistance()
            End Set
        End Property
        Public Property UseFowByWidth As Boolean
        Private Sub ComputeDistance()
            Dim result As Single
            Static lastFOV As Single
            If lastFOV <> currentCamera.FOV Then
                result = (myParent.diagonal / (2 * Math.Tan(currentCamera.FOV / 360 * Math.PI)))
                If UseFowByWidth Then
                    result = (myParent.width / (2 * Math.Tan(currentCamera.FOV / 360 * Math.PI)))
                End If
                cameraDist = result
                PrepareCameraVectors()
            End If
            lastFOV = currentCamera.FOV
        End Sub
        Private Sub TransformCamera()
            With currentCamera
                matrixTransition1.Transition(-.PositionX, -.PositionY, -.PositionZ)
                'matrixTransition1.Transition(0, 0, cameraDist)
                matrixRotY.RotationY(-.Yaw)
                matrixRotX.RotationX(-.Pitch)
                matrixRotZ.RotationZ(-.Roll)
                matrixTransition2.Transition(0, 0, -cameraDist)
            End With
            matrixFinal.CopyFrom(matrixTransition2)
            matrixFinal.MulOnMatrix(matrixRotX)
            matrixFinal.MulOnMatrix(matrixRotZ)
            matrixFinal.MulOnMatrix(matrixRotY)
            matrixFinal.MulOnMatrix(matrixTransition1)
            TransformVertexBuffer(matrixFinal, True)
            TransformLighters(matrixFinal)
            TransformSpritesBuffer(matrixFinal)
        End Sub
        Private Sub RenderSprites()
            Dim px, py As Integer
            Dim rScale As Single
            Dim w As Integer
            Dim rScaleRev As Single
            Dim sx1, sy1 As Single
            Dim slx, sly As Single
            Dim tlx, tly As Integer
            Dim tx1, tx2, ty1, ty2 As Integer
            Dim tx, ty As Integer
            Dim pixels As Byte()
            Dim texture As Byte()
            Dim textureWidth As Integer
            Dim sx, sy As Integer
            Dim r, g, b As Integer
            Dim i As Integer
            For i = 0 To spritesTop
                With sprites(i)
                    If .z - 1 > -cameraDist Then
                        'считываем данные
                        w = (1 / (.z + cameraDist)) * 4096 * fixedI
                        texture = .sprite.texture.pixels2
                        pixels = myParent.drawBuffer.pixels2
                        textureWidth = .sprite.texture.Width
                        sx1 = .sprite.left
                        sy = .sprite.right
                        'считаем координаты
                        rScale = cameraDist / (.z + cameraDist)
                        px = (width >> 1) + .x * rScale
                        py = (height >> 1) - .y * rScale
                        .px = px
                        .py = py
                        rScale *= .sprite.scale
                        If rScale < .sprite.minimumScale Then rScale = .sprite.minimumScale
                        If rScale > .sprite.maximumScale Then rScale = .sprite.maximumScale
                        slx = .sprite.right - .sprite.left
                        sly = .sprite.bottom - .sprite.top
                        'размах изображения влево и вправо
                        tlx = (slx * rScale) / 2
                        tly = (sly * rScale) / 2
                        rScaleRev = 1 / rScale
                        'координаты прямоугольника изображения
                        tx1 = px - tlx : tx2 = px + tlx
                        ty1 = py - tly : ty2 = py + tly
                        If tx1 < 0 Then tx1 = 0
                        If tx2 > width - 1 Then tx2 = width - 1
                        If ty1 < 0 Then ty1 = 0
                        If ty2 > height - 1 Then ty2 = height - 1
                        'считаем освещенность
                        Dim light As Integer
                        light = ((ambientB + ambientG + ambientR) / 3)
                        'проверяем персональный источник света
                        Dim lightr = 255
                        Dim lightg = 255
                        Dim lightb = 255
                        If .lighter IsNot Nothing Then
                            lightr = .lighter.colorR
                            lightg = .lighter.colorG
                            lightb = .lighter.colorB
                        End If
                        'считаем остальное
                        light += lighterPoint.intense
                        light -= lighterPoint.attenutionA * 5 * ((lighterPoint.x - .x) * (lighterPoint.x - .x) + (lighterPoint.y - .y) * (lighterPoint.y - .y) + (lighterPoint.z - .z) * (lighterPoint.z - .z))
                        If light > 255 Then light = 255
                        If light < 80 Then light = 80
                        'light = 255
                        lightr = lightr * light >> 8
                        lightg = lightg * light >> 8
                        lightb = lightb * light >> 8
                        'если после обрезаний на экране хоть что-то осталось
                        If tx2 > tx1 Then
                            DebugSpritesList.Add(sprites(i))
                            For ty = ty1 To ty2
                                For tx = tx1 To tx2
                                    If w > wBuffer(ty * width + tx) Then
                                        sx = ((tx - px + tlx) * rScaleRev) + sx1
                                        sy = ((ty - py + tly) * rScaleRev) + sy1
                                        r = texture((sy * textureWidth + sx) * 3 + 2)
                                        g = texture((sy * textureWidth + sx) * 3 + 1)
                                        b = texture((sy * textureWidth + sx) * 3 + 0)


                                        If r <> 255 Or b <> 255 Or g <> 0 Then
                                            pixels((ty * width + tx) * 3 + 2) = (r * lightr) >> 8
                                            pixels((ty * width + tx) * 3 + 1) = (g * lightg) >> 8
                                            pixels((ty * width + tx) * 3 + 0) = (b * lightb) >> 8
                                            '   wBuffer(ty * width + tx) = w
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    End If
                End With
            Next i
        End Sub
        Private Sub TransformSpritesBuffer(ByRef matrix As TransformMatrix)
            Dim m11, m12, m13, m21, m22, m23, m31, m32, m33, m14, m24, m34 As Single
            m11 = matrix.matrix(0, 0)
            m12 = matrix.matrix(0, 1)
            m13 = matrix.matrix(0, 2)
            m21 = matrix.matrix(1, 0)
            m22 = matrix.matrix(1, 1)
            m23 = matrix.matrix(1, 2)
            m31 = matrix.matrix(2, 0)
            m32 = matrix.matrix(2, 1)
            m33 = matrix.matrix(2, 2)
            m14 = matrix.matrix(0, 3)
            m24 = matrix.matrix(1, 3)
            m34 = matrix.matrix(2, 3)
            'развернем также исходный вектор
            Dim px, py, pz As Single
            Dim i As Integer
            For i = 0 To spritesTop
                px = sprites(i).x
                py = sprites(i).y
                pz = sprites(i).z
                sprites(i).x = px * m11 + py * m12 + pz * m13 + m14
                sprites(i).y = px * m21 + py * m22 + pz * m23 + m24
                sprites(i).z = px * m31 + py * m32 + pz * m33 + m34
            Next
        End Sub
        Private Sub TransformLighters(ByRef matrix As TransformMatrix)
            Dim m11, m12, m13, m21, m22, m23, m31, m32, m33, m14, m24, m34 As Single
            m11 = matrix.matrix(0, 0)
            m12 = matrix.matrix(0, 1)
            m13 = matrix.matrix(0, 2)
            m21 = matrix.matrix(1, 0)
            m22 = matrix.matrix(1, 1)
            m23 = matrix.matrix(1, 2)
            m31 = matrix.matrix(2, 0)
            m32 = matrix.matrix(2, 1)
            m33 = matrix.matrix(2, 2)
            m14 = matrix.matrix(0, 3)
            m24 = matrix.matrix(1, 3)
            m34 = matrix.matrix(2, 3)
            'развернем также исходный вектор
            Dim px, py, pz As Single
            px = lighterPoint.x
            py = lighterPoint.y
            pz = lighterPoint.z
            lighterPoint.x = px * m11 + py * m12 + pz * m13 + m14
            lighterPoint.y = px * m21 + py * m22 + pz * m23 + m24
            lighterPoint.z = px * m31 + py * m32 + pz * m33 + m34
        End Sub
        Private Sub TransformVertexBuffer(ByRef matrix As TransformMatrix, Optional ByVal saveCoordsAsReal As Boolean = False)
            'начинаем производить трансформации вершин
            'для ускорения развернем матрицу в отдельные переменные
            Dim m11, m12, m13, m21, m22, m23, m31, m32, m33, m14, m24, m34 As Single
            m11 = matrix.matrix(0, 0)
            m12 = matrix.matrix(0, 1)
            m13 = matrix.matrix(0, 2)
            m21 = matrix.matrix(1, 0)
            m22 = matrix.matrix(1, 1)
            m23 = matrix.matrix(1, 2)
            m31 = matrix.matrix(2, 0)
            m32 = matrix.matrix(2, 1)
            m33 = matrix.matrix(2, 2)
            m14 = matrix.matrix(0, 3)
            m24 = matrix.matrix(1, 3)
            m34 = matrix.matrix(2, 3)
            'развернем также исходный вектор
            Dim px, py, pz As Single
            Dim nx, ny, nz As Single
            'Dim rpx, rpy, rpz As Single
            ' Dim rnx, rny, rnz As Single
            Dim i As Integer
            Dim trg As Integer
            'просчитаем точки
            For i = 0 To vertexTop
                trg = i \ 3
                vertexPointer += 1
                px = vertexes.pX(i)
                py = vertexes.pY(i)
                pz = vertexes.pZ(i)
                nx = vertexes.nX(i)
                ny = vertexes.nY(i)
                nz = vertexes.nZ(i)
                If saveCoordsAsReal Then
                    vertexes.rX(i) = px
                    vertexes.rY(i) = py
                    vertexes.rY(i) = pz
                End If
                If triangles.renderSettings(trg).isSkybox = False Then
                    vertexes.pX(i) = px * m11 + py * m12 + pz * m13 + m14
                    vertexes.pY(i) = px * m21 + py * m22 + pz * m23 + m24
                    vertexes.pZ(i) = px * m31 + py * m32 + pz * m33 + m34
                Else
                    vertexes.pX(i) = px * m11 + py * m12 + pz * m13
                    vertexes.pY(i) = px * m21 + py * m22 + pz * m23
                    vertexes.pZ(i) = px * m31 + py * m32 + pz * m33
                End If
                vertexes.nX(i) = nx * m11 + ny * m12 + nz * m13
                vertexes.nY(i) = nx * m21 + ny * m22 + nz * m23
                vertexes.nZ(i) = nx * m31 + ny * m32 + nz * m33
            Next i
            For i = 0 To trianglesTop
                nx = triangles.nX(i)
                ny = triangles.nY(i)
                nz = triangles.nZ(i)
                triangles.nX(i) = nx * m11 + ny * m12 + nz * m13
                triangles.nY(i) = nx * m21 + ny * m22 + nz * m23
                triangles.nZ(i) = nx * m31 + ny * m32 + nz * m33
            Next
        End Sub
        ''' <summary>
        ''' Выполняет 3D-отсечение
        ''' </summary>
        ''' <remarks>
        ''' Использует метод Сазерленда-Ходжмана для 
        ''' трехмерного отсечения пятью плоскостями области зрения камеры.
        ''' </remarks>
        Private Sub Culling()
            'использует метод Сазерленда-Ходжмана для трехмерного отсечения пятью плоскостями
            Dim i, vi, a, b As Integer
            ' Dim x1, x2 As Integer
            Dim px1, py1, pz1 As Single
            Dim px2, py2, pz2 As Single
            Dim rx1, ry1, rz1 As Single
            Dim rx2, ry2, rz2 As Single
            Dim nx1, ny1, nz1 As Single
            Dim nx2, ny2, nz2 As Single
            Dim tu1, tv1 As Single
            Dim tu2, tv2 As Single
            Dim oz As Single
            Dim nx(5), ny(5), nz(5) As Single
            Dim dst As Single = cameraDist
            Dim xsz As Single = myParent.width / 2
            Dim ysz As Single = myParent.height / 2
            Dim result1 As Single
            Dim result2 As Single
            Dim m As Integer
            Dim k As Single
            vertexesCulled.name = "culled"
            vertexesCulledSwap.name = "culledSwap"
            'Dim tempVertexes As Vertexes
            'InitializeVertexArrays(tempVertexes, 10)
            'задаем точку (общую для всех секущих)
            oz = -dst + 0.01
            'и нормали к ним
            nx(0) = 0 : ny(0) = 0 : nz(0) = 1
            nx(1) = 0 : ny(1) = -dst : nz(1) = ysz
            nx(2) = 0 : ny(2) = dst : nz(2) = ysz
            nx(3) = -dst : ny(3) = 0 : nz(3) = xsz
            nx(4) = dst : ny(4) = 0 : nz(4) = xsz
            Dim vertexCount As Integer = vertexTop
            'это УКАЗАТЕЛИ на массивы!!
            Dim vertexSource, vertexTarget As Vertexes
            Dim direction As Integer = 1
            Dim triangleNum As Integer
            'vertexes.id = 2
            ' vertexesCulled.id = 1
            '  vertexesCulledSwap.id = 2
            For i = 0 To vertexCount Step 3
                vertexes.triangleNum(i) = i \ 3
            Next
            vertexSource = vertexes
            vertexTarget = vertexesCulled
            For m = 0 To 4
                vi = 0
                For i = 0 To vertexCount Step 3
                    triangleNum = i \ 3
                    'а - номер ребра треугольника
                    b = 0
                    For a = 0 To 2
                        px1 = px2 : py1 = py2 : pz1 = pz2
                        nx1 = nx2 : ny1 = ny2 : nz1 = nz2
                        rx1 = rx2 : ry1 = ry2 : rz1 = rz2
                        tu1 = tu2 : tv1 = tv2
                        result1 = result2
                        If a = 0 Then
                            px1 = vertexSource.pX(i + 0) : py1 = vertexSource.pY(i + 0) : pz1 = vertexSource.pZ(i + 0)
                            px2 = vertexSource.pX(i + 1) : py2 = vertexSource.pY(i + 1) : pz2 = vertexSource.pZ(i + 1)
                            nx1 = vertexSource.nX(i + 0) : ny1 = vertexSource.nY(i + 0) : nz1 = vertexSource.nZ(i + 0)
                            nx2 = vertexSource.nX(i + 1) : ny2 = vertexSource.nY(i + 1) : nz2 = vertexSource.nZ(i + 1)
                            tu1 = vertexSource.tU(i + 0) : tv1 = vertexSource.tV(i + 0)
                            tu2 = vertexSource.tU(i + 1) : tv2 = vertexSource.tV(i + 1)
                            rx2 = vertexSource.pX(i + 1) : ry2 = vertexSource.pY(i + 1) : rz2 = vertexSource.rZ(i + 1)
                            result1 = px1 * nx(m) + py1 * ny(m) + (pz1 - oz) * nz(m)
                        Else
                            If a = 1 Then
                                px2 = vertexSource.pX(i + 2) : py2 = vertexSource.pY(i + 2) : pz2 = vertexSource.pZ(i + 2)
                                nx2 = vertexSource.nX(i + 2) : ny2 = vertexSource.nY(i + 2) : nz2 = vertexSource.nZ(i + 2)
                                tu2 = vertexSource.tU(i + 2) : tv2 = vertexSource.tV(i + 2)
                                rx2 = vertexSource.rX(i + 2) : ry2 = vertexSource.rY(i + 2) : rz2 = vertexSource.rZ(i + 2)
                            Else 'a=2
                                px2 = vertexSource.pX(i + 0) : py2 = vertexSource.pY(i + 0) : pz2 = vertexSource.pZ(i + 0)
                                nx2 = vertexSource.nX(i + 0) : ny2 = vertexSource.nY(i + 0) : nz2 = vertexSource.nZ(i + 0)
                                tu2 = vertexSource.tU(i + 0) : tv2 = vertexSource.tV(i + 0)
                                rx2 = vertexSource.rX(i + 0) : ry2 = vertexSource.rY(i + 0) : rz2 = vertexSource.rZ(i + 0)
                            End If
                        End If
                        result2 = px2 * nx(m) + py2 * ny(m) + (pz2 - oz) * nz(m)
                        'result1 = 1
                        'result2 = 1
                        'начало лежит в области отсечения
                        If result1 > 0 Then
                            'добавляем начало в результат
                            vertexTarget.pX(vi + b) = px1
                            vertexTarget.pY(vi + b) = py1
                            vertexTarget.pZ(vi + b) = pz1
                            vertexTarget.material(vi + b) = vertexSource.material(i + a)
                            vertexTarget.materialUID(vi + b) = vertexSource.materialUID(i + a)
                            vertexTarget.tU(vi + b) = tu1
                            vertexTarget.tV(vi + b) = tv1
                            vertexTarget.nX(vi + b) = nx1
                            vertexTarget.nY(vi + b) = ny1
                            vertexTarget.nZ(vi + b) = nz1
                            vertexTarget.rX(vi + b) = rx1
                            vertexTarget.rY(vi + b) = ry1
                            vertexTarget.rZ(vi + b) = rz1
                            b += 1
                        End If
                        'начало лежит - а конец не лежит
                        'конец лежит - начало нет
                        If result2 * result1 < 0 Then
                            'ищем коэфф. для точки пересечения
                            'ха-ха! а готовый  не работает...
                            'Толстиков рулит с электронными лекциями...
                            ' k = (px1 * nx(m) + py1 * ny(m) + pz1 * nz(m)) / (px2 * nx(m) + py2 * ny(m) + pz2 * nz(m))
                            k = ((-px1) * nx(m) + (-py1) * ny(m) + (oz - pz1) * nz(m)) / ((px2 - px1) * nx(m) + (py2 - py1) * ny(m) + (pz2 - pz1) * nz(m))
                            'пишем точку пересечения
                            vertexTarget.pX(vi + b) = px1 + k * (px2 - px1)
                            vertexTarget.pY(vi + b) = py1 + k * (py2 - py1)
                            vertexTarget.pZ(vi + b) = pz1 + k * (pz2 - pz1)
                            vertexTarget.material(vi + b) = vertexSource.material(i + a)
                            vertexTarget.materialUID(vi + b) = vertexSource.materialUID(i + a)
                            vertexTarget.tU(vi + b) = tu1 + k * (tu2 - tu1)
                            vertexTarget.tV(vi + b) = tv1 + k * (tv2 - tv1)
                            vertexTarget.nX(vi + b) = nx1 + k * (nx2 - nx1)
                            vertexTarget.nY(vi + b) = ny1 + k * (ny2 - ny1)
                            vertexTarget.nZ(vi + b) = nz1 + k * (nz2 - nz1)
                            vertexTarget.rX(vi + b) = rx1 + k * (rx2 - rx1)
                            vertexTarget.rY(vi + b) = ry1 + k * (ry2 - ry1)
                            vertexTarget.rZ(vi + b) = rz1 + k * (rz2 - rz1)
                            b += 1
                        End If
                        '********************************************
                        'и мы умрем в один день, взявшись за руки...
                        'не пожалев ни на миг ни о чем...
                        '********************************************
                    Next a
                    'проверим, лицевая грань или нет
                    If m = 0 Then

                        nx1 = triangles.nX(triangleNum)
                        ny1 = triangles.nY(triangleNum)
                        nz1 = triangles.nZ(triangleNum)
                        px1 = vertexSource.pX(i)
                        py1 = vertexSource.pY(i)
                        pz1 = vertexSource.pZ(i)
                        If (nz1 * cameraDist + nx1 * px1 + ny1 * py1 + nz1 * pz1) * (triangles.renderSettings(triangleNum).culling - 2) < 0 Then
                            b = 0
                        End If
                    End If
                    If b = 3 Then
                        vertexTarget.triangleNum(vi) = vertexSource.triangleNum(i)
                        vi += 3
                    End If
                    If b = 4 Then
                        vertexTarget.triangleNum(vi) = vertexSource.triangleNum(i)
                        vertexTarget.triangleNum(vi + 3) = vertexSource.triangleNum(i)
                        'выполняем триангуляцию четырехугольника 0123 в 013 и 312 = 013312
                        VertexCopy(vertexTarget, vertexTarget, vi + 2, vi + 5)
                        VertexCopy(vertexTarget, vertexTarget, vi + 3, vi + 2)
                        VertexCopy(vertexTarget, vertexTarget, vi + 1, vi + 4)
                        vi += 6
                    End If
                Next i
                'забили массив, прогнав через отсечение одной плоскостью
                vertexCount = vi - 1
                'меняем буферы местами
                If direction = 1 Then
                    vertexSource = vertexesCulled
                    vertexTarget = vertexesCulledSwap
                    direction = 2
                Else
                    vertexSource = vertexesCulledSwap
                    vertexTarget = vertexesCulled
                    direction = 1
                End If
            Next m
            vertexCulledTop = vertexCount
            trianglesCulledTop = vertexCulledTop / 3
            For i = 0 To vertexCulledTop / 3 - 1
                TriangleCopy(triangles, trianglesCulled, vertexesCulled.triangleNum(i * 3), i)
            Next
            'перегоняет массив треугольников соотв. изменениям
        End Sub
        Private Sub PrepareFogTables()
            Const powerKoeff As Single = 3
            ReDim fogTable1(256 * 256)
            ReDim fogTable2(256 * 256)
            ReDim fogTable3(256 * 256)
            Dim f, w As Integer
            Dim wf As Single
            Dim fr, fg, fb As Byte
            fr = environment.fogColorR
            fb = environment.fogColorG
            fg = environment.fogColorB
            fogColorR = environment.fogColorR
            fogColorG = environment.fogColorG
            fogColorB = environment.fogColorB
            For f = 0 To 255
                For w = 0 To 255
                    wf = 255 * (w ^ powerKoeff / 255 ^ powerKoeff)
                    wf /= 255
                    fogTable1(f * 256 + w) = f * wf + fr * (1 - wf)
                    fogTable2(f * 256 + w) = f * wf + fg * (1 - wf)
                    fogTable3(f * 256 + w) = f * wf + fb * (1 - wf)
                Next
            Next
        End Sub
        ' Private Sub PrepareBilinearTable()
        '     ReDim bilinearTable1(256 * 256)
        ' Dim result As Single
        '  Dim x, y As Integer
        '      For x = 0 To 255
        '         For y = 0 To 255
        '             result = x * y * 0.00390625
        '            bilinearTable1(y * 256 + x) = result
        '         Next
        '    Next
        ' End Sub
        ''' <summary>
        ''' Рассчитывает таблицу дисторсии
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub PrepareDistortionTable()
            Dim koeffA As Single = environment.distortionA
            Dim koeffB As Single = environment.distortionB
            Dim width, height As Integer
            Dim rx, ry, r, rSqrt As Single
            Dim x, y, nx, ny As Integer
            Dim widthHalf, heightHalf As Integer
            width = myParent.width
            height = myParent.height
            widthHalf = width \ 2
            heightHalf = height \ 2
            ReDim distortionTable(width * height * 3)
            'коэффициент масштабирования и результаты с диагонали и перпендикуляров
            Dim scale, r1, r2, r3 As Single
            r1 = widthHalf
            r2 = heightHalf
            r3 = Math.Sqrt(widthHalf * widthHalf + heightHalf * heightHalf)
            r1 = (1 + koeffA * r1 ^ 2 + koeffB * r1 ^ 4)
            r2 = (1 + koeffA * r2 ^ 2 + koeffB * r2 ^ 4)
            r3 = (1 + koeffA * r3 ^ 2 + koeffB * r3 ^ 4)
            ' scale = r1
            ' If scale > r2 Then scale = r2
            'If scale > r3 Then scale = r3
            ' scale = 1 + scale
            ' scale = 1.6
            'scale = Math.Sqrt(scale)
            scale = environment.distortionScale
            For y = 0 To height - 1
                For x = 0 To width - 1
                    r = Math.Sqrt((x - widthHalf) * (x - widthHalf) + (y - heightHalf) * (y - heightHalf))
                    r = 1 / (1 + koeffA * r ^ 2 + koeffB * r ^ 4)
                    rSqrt = Math.Sqrt(r)
                    rx = widthHalf + (x - widthHalf) * rSqrt * scale
                    ry = heightHalf + (y - heightHalf) * rSqrt * scale
                    nx = rx
                    ny = ry
                    If nx < 0 Then nx = 0
                    If ny < 0 Then ny = 0
                    If nx > width - 1 Then nx = width - 1
                    If ny > height - 1 Then ny = height - 1
                    distortionTable(y * width * 3 + x * 3) = (ny * width * 3 + nx * 3)
                    distortionTable(y * width * 3 + x * 3 + 1) = (ny * width * 3 + nx * 3) + 1
                    distortionTable(y * width * 3 + x * 3 + 2) = (ny * width * 3 + nx * 3) + 2
                Next
            Next
            preparedDistortionTableA = koeffA
            preparedDistortionTableB = koeffB
            'ReDim tempPixels(width * height * 3)
        End Sub
        'выполняет преобразования экрана, если таковые необходимы
        Private Sub PostProcess()
            Dim pixels() As Byte = myParent.drawBuffer.pixels2
            Dim width, height, widthOffset, widthOffset3 As Integer
            width = myParent.width
            height = myParent.height
            Dim x, y As Integer
            Dim w As Integer
            Dim r, g, b As Byte
            Dim fog As Integer = environment.fog
            If fog > 100 Then fog = 100
            Dim fogKoeff As Integer = 101 - fog
            If fog > 0 Then
                If fogColorR <> environment.fogColorR Or fogColorG <> environment.fogColorG Or fogColorB <> environment.fogColorB Then PrepareFogTables()
                If environment.distortionA <> 0 Then
                    For y = 0 To height - 1
                        widthOffset = y * width
                        widthOffset3 = widthOffset * 3
                        For x = 0 To width - 1
                            w = (wBuffer(x + widthOffset) >> 15) * fogKoeff
                            If w > 255 Then w = 255
                            r = pixels(widthOffset3 + 3 * x + 0)
                            g = pixels(widthOffset3 + 3 * x + 1)
                            b = pixels(widthOffset3 + 3 * x + 2)
                            r = fogTable1(r * 256 + w) : g = fogTable1(g * 256 + w) : b = fogTable1(b * 256 + w)
                            tempPixels(widthOffset3 + 3 * x + 0) = r
                            tempPixels(widthOffset3 + 3 * x + 1) = g
                            tempPixels(widthOffset3 + 3 * x + 2) = b
                        Next
                    Next
                Else
                    For y = 0 To height - 1
                        widthOffset = y * width
                        widthOffset3 = widthOffset * 3
                        For x = 0 To width - 1
                            w = (wBuffer(x + widthOffset) >> 15) * fogKoeff
                            If w <> 0 Then
                                If w > 255 Then w = 255
                                If w < 0 Then w = 0
                                'w = 255
                                r = pixels(widthOffset3 + 3 * x + 2)
                                g = pixels(widthOffset3 + 3 * x + 1)
                                b = pixels(widthOffset3 + 3 * x + 0)
                                r = fogTable1(r * 256 + w) : g = fogTable2(g * 256 + w) : b = fogTable3(b * 256 + w)
                                pixels(widthOffset3 + 3 * x + 2) = r
                                pixels(widthOffset3 + 3 * x + 1) = g
                                pixels(widthOffset3 + 3 * x + 0) = b
                            Else
                                pixels(widthOffset3 + 3 * x + 2) = fogColorR
                                pixels(widthOffset3 + 3 * x + 1) = fogColorG
                                pixels(widthOffset3 + 3 * x + 0) = fogColorB
                            End If
                        Next
                    Next
                End If
            End If
            Dim i, gray As Integer
            Dim dr, dg, db As Integer
            Dim arraySize As Integer = width * height * 3 - 1
            Dim colorDown As Integer = environment.colorDown
            ' colorDown = 70
            If colorDown <> 0 Then
                If environment.distortionA <> 0 Then
                    For i = 0 To arraySize Step 3
                        r = pixels(i + 2)
                        g = pixels(i + 1)
                        b = pixels(i + 0)
                        gray = (r * 0.3 + g * 0.6 + b * 0.1)
                        dr = r - gray
                        dg = g - gray
                        db = b - gray
                        tempPixels(i + 2) = r - ((dr * colorDown) >> 7)
                        tempPixels(i + 1) = g - ((dg * colorDown) >> 7)
                        tempPixels(i + 0) = b - ((db * colorDown) >> 7)
                    Next
                Else
                    For i = 0 To arraySize Step 3
                        r = pixels(i + 2)
                        g = pixels(i + 1)
                        b = pixels(i + 0)
                        gray = (r * 0.3 + g * 0.6 + b * 0.1)
                        dr = r - gray
                        dg = g - gray
                        db = b - gray
                        pixels(i + 2) = r - ((dr * colorDown) >> 7)
                        pixels(i + 1) = g - ((dg * colorDown) >> 7)
                        pixels(i + 0) = b - ((db * colorDown) >> 7)
                    Next
                End If
            End If
            'tempPixels = pixels.Clone
            If environment.distortionA <> 0 Then
                If fog = 0 And colorDown = 0 Then
                    Array.Copy(pixels, tempPixels, pixels.GetUpperBound(0))
                End If
                If preparedDistortionTableA <> environment.distortionA Or preparedDistortionTableB <> environment.distortionB Then PrepareDistortionTable()
                For i = 0 To arraySize
                    pixels(i) = tempPixels(distortionTable(i))
                Next
            End If
        End Sub
        Private Shared Sub PrepareSQRTTable()
            If sqrtTable Is Nothing Then
                ReDim sqrtTable(sqrtTableSize)
                Dim i As Integer
                For i = 0 To sqrtTableSize
                    sqrtTable(i) = CInt(Math.Sqrt(i) * 16)
                Next
                sqrtTable(0) = 1
            End If
        End Sub
        Private Sub PrepareCameraVectors()
            ReDim cameraVectorsX((width + 1) * (height + 1))
            ReDim cameraVectorsY((width + 1) * (height + 1))
            ReDim cameraVectorsZ((width + 1) * (height + 1))
            Dim x, y As Integer
            Dim rx, ry, rz As Single
            Dim length As Single
            For y = 0 To height - 1
                For x = 0 To width - 1
                    rx = -(x - (width >> 1))
                    ry = (y - (height >> 1))
                    rz = -cameraDist
                    length = Math.Sqrt(rx * rx + ry * ry + rz * rz)
                    If length < 1 Then length = 1
                    rx /= length
                    ry /= length
                    rz /= length
                    cameraVectorsX(y * width + x) = CInt(rx * 1024)
                    cameraVectorsY(y * width + x) = CInt(ry * 1024)
                    cameraVectorsZ(y * width + x) = CInt(rz * 1024)
                Next
            Next
        End Sub
        ''' <summary>
        ''' Настраивает карту теней.
        ''' </summary>
        ''' <param name="index">Индекс карты.</param>
        ''' <param name="enabled">Используется карта или нет.</param>
        ''' <param name="x1"></param>
        ''' <param name="y1"></param>
        ''' <param name="x2"></param>
        ''' <param name="y2"></param>
        ''' <param name="axis">Перпендикулярно какой оси проецировать.</param>
        ''' <param name="distance">Расстояние по этой оси.</param>
        ''' <param name="scale">Коэффициент уменьшения.</param>
        ''' <remarks></remarks>
        Public Sub SetupShadowMap(ByVal index As Integer, ByVal enabled As Boolean, ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal axis As AxisType, ByVal distance As Integer, ByVal scale As Integer)
            Dim tmp As Integer
            With shadowPlanes(index - 1)
                .used = enabled
                If enabled Then
                    If scale = 0 Then scale = 1
                    If x2 < x1 Then tmp = x2 : x2 = x1 : x1 = tmp
                    If y2 < y1 Then tmp = y2 : y2 = y1 : y1 = tmp
                    .x1 = x1
                    .x2 = x2
                    .y1 = y1
                    .y2 = y2
                    .distanse = distance
                    .axis = axis
                    .scale = scale
                    If .map Is Nothing Then .map = New PixelSurface
                    .map.SetSizeNoSave((x2 - x1) \ scale, (y2 - y1) \ scale)
                Else
                    .map.SetSizeNoSave(1, 1)
                End If
            End With
        End Sub
        ''' <summary>
        ''' Очищает карты теней.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ClearShadows()
            Dim i As Integer
            For i = 0 To shadowPlanesCount - 1
                If shadowPlanes(i).used Then
                    shadowPlanes(i).map.Clear()
                End If
            Next
        End Sub
        ''' <summary>
        ''' Проецирует тень полигонов на поверхность.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CastShadows()
            Dim i As Integer
            Dim target As Byte, x1, y1 As Integer
            Dim vy1, vy2, vy3 As Integer
            Dim dist As Integer
            Dim scale As Integer, xSize, ySize As Integer
            Dim test1, test2, test3 As Integer
            Dim scrX1, scrY1 As Integer
            Dim scrX2, scrY2 As Integer
            Dim scrX3, scrY3 As Integer
            Dim pixels() As Byte
            Dim axis As AxisType
            Dim x, y, z As Integer
            Dim lx, ly, lz As Integer
            lx = lighterPoint.x
            ly = lighterPoint.y
            lz = lighterPoint.z
            'lx = 0
            ' ly = x
            ' lz = 200
            pixels = Nothing
            Dim lastTarget As Integer
            For i = 0 To trianglesTop - 1
                target = triangles.renderSettings(i).shadowsOn
                If target > 0 AndAlso shadowPlanes(target - 1).used Then
                    If lastTarget <> target Then
                        lastTarget = target
                        x1 = shadowPlanes(target - 1).x1 '>> 1
                        'x2 = shadowPlanes(target - 1).x2 >> 1
                        y1 = shadowPlanes(target - 1).y1 '>> 1
                        ' y2 = shadowPlanes(target - 1).y2
                        xSize = shadowPlanes(target - 1).map.Width
                        ySize = shadowPlanes(target - 1).map.Height
                        scale = shadowPlanes(target - 1).scale
                        axis = shadowPlanes(target - 1).axis
                        dist = shadowPlanes(target - 1).distanse
                        pixels = shadowPlanes(target - 1).map.pixels2
                    End If

                    Select Case axis
                        Case AxisType.y
                            vy1 = vertexes.pY(i * 3 + 0)
                            vy2 = vertexes.pY(i * 3 + 1)
                            vy3 = vertexes.pY(i * 3 + 2)
                            If vy1 <> ly AndAlso vy2 <> ly AndAlso vy3 <> ly Then
                                'scrX = x, scrY = z
                                x = vertexes.pX(i * 3 + 0) : y = vy1 : z = vertexes.pZ(i * 3 + 0)
                                scrX1 = (lx + (x - lx) * (dist - ly - Camera.PositionY) \ ((y - ly)) - x1) \ scale
                                scrY1 = (lz + (z - lz) * (dist - ly - Camera.PositionY) \ ((y - ly)) - y1) \ scale
                                test1 = (ly - y) * (ly - dist + Camera.PositionY)
                                x = vertexes.pX(i * 3 + 1) : y = vy2 : z = vertexes.pZ(i * 3 + 1)
                                scrX2 = (lx + (x - lx) * (dist - ly - Camera.PositionY) \ ((y - ly)) - x1) \ scale
                                scrY2 = (lz + (z - lz) * (dist - ly - Camera.PositionY) \ ((y - ly)) - y1) \ scale
                                test2 = (ly - y) * (ly - dist + Camera.PositionY)
                                x = vertexes.pX(i * 3 + 2) : y = vy3 : z = vertexes.pZ(i * 3 + 2)
                                scrX3 = (lx + (x - lx) * (dist - ly - Camera.PositionY) \ ((y - ly)) - x1) \ scale
                                scrY3 = (lz + (z - lz) * (dist - ly - Camera.PositionY) \ ((y - ly)) - y1) \ scale
                                test3 = (ly - y) * (ly - dist + Camera.PositionY)
                            Else
                                test1 = -1
                            End If
                    End Select
                    If test1 >= 0 AndAlso test2 >= 0 AndAlso test3 >= 0 Then
                        DrawShadowTriangle(scrX1, scrX2, scrX3, scrY1, scrY2, scrY3, pixels, xSize, ySize)
                    End If
                End If
            Next
        End Sub
        Private Sub DrawShadowTriangle(ByVal x1 As Integer, ByVal x2 As Integer, ByVal x3 As Integer, ByVal y1 As Integer, ByVal y2 As Integer, ByVal y3 As Integer, ByRef pixels() As Byte, ByVal width As Integer, ByVal height As Integer)
            Dim lx1, lx2, dlx1, dlx2 As Single
            Dim x, y As Integer
            Dim widthOffset As Integer
            Dim sx1, sx2 As Integer
            If y1 > y2 Then
                Swap(y1, y2)
                Swap(x1, x2)
            End If
            If y2 > y3 Then
                Swap(y2, y3)
                Swap(x2, x3)
            End If
            If y1 > y2 Then
                Swap(y1, y2)
                Swap(x1, x2)
            End If
            y1 -= 1
            y3 += 1
            If y3 > y1 Then
                'общие параметры для обоих частей
                If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                    dlx2 = (x3 - x1) / (y3 - y1)
                    lx2 = x1
                Else
                    dlx1 = (x3 - x1) / (y3 - y1)
                    lx1 = x1
                End If
                'рисуем верхнюю часть треугольника
                If y2 - y1 > 0 Then
                    If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                        dlx1 = (x2 - x1) / (y2 - y1)
                        lx1 = x1
                    Else
                        dlx2 = (x2 - x1) / (y2 - y1)
                        lx2 = x1
                    End If
                End If
                For y = y1 To y3
                    'проверяем. не перешли ли через границу Y2
                    If y = y2 Then
                        'рисуем нижнюю часть треугольника
                        If (x3 - x1) / (y3 - y1) > (x2 - x1) / (y2 - y1) Then
                            dlx1 = (x3 - x2) / (y3 - y2)
                            lx1 = x2
                        Else
                            dlx2 = (x3 - x2) / (y3 - y2)
                            lx2 = x2
                        End If
                    End If

                    'рисуем линию
                    If y >= 0 AndAlso y < height Then
                        sx1 = CInt(lx1) - 1 : sx2 = CInt(lx2) + 1
                        widthOffset = y * width * 3
                        If sx1 > width Then sx1 = width
                        If sx2 > width Then sx2 = width
                        If sx1 < 0 Then sx1 = 0
                        If sx2 < 0 Then sx2 = 0
                        For x = sx1 To sx2
                            If y > y2 Then pixels(widthOffset + x * 3 + 1) = 255
                            '   pixels(widthOffset + x * 3 + 2) = 255
                            pixels(widthOffset + x * 3) = 255
                        Next
                    End If
                    lx1 += dlx1
                    lx2 += dlx2
                Next y
            End If
        End Sub
        Private Sub Posterize()

        End Sub
    End Class
End Class
