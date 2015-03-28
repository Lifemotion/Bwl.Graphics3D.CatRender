Imports System.Threading
Public Class CatRender
    Inherits UserControl
    Private _scene As New Scene
    Private _drawThread As Threading.Thread
    Private _workThread As Threading.Thread
    Private _syncRoot As New Object
    Private _autoMove As Boolean
    Private _autoMoveStep As Single = 10
    Public Property RenderAutoMove() As Boolean
        Get
            Return _autoMove
        End Get
        Set(ByVal value As Boolean)
            _autoMove = value
        End Set
    End Property
    Private Sub InitGraphics()
        _scene.render.presentSettings.TargetType = OutputTargetTypeEnum.lastBitmapOnly
        Dim width = Math.Floor(Display.Width \ 4) * 4
        _scene.render.presentSettings.WindowWidth = width
        _scene.render.presentSettings.WindowHeight = Display.Height
        _scene.Init()
        _scene.render.Present()
        Display.Bitmap = _scene.render.lastPresentedBitmap
        Display.RefreshBitmap()
    End Sub
    ''' <summary>
    ''' Проводится ли отрисовка очередных кадров.
    ''' Запущен ли рисующий поток.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RenderDrawingEnabled() As Boolean
        Get
            If _drawThread Is Nothing Then Return False
            Return _drawThread.IsAlive
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                'надо включить рисование
                If _drawThread Is Nothing OrElse _drawThread.IsAlive = False Then
                    _drawThread = New Thread(AddressOf DrawThreadSub)
                    _drawThread.Priority = ThreadPriority.BelowNormal
                    _drawThread.IsBackground = True
                    _drawThread.Name = "DrawThreadCR"
                    _drawThread.Start()
                    Display.RefreshBitmap()
                End If
            Else
                'надо выключить
                If _drawThread Is Nothing Then Return
                If _drawThread.IsAlive = True Then _drawThread.Abort()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Включен ли "рабочий" поток, в котором удобно работать со сценой с заданной периодичностью.
    ''' Если поток включен, будет срабатывать событие WorkCycle.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RenderWorkingEnabled() As Boolean
        Get
            If _workThread Is Nothing Then Return False
            Return _workThread.IsAlive
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                'надо включить рисование
                If _workThread Is Nothing OrElse _drawThread.IsAlive = False Then
                    _workThread = New Thread(AddressOf WorkThreadSub)
                    _workThread.Priority = ThreadPriority.Normal
                    _workThread.IsBackground = True
                    _workThread.Name = "WorkThread(CR)"
                    _workThread.Start()
                End If
            Else
                'надо выключить
                If _workThread Is Nothing Then Return
                If _workThread.IsAlive = True Then _workThread.Abort()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Здесь происходит рисование.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DrawThreadSub()
        Do
            Thread.Sleep(20)
            If _scene.render.width > 0 Then
                '      Try
                SyncLock _syncRoot
                    _scene.FullDraw()
                End SyncLock
                Display.RefreshBitmap()
                '  Catch ex As Exception
                'If Not SuppressErrorsInDrawThread Then Throw ex
                '  End Try
            End If
        Loop
    End Sub

    ' Public Property SuppressErrorsInDrawThread As Boolean = True

    Private Sub CatRenders_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitGraphics()
    End Sub

    Private Sub CatRenders_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Invalidate()
    End Sub
    ''' <summary>
    ''' Сцена - набор камеры, объектов, осветителей, моделей.
    ''' Работа с ними, работа со скриптами.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RenderScene() As Scene
        Get
            Return _scene
        End Get
    End Property
    ''' <summary>
    ''' Отрисовщик в целом.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Render() As Render3D
        Get
            Return _scene.render
        End Get
    End Property
    ''' <summary>
    ''' Трехмерная часть отрисовщика.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RenderSceneDraw() As Render3D.SceneDrawClass
        Get
            Return _scene.render.SceneDraw
        End Get
    End Property
    ''' <summary>
    ''' Объект синхронизации. При работе со сценой, камерой, рендером обязательно 
    ''' синхронизируйтесь по нему.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RenderSyncRoot() As Object
        Get
            Return _syncRoot
        End Get
    End Property
    ''' <summary>
    ''' Камера сцены.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RenderCamera() As Camera3D
        Get
            Return _scene.camera
        End Get
        Set(ByVal value As Camera3D)
            SyncLock _syncRoot
                _scene.camera = RenderCamera
            End SyncLock
        End Set
    End Property
    ''' <summary>
    ''' Срабатывает регулярно в рабочем цикле, если он включен (WorkingEnabled).
    ''' Обрабатывайте здесь события клавиатуры, мыши и работайте со сценой.
    ''' На время обработки поток отрисовки блокируется.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event RenderWorkCycle()
    Private Sub WorkThreadSub()
        Do
            SyncLock _syncRoot
                If _autoMove Then
                    If Display.PressedKeys(Keys.Up) Then _scene.camera.Move(_autoMoveStep, 0)
                    If Display.PressedKeys(Keys.Down) Then _scene.camera.Move(-_autoMoveStep, 0)
                    If Display.PressedKeys(Keys.Left) Then _scene.camera.Move(0, -_autoMoveStep)
                    If Display.PressedKeys(Keys.Right) Then _scene.camera.Move(0, _autoMoveStep)
                    If Display.PressedKeys(Keys.W) Then _scene.camera.Move(_autoMoveStep, 0)
                    If Display.PressedKeys(Keys.S) Then _scene.camera.Move(-_autoMoveStep, 0)
                    If Display.PressedKeys(Keys.A) Then _scene.camera.Move(0, -_autoMoveStep)
                    If Display.PressedKeys(Keys.D) Then _scene.camera.Move(0, _autoMoveStep)
                    If Display.PressedKeys(Keys.Q) Then _scene.camera.PositionY -= 10
                    If Display.PressedKeys(Keys.E) Then _scene.camera.PositionY += 10
                End If
                RaiseEvent RenderWorkCycle()
            End SyncLock
            Thread.Sleep(50)
        Loop
    End Sub
    Public Event RenderMouseMoved(ByVal offsetX As Integer, ByVal offsetY As Integer)
    Private Sub display_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Display.MouseMove
        If Display.MouseLockingEnabled Then
            Static lastState As Boolean
            If lastState <> Display.MouseLocked Then
                If Not Display.MouseLocked Then
                    lState.Text = "Щелкните левой кнопкой для управления."
                Else
                    lState.Text = "Управляйте с помощью клавиатуры и мыши. ESC - выход из управления."
                End If
            End If
        End If
    End Sub
    Private Sub MouseMovedHandler(ByVal offsetX As Integer, ByVal offsetY As Integer) Handles Display.MouseMoved
        SyncLock _syncRoot
            If _autoMove Then
                _scene.camera.Yaw += -offsetX / 100
                _scene.camera.Pitch += offsetY / 100
                If _scene.camera.Pitch > 0.5 Then _scene.camera.Pitch = 0.5
                If _scene.camera.Pitch < -0.5 Then _scene.camera.Pitch = -0.5
            End If
            RaiseEvent RenderMouseMoved(offsetX, offsetY)
        End SyncLock
    End Sub
    Public ReadOnly Property RenderPressedKeys() As Boolean()
        Get
            Return Display.PressedKeys
        End Get
    End Property
    Public Property RenderMouseLocking() As Boolean
        Get
            Return Display.MouseLocked
        End Get
        Set(ByVal value As Boolean)
            Display.MouseLockingEnabled = value
        End Set
    End Property

    Private Sub display_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Display.Load

    End Sub
    Public Overrides Sub Refresh()
        SyncLock _syncRoot
            If _scene.render.width > 0 Then
                _scene.FullDraw()
                Display.RefreshBitmap()
            End If
        End SyncLock
    End Sub

    Public Sub RenderRefresh()
        Refresh()
    End Sub

    Private Sub lState_Click(sender As Object, e As EventArgs) Handles lState.Click

    End Sub
End Class

