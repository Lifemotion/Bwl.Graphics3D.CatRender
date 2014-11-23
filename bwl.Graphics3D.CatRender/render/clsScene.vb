''' <summary>
''' Представляет сцену целиком.
''' </summary>
''' <remarks></remarks>
Partial Public Class Scene
    Public Structure ScriptDataStruct
        Dim x, y, z As Single
        Dim x2, y2, z2 As Single
        Dim rx, ry, rz As Single
        Dim scale As Single
        Dim minScale, maxScale As Single
        Dim type As String
        Dim red, green, blue As Integer
        Dim range As Integer
        Dim attenuation1, attenuation2 As Single
        Dim distortionA, distortionB As Single
        Dim fov As Single
        Dim target As String
        Dim filename As String
        Dim name As String
        Dim texture As String
        Dim renderMode As RenderParameters
        Dim renderModeDefaults As RenderParameters
        'Dim renderModeClean As RenderParameters
        Dim texturing, lighting, normals, renderer, mipmaps As String
        Dim meshNumber As Integer
        Dim model As String
        Dim intense As Single
        Dim time As Single
        Dim hidden As Boolean
        Dim drawLines As Boolean
        Dim drawTriangles As Boolean
        Dim sortTriangles As Boolean
        Dim fastCulling As Boolean
        Dim shadowsOn As Integer
        Dim shadowedBy As Integer
        Dim distance As Integer
        Dim enabled As Boolean
        Dim index As Integer
        Dim axis As AxisType
        Dim startAlpha As Single
        Dim endAlpha As Single
        Dim font As String
        Dim text As String
        Dim skybox As Boolean
    End Structure
    Private ScriptThread As Threading.Thread
    Private Objects() As Object3D
    Private ObjectsDrawCopy() As Object3D
    Private Models() As Model
    Private Sprites() As Sprite
    Private Textures() As PixelSurface
    Private Movers() As Mover
    Private Texts() As TextMessage
    Private Fonts() As BitmapFont
    Private objectsCount As Integer
    Public camera As Camera3D
    Public render As Render3D
    Public scriptData As ScriptDataStruct
    Dim scriptNoWait As Boolean
    Dim scriptTimer As Timer
    Dim scriptWait As Boolean
    Dim scriptRunning As Boolean
    Dim scriptFileNumber As Integer
    Dim scriptLineNum As Integer
    Dim drawInProgress As New Object
    Dim fullDrawInProgress As New Object
    Sub New()
        ReDim Objects(0)
        ReDim ObjectsDrawCopy(0)
        ReDim Models(0)
        ReDim Sprites(0)
        ReDim Textures(0)
        ReDim Movers(0)
        ReDim Texts(0)
        ReDim Fonts(0)
        scriptTimer = New Timer
        AddHandler scriptTimer.Tick, AddressOf ScriptTimerTick
        render = New Render3D
        camera = New Camera3D
        'objectsCount = 0
        'libraryCount = 0
    End Sub
    Public Sub AddObject(ByRef object3d As Object3D)
        ReDim Preserve Objects(objectsCount)
        Objects(objectsCount) = object3d
        objectsCount += 1
    End Sub
    Public Overloads Function Create(ByRef model As Model) As Object3D
        Dim newObject As New Object3D
        newObject.model = model
        newObject.type = Object3DType.model
        AddObject(newObject)
        Return newObject
    End Function
    Public Overloads Function Create(ByRef sprite As Sprite) As Object3D
        Dim newObject As New Object3D
        newObject.sprite = sprite
        newObject.type = Object3DType.sprite
        AddObject(newObject)
        Return newObject
    End Function
    Public Overloads Function Create(ByRef lighter As Lighter) As Object3D
        Dim newObject As New Object3D
        newObject.light = lighter
        newObject.type = Object3DType.light
        AddObject(newObject)
        Return newObject
    End Function
    Public Overloads Function FindObject(ByVal objectName As String) As Object3D
        Dim i As Integer
        For i = 0 To objectsCount - 1
            If Objects(i).name = objectName Then Return Objects(i)
        Next
        Return Nothing
    End Function
    Public Overloads Function FindObject(ByVal objectUID As Integer) As Object3D
        Dim i As Integer
        For i = 0 To objectsCount - 1
            If Objects(i).UID = objectUID Then Return Objects(i)
        Next
        Return Nothing
    End Function
    Public Function FindModel(ByVal modelName As String) As Model
        Dim i As Integer
        For i = 1 To Models.GetUpperBound(0)
            If Models(i).name = modelName Then Return Models(i)
        Next
        Return Nothing
    End Function
    Public Function FindTexture(ByVal textureName As String) As PixelSurface
        Dim i As Integer
        For i = 1 To Textures.GetUpperBound(0)
            If Textures(i).name = textureName Then Return Textures(i)
        Next
        Return Nothing
    End Function
    Public Function FindFont(ByVal fontName As String) As BitmapFont
        Dim i As Integer
        For i = 1 To Fonts.GetUpperBound(0)
            If Fonts(i).name = fontName Then Return Fonts(i)
        Next
        Return Nothing
    End Function
    Public Function FindSprite(ByVal spriteName As String) As Sprite
        Dim i As Integer
        For i = 1 To Sprites.GetUpperBound(0)
            If Sprites(i).name = spriteName Then Return Sprites(i)
        Next
        Return Nothing
    End Function
    Public Function DeleteObject(ByRef object3d As Object3D) As Boolean
        Dim i As Integer
        For i = 0 To objectsCount - 1
            If Object.Equals(Objects(i), object3d) Then
                Objects(i) = Objects(objectsCount - 1)
                objectsCount -= 1
                ReDim Preserve Objects(objectsCount - 1)
                Return True
            End If
        Next
        Return False
    End Function
    ''' <summary>
    ''' Выполняет очистку буфера, рендера, рисование всех объектов, рисование спрайтов, отображение.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FullDraw()
        SyncLock fullDrawInProgress
            If Not (render Is Nothing) Then
                render.Clear()
                render.SceneDraw.Clear()
                Draw()
                render.SceneDraw.Render()
                Draw2D()
                render.Present()
            End If
        End SyncLock
    End Sub
    ''' <summary>
    ''' Передает все трехмерные объекты в рендер. Перед этим нужно очистить экран и рендер,
    ''' а затем запустить рендеринг и отображение отображение на экран в рендере.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Draw()
        SyncLock drawInProgress
            Try
                Dim i As Integer
                If Not (render Is Nothing) Then
                    render.SceneDraw.Camera = camera
                    Dim count As Integer = objectsCount
                    ReDim ObjectsDrawCopy(count - 1)
                    For i = 0 To count - 1
                        ObjectsDrawCopy(i) = Objects(i)
                    Next
                    For i = 0 To count - 1
                        render.SceneDraw.DrawObject(ObjectsDrawCopy(i))
                    Next
                End If
            Catch ex As Exception
            End Try
        End SyncLock
    End Sub
    ''' <summary>
    ''' Выполняет рисование двумерных объектов сцены. Объекты рисуются сразу в экранный буфер.
    ''' Вызывать после трехмерных объектов.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Draw2D()
        SyncLock drawInProgress
            Dim i As Integer
            For i = 1 To Texts.GetUpperBound(0)
                If Texts(i).Active Then render.DirectDraw.DrawString(Texts(i))
            Next
        End SyncLock
    End Sub
    Public Sub LoadModel(ByVal name As String, ByVal filename As String)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

        ReDim Preserve Models(Models.GetUpperBound(0) + 1)
        Models(Models.GetUpperBound(0)) = New Model
        With Models(Models.GetUpperBound(0))
            .LoadFromX(filename)
            .ComputeNormals()
            .MakeMipmaps()
            .Prepare()
            .name = name
        End With
    End Sub
    Public Sub LoadFont(ByVal name As String, ByVal filename As String)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

        ReDim Preserve Fonts(Fonts.GetUpperBound(0) + 1)
        Fonts(Fonts.GetUpperBound(0)) = New BitmapFont
        With Fonts(Fonts.GetUpperBound(0))
            .name = name
            .LoadBitmap(filename)
            If Not .Loaded Then Throw New Exception
        End With
    End Sub
    Public Sub AddModel(ByVal model As Model, Optional ByVal newName As String = "")
        ReDim Preserve Models(Models.GetUpperBound(0) + 1)
        If newName > "" Then model.name = newName
        Models(Models.GetUpperBound(0)) = model
    End Sub
    Public Sub LoadTexture(ByVal name As String, ByVal filename As String)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

        ReDim Preserve Textures(Textures.GetUpperBound(0) + 1)
        Textures(Textures.GetUpperBound(0)) = New PixelSurface
        With Textures(Textures.GetUpperBound(0))
            .LoadFromFile(filename)
            .name = name
        End With
    End Sub
    Public Function SetSprite(ByVal name As String, ByVal textureName As String, ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal scale As Single, ByVal minimumScale As Single, ByVal maximumScale As Single) As Boolean
        Dim i As Integer
        For i = 1 To Textures.GetUpperBound(0)
            If Textures(i).name.ToUpper = textureName.ToUpper Then
                ReDim Preserve Sprites(Sprites.GetUpperBound(0) + 1)
                Sprites(Sprites.GetUpperBound(0)) = New Sprite
                With Sprites(Sprites.GetUpperBound(0))
                    .left = x1
                    .right = x2
                    .bottom = y2
                    .top = y1
                    .texture = Textures(i)
                    .name = name
                    .maximumScale = maximumScale
                    .minimumScale = minimumScale
                    .scale = scale
                End With
                Return True
            End If
        Next
        Return False
    End Function
    Public Sub ExecScript(ByVal filename As String)

        filename = filename.Replace("\", IO.Path.DirectorySeparatorChar)

        If Not scriptRunning Then
            scriptLineNum = 0
            scriptNoWait = False
            Try
                scriptFileNumber = FreeFile()
                FileOpen(scriptFileNumber, filename, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)
                scriptRunning = True
                LoadScriptLine()
            Catch ex As Exception
                MsgBox("Скрипт открыть не удалось! " + filename)
            End Try
        Else
            MsgBox("Скрипт уже выполняется!")
        End If
    End Sub
    Private Sub LoadScriptLine()
        scriptWait = False
        Do While Not EOF(scriptFileNumber) And Not scriptWait
            Dim tmpLine As String = ""
            tmpLine = LineInput(scriptFileNumber)
            scriptLineNum += 1
            InterpreteLine(tmpLine, scriptLineNum)
        Loop

        If EOF(scriptFileNumber) Then
            If scriptRunning = True Then
                scriptRunning = False
                FileClose(scriptFileNumber)
                RaiseEvent ScriptFinished()
            End If
        End If
    End Sub
    Private Sub ScriptTimerTick()
        scriptTimer.Enabled = False
        LoadScriptLine()
    End Sub
    Public Function NewMover() As Mover
        ReDim Preserve Movers(Movers.GetUpperBound(0) + 1)
        Movers(Movers.GetUpperBound(0)) = New Mover()
        Return Movers(Movers.GetUpperBound(0))
    End Function
    Public Function NewText() As TextMessage
        ReDim Preserve Texts(Texts.GetUpperBound(0) + 1)
        Texts(Texts.GetUpperBound(0)) = New TextMessage()
        Return Texts(Texts.GetUpperBound(0))
    End Function
    Public Sub Tick()
        MoversTick()
        TextTick()
        ' Debug.Print(camera.PositionX.ToString + " " + camera.PositionY.ToString)
    End Sub
    Private Sub MoversTick()
        Dim i As Integer
        For i = 1 To Movers.GetUpperBound(0)
            Movers(i).Tick()
            ' Do While Movers(i).Finished
            'Movers(i) = Movers(Movers.GetUpperBound(0))
            '  ReDim Preserve Movers(Movers.GetUpperBound(0) - 1)
            '  Loop
        Next
    End Sub
    Private Sub TextTick()
        Dim i As Integer
        For i = 1 To Texts.GetUpperBound(0)
            Texts(i).Tick()
            ' Do While Movers(i).Finished
            'Movers(i) = Movers(Movers.GetUpperBound(0))
            '  ReDim Preserve Movers(Movers.GetUpperBound(0) - 1)
            '  Loop
        Next
    End Sub
    Public Event ScriptCommand(ByVal line As String)
    Public Event ScriptFinished()

    Private Sub InterpreteLine(ByRef line As String, ByVal lineNum As Integer)
        With scriptData
            Dim i As Integer, parameter As String, value As String
            Dim errors As String = ""
            If lineNum = 1 Or line.ToUpper = "!DEFAULTS" Then
                .x = 0 : .y = 0 : .z = 0
                .x2 = 0 : .y2 = 0 : .z2 = 0
                .rx = 0 : .ry = 0 : .rz = 0
                .scale = 1 : .minScale = 0
                .range = 0 : .intense = 0
                .red = 0 : .green = 0 : .blue = 0
                .attenuation1 = 0 : .attenuation2 = 0
                .distortionA = 0 : .distortionA = 0
                .filename = "" : .name = "" : .texture = ""
                .meshNumber = -1
                .fov = 150
                .texturing = "" : .lighting = "" : .normals = "" : .mipmaps = "" : .renderer = ""
                .model = "" : .time = 0
                .hidden = False
                .drawLines = False
                .drawTriangles = True
                .sortTriangles = True
                .fastCulling = False
                .shadowsOn = 0
                .index = 1
                .distance = 0
                .shadowedBy = 0
                .axis = AxisType.y
                .enabled = False
                .maxScale = 1000
                .startAlpha = 1
                .endAlpha = 0
                .text = ""
                .font = ""
            End If
            i = InStr(line, "=")
            If i > 0 Then
                parameter = Mid(line, 1, i - 1).ToLower
                value = Mid(line, i + 1)
                Select Case parameter
                    Case "x" : .x = Val(value)
                    Case "y" : .y = Val(value)
                    Case "z" : .z = Val(value)
                    Case "dx" : .x2 = Val(value)
                    Case "dy" : .y2 = Val(value)
                    Case "dz" : .z2 = Val(value)
                    Case "x1" : .x = Val(value)
                    Case "y1" : .y = Val(value)
                    Case "z1" : .z = Val(value)
                    Case "x2" : .x2 = Val(value)
                    Case "y2" : .y2 = Val(value)
                    Case "z2" : .z2 = Val(value)
                    Case "rx" : .rx = Val(value)
                    Case "ry" : .ry = Val(value)
                    Case "rz" : .rz = Val(value)
                    Case "scale" : .scale = Val(value)
                    Case "minscale" : .minScale = Val(value)
                    Case "maxscale" : .maxScale = Val(value)
                    Case "type" : .type = value
                    Case "range" : .range = Val(value)
                    Case "red" : .red = Val(value)
                    Case "green" : .green = Val(value)
                    Case "blue" : .blue = Val(value)
                    Case "index" : .index = Val(value)
                    Case "attenuationa" : .attenuation1 = Val(value)
                    Case "attenuationb" : .attenuation2 = Val(value)
                    Case "distortiona" : .distortionA = Val(value)
                    Case "distortionb" : .distortionB = Val(value)
                    Case "filename" : .filename = value
                    Case "name" : .name = value
                    Case "texture" : .texture = value
                    Case "target" : .target = value
                    Case "meshnumber" : .meshNumber = Val(value)
                    Case "texturing" : .texturing = value
                    Case "lighting" : .lighting = value
                    Case "normals" : .normals = value
                    Case "renderer" : .renderer = value
                    Case "fov" : .fov = Val(value)
                    Case "startalpha" : .startAlpha = Val(value)
                    Case "endalpha" : .endAlpha = Val(value)
                    Case "mipmaps" : .mipmaps = value
                    Case "model" : .model = value
                    Case "text" : .text = value
                    Case ("font") : .font = value
                    Case "time" : .time = Val(value)
                    Case "intense" : .intense = Val(value)
                    Case "distance" : .distance = Val(value)
                    Case "shadowson" : .shadowsOn = Val(value)
                    Case "shadowedby" : .shadowedBy = Val(value)
                    Case "enabled" : If Val(value) = 1 Then .enabled = True Else .enabled = False
                    Case "hidden" : If Val(value) = 1 Then .hidden = True Else .hidden = False
                    Case "drawlines" : If Val(value) = 1 Then .drawLines = True Else .drawLines = False
                    Case "drawtriangles" : If Val(value) = 1 Then .drawTriangles = True Else .drawTriangles = False
                    Case "sorttriangles" : If Val(value) = 1 Then .sortTriangles = True Else .sortTriangles = False
                    Case "fastculling" : If Val(value) = 1 Then .fastCulling = True Else .fastCulling = False
                    Case "skybox" : If Val(value) = 1 Then .skybox = True Else .skybox = False

                End Select
            Else

                If line > "" AndAlso line(0) = "!" Then
                    line = Mid(line, 2).ToUpper
                    Dim newObject As Object3D
                    Dim newModel As Model
                    ' Dim newTexture As PixelSurface
                    'Dim newSprite As Sprite
                    Dim newLighter As Lighter
                    Select Case line
                        Case "LOADMODEL"
                            If .name > "" Then
                                Try
                                    LoadModel(.name, Application.StartupPath + "" + IO.Path.DirectorySeparatorChar + .filename)
                                Catch ex As Exception
                                    errors += "Модель не загрузилась! " + lineNum.ToString + vbCr
                                End Try
                            Else
                                errors += "Для модели не указано имя! " + lineNum.ToString + vbCr
                            End If
                        Case "LOADTEXTURE"
                            If .name > "" Then
                                Try
                                    LoadTexture(.name, Application.StartupPath + "" + IO.Path.DirectorySeparatorChar + .filename)
                                Catch ex As Exception
                                    errors += "Текстура не загрузилась! " + lineNum.ToString + vbCr
                                End Try
                            Else
                                errors += "Для текстуры не указано имя! " + lineNum.ToString + vbCr
                            End If
                        Case "DEFINESPRITE"
                            If .name > "" Then
                                If SetSprite(.name, .texture, .x, .y, .x2, .y2, .scale, .minScale, .maxScale) = False Then
                                    errors += "Для спрайта не найдена текстура! " + lineNum.ToString + vbCr
                                End If
                            Else
                                errors += "Для спрайта не указано имя! " + lineNum.ToString + vbCr
                            End If
                        Case "SETCAMERA"
                            camera.FOV = .fov
                            camera.Pitch = .rx
                            camera.Roll = .rz
                            camera.Yaw = .ry
                            camera.PositionX = .x
                            camera.PositionY = .y
                            camera.PositionZ = .z
                        Case "PLACEMODEL"
                            newModel = FindModel(.model)
                            If newModel Is Nothing Then
                                errors += "Не найдена указанная модель! " + lineNum.ToString + vbCr
                            Else
                                .renderMode = .renderModeDefaults
                                If .texturing.ToUpper = "DEFAULT" Then .renderMode.texturing = TexturingMode.byRender
                                If .texturing.ToUpper = "NORMAL" Then .renderMode.texturing = TexturingMode.normal
                                If .texturing.ToUpper = "BILINEAR" Then .renderMode.texturing = TexturingMode.bilinear
                                If .texturing.ToUpper = "NOWHITE" Then .renderMode.texturing = TexturingMode.transparent
                                If .lighting.ToUpper = "DEFAULT" Then .renderMode.lighting = LightingMode.byRender
                                If .lighting.ToUpper = "FULL" Then .renderMode.lighting = LightingMode.full
                                If .lighting.ToUpper = "NONE" Then .renderMode.lighting = LightingMode.none
                                If .lighting.ToUpper = "NORMAL" Then .renderMode.lighting = LightingMode.full
                                If .lighting.ToUpper = "AMBIENT" Then .renderMode.lighting = LightingMode.ambientOnly
                                If .normals.ToUpper = "DEFAULT" Then .renderMode.normals = NormalsInterpolationMode.byRender
                                If .normals.ToUpper = "TRIANGLE" Then .renderMode.normals = NormalsInterpolationMode.oneForTriangle
                                If .normals.ToUpper = "VERTEX" Then .renderMode.normals = NormalsInterpolationMode.interpolateByMesh
                                If .mipmaps.ToUpper = "DEFAULT" Then .renderMode.mipMap = MipMapMode.byRender
                                If .mipmaps.ToUpper = "ON" Then .renderMode.mipMap = MipMapMode.mipMapOn
                                If .mipmaps.ToUpper = "OFF" Then .renderMode.mipMap = MipMapMode.mipMapOff
                                If .renderer.ToUpper = "DEFAULT" Then .renderMode.renderer = mdlStructures.RenderMode.byRender
                                If .renderer.ToUpper = "FAST" Then .renderMode.renderer = mdlStructures.RenderMode.fast
                                If .renderer.ToUpper = "NORMAL" Then .renderMode.renderer = mdlStructures.RenderMode.normal
                                .renderMode.shadowsOn = .shadowsOn
                                .renderMode.shadowedBy = .shadowedBy
                                .renderMode.isSkybox = .skybox
                                ' texturing = "" : lighting = "" : normals = "" : mipmaps = "" : renderer = ""
                                newObject = Create(newModel)
                                With newObject
                                    .positionX = scriptData.x
                                    .positionY = scriptData.y
                                    .positionZ = scriptData.z
                                    .modelMesh = scriptData.meshNumber
                                    .name = scriptData.name
                                    .rotateX = scriptData.rx
                                    .rotateY = scriptData.ry
                                    .rotateZ = scriptData.rz
                                    .scale = scriptData.scale
                                    .renderMode = scriptData.renderMode
                                End With
                            End If
                        Case "UPDATEMODEL"
                            Dim objectModel As Object3D = FindObject(.name)
                            If Not objectModel Is Nothing AndAlso objectModel.type <> Object3DType.model Then objectModel = Nothing
                            If objectModel Is Nothing Then
                                errors += "Не найден меняемый объект! " + lineNum.ToString + vbCr
                            Else
                                .renderMode = .renderModeDefaults
                                If .texturing.ToUpper = "DEFAULT" Then .renderMode.texturing = TexturingMode.byRender
                                If .texturing.ToUpper = "NORMAL" Then .renderMode.texturing = TexturingMode.normal
                                If .texturing.ToUpper = "BILINEAR" Then .renderMode.texturing = TexturingMode.bilinear
                                If .texturing.ToUpper = "NOWHITE" Then .renderMode.texturing = TexturingMode.transparent
                                If .lighting.ToUpper = "DEFAULT" Then .renderMode.lighting = LightingMode.byRender
                                If .lighting.ToUpper = "FULL" Then .renderMode.lighting = LightingMode.full
                                If .lighting.ToUpper = "NONE" Then .renderMode.lighting = LightingMode.none
                                If .lighting.ToUpper = "NORMAL" Then .renderMode.lighting = LightingMode.full
                                If .lighting.ToUpper = "AMBIENT" Then .renderMode.lighting = LightingMode.ambientOnly
                                If .normals.ToUpper = "DEFAULT" Then .renderMode.normals = NormalsInterpolationMode.byRender
                                If .normals.ToUpper = "TRIANGLE" Then .renderMode.normals = NormalsInterpolationMode.oneForTriangle
                                If .normals.ToUpper = "VERTEX" Then .renderMode.normals = NormalsInterpolationMode.interpolateByMesh
                                If .mipmaps.ToUpper = "DEFAULT" Then .renderMode.mipMap = MipMapMode.byRender
                                If .mipmaps.ToUpper = "ON" Then .renderMode.mipMap = MipMapMode.mipMapOn
                                If .mipmaps.ToUpper = "OFF" Then .renderMode.mipMap = MipMapMode.mipMapOff
                                If .renderer.ToUpper = "DEFAULT" Then .renderMode.renderer = mdlStructures.RenderMode.byRender
                                If .renderer.ToUpper = "FAST" Then .renderMode.renderer = mdlStructures.RenderMode.fast
                                If .renderer.ToUpper = "NORMAL" Then .renderMode.renderer = mdlStructures.RenderMode.normal
                                .renderMode.shadowsOn = .shadowsOn
                                .renderMode.shadowedBy = .shadowedBy
                                ' texturing = "" : lighting = "" : normals = "" : mipmaps = "" : renderer = ""
                                With objectModel
                                    .positionX = scriptData.x
                                    .positionY = scriptData.y
                                    .positionZ = scriptData.z
                                    .modelMesh = scriptData.meshNumber
                                    .name = scriptData.name
                                    .rotateX = scriptData.rx
                                    .rotateY = scriptData.ry
                                    .rotateZ = scriptData.rz
                                    .scale = scriptData.scale
                                    .renderMode = scriptData.renderMode
                                End With
                            End If
                        Case "PLACELIGHT"
                            newLighter = New Lighter
                            newObject = Create(newLighter)
                            With newLighter
                                .colorR = scriptData.red
                                .colorG = scriptData.green
                                .colorB = scriptData.blue
                                .intense = scriptData.intense
                                .attenutionA = scriptData.attenuation1
                                .attenutionB = scriptData.attenuation2
                                If scriptData.type.ToUpper = "POINT" Then .type = LighterTypeEnum.point
                                If scriptData.type.ToUpper = "AMBIENT" Then .type = LighterTypeEnum.ambient
                                If scriptData.type.ToUpper = "SIMPLE" Then .type = LighterTypeEnum.simple
                                If scriptData.type.ToUpper = "SKY" Then .type = LighterTypeEnum.sky
                            End With
                            With newObject
                                .positionX = scriptData.x
                                .positionY = scriptData.y
                                .positionZ = scriptData.z
                                .rotateX = scriptData.rx
                                .rotateY = scriptData.ry
                                .rotateZ = scriptData.rz
                                .name = scriptData.name
                            End With
                        Case "SETEFFECT"
                            If scriptData.type.ToUpper = "FOG" Then
                                render.SceneDraw.environment.fog = scriptData.intense
                                render.SceneDraw.environment.fogColorR = scriptData.red
                                render.SceneDraw.environment.fogColorG = scriptData.green
                                render.SceneDraw.environment.fogColorB = scriptData.blue
                            End If
                            If scriptData.type.ToUpper = "DISTORTION" Then
                                render.SceneDraw.environment.distortionA = scriptData.distortionA
                                render.SceneDraw.environment.distortionB = scriptData.distortionB
                                render.SceneDraw.environment.distortionScale = scriptData.scale
                            End If
                            If scriptData.type.ToUpper = "COLORDOWN" Then
                                render.SceneDraw.environment.colorDown = .intense
                            End If
                        Case "SETRENDERDEFAULTS"
                            If scriptData.texturing.ToUpper = "NORMAL" Then render.SceneDraw.defaults.texturing = TexturingMode.normal
                            If scriptData.texturing.ToUpper = "BILINEAR" Then render.SceneDraw.defaults.texturing = TexturingMode.bilinear
                            If scriptData.lighting.ToUpper = "FULL" Then render.SceneDraw.defaults.lighting = LightingMode.full
                            If scriptData.lighting.ToUpper = "NONE" Then render.SceneDraw.defaults.lighting = LightingMode.none
                            If scriptData.lighting.ToUpper = "NORMAL" Then render.SceneDraw.defaults.lighting = LightingMode.full
                            If scriptData.lighting.ToUpper = "AMBIENT" Then render.SceneDraw.defaults.lighting = LightingMode.ambientOnly
                            If scriptData.normals.ToUpper = "TRIANGLE" Then render.SceneDraw.defaults.normals = NormalsInterpolationMode.oneForTriangle
                            If scriptData.normals.ToUpper = "VERTEX" Then render.SceneDraw.defaults.normals = NormalsInterpolationMode.interpolateByMesh
                            If scriptData.mipmaps.ToUpper = "ON" Then render.SceneDraw.defaults.mipMap = MipMapMode.mipMapOn
                            If scriptData.mipmaps.ToUpper = "OFF" Then render.SceneDraw.defaults.mipMap = MipMapMode.mipMapOff
                            If scriptData.renderer.ToUpper = "FAST" Then render.SceneDraw.defaults.renderer = mdlStructures.RenderMode.fast
                            If scriptData.renderer.ToUpper = "NORMAL" Then render.SceneDraw.defaults.renderer = mdlStructures.RenderMode.normal
                        Case "SETRENDERSETTINGS"
                            render.SceneDraw.settings.drawLines = scriptData.drawLines
                            render.SceneDraw.settings.drawTriangles = scriptData.drawTriangles
                            render.SceneDraw.settings.sortTriangles = scriptData.sortTriangles
                            render.SceneDraw.settings.useFastCulling = scriptData.fastCulling
                        Case "SETSHADOWMAP"
                            Try
                                render.SceneDraw.SetupShadowMap(.index, .enabled, .x, .y, .x2, .x2, .axis, .distance, .scale)
                            Catch ex As Exception
                                errors += "Ошибка при настройке карты тени! " + lineNum.ToString + vbCr
                            End Try
                        Case "ADDMOVER"
                            Dim object3d As Object3D
                            Dim needAdd As Boolean
                            Dim mover As Mover = Nothing
                            If .target.ToUpper = "CAMERA" Then
                                mover = NewMover()
                                mover.CaptureSource(Me.camera)
                                needAdd = True
                            Else
                                object3d = FindObject(.target)
                                If object3d Is Nothing Then
                                    errors += "Не найдена модель при добавлении перемещателя! " + lineNum.ToString + vbCr
                                Else
                                    mover = NewMover()
                                    mover.CaptureSource(object3d)
                                    needAdd = True
                                End If
                            End If
                            If needAdd Then
                                'удаляем старые муверы
                                Dim j As Integer
                                Application.DoEvents()
                                For j = 1 To Movers.GetUpperBound(0)
                                    If Movers(j).name = .target.ToUpper Then Movers(j).Finish()
                                Next
                                Dim difference As Boolean = False
                                mover.name = .target.ToUpper
                                If InStr(.type.ToUpper, "DIFF") > 0 Then difference = True
                                If InStr(.type.ToUpper, "PC") > 0 Then
                                    mover.SetTargetPC(.x, .y, .z, .red, .green, .blue, .time, difference)
                                Else
                                    mover.SetTargetPR(.x, .y, .z, .rx, .ry, .rz, .time, difference)
                                End If
                            End If
                        Case "DELETEOBJECT"
                            Dim obj3d As Object3D
                            Do
                                obj3d = FindObject(.name)
                                If Not obj3d Is Nothing Then
                                    DeleteObject(obj3d)
                                    ' Else
                                    '     errors += "Объект удаления не найден!" + lineNum.ToString + vbCr
                                End If
                            Loop While Not obj3d Is Nothing
                        Case "WAIT"
                            If Not scriptNoWait Then
                                scriptWait = True
                                scriptTimer.Enabled = False
                                scriptTimer.Interval = (.time * 1000)
                                scriptTimer.Enabled = True
                            End If
                        Case "DISABLEWAITS"
                            scriptNoWait = True
                        Case "ENABLEWAITS"
                            scriptNoWait = False
                        Case "LOADFONT"
                            If .name > "" Then
                                Try
                                    LoadFont(.name, Application.StartupPath + "" + IO.Path.DirectorySeparatorChar + .filename)
                                Catch ex As Exception
                                    errors += "Шрифт не загрузился! " + lineNum.ToString + vbCr
                                End Try
                            Else
                                errors += "Для шрифта не указано имя! " + lineNum.ToString + vbCr
                            End If
                        Case "ADDTEXT"
                            If .text > "" Then
                                Dim newTextO As TextMessage = NewText()
                                Dim font As BitmapFont = FindFont(.font)
                                If Not font Is Nothing Then
                                    newTextO.red = .red
                                    newTextO.green = .green
                                    newTextO.blue = .blue
                                    newTextO.startAlpha = .startAlpha
                                    newTextO.endAlpha = .endAlpha
                                    newTextO.font = font
                                    newTextO.text = .text
                                    newTextO.Start(.x, .y, .x2, .y2, .time)
                                Else
                                    errors += "А шрифт-то и не найден, ха-ха-ха!" + lineNum.ToString + vbCr
                                End If
                            End If
                        Case Else
                            RaiseEvent ScriptCommand(line)
                    End Select
                End If
            End If
            If errors > "" Then
                MsgBox(errors)
            End If
        End With
    End Sub
    Public Function TestIntersect(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal sizeX As Single, ByVal sizeY As Single, ByVal sizeZ As Single) As Boolean
        Dim i As Integer, cnt As Integer = 0
        For i = 1 To objectsCount - 1
            With Objects(i)
                If .type = Object3DType.model Then
                    If Abs(.positionX - x) < .model.boundX + sizeX Then cnt += 1
                    If Abs(.positionY - y) < .model.boundY + sizeY Then cnt += 1
                    If Abs(.positionZ - z) < .model.boundZ + sizeZ Then cnt += 1
                    If cnt = 3 Then Return True Else cnt = 0
                End If
            End With
        Next
        Return False
    End Function
    Function GetDistanse(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal target As Object3D) As Single
        Return Math.Sqrt((target.positionX - x) * (target.positionX - x) + (target.positionY - y) * (target.positionY - y) + (target.positionZ - z) * (target.positionZ - z))
    End Function
    Private Function Abs(ByVal value As Single) As Single
        If value > 0 Then Return value Else Return -value
    End Function
    Public Sub Init()
        render.Init()
    End Sub


End Class
