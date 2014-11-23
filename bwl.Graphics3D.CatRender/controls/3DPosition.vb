Public Class Position3D
    Public Class Coordinates
        Public X, Y, Z, RX, RY, RZ As Single
    End Class
    Private _step As Single = 5
    Private _range As Single = 3000
    Private _angleStep As Single = 3
    Private _angleRange As Single = 360
    Private _ready As Boolean
    Private _notReady As Boolean
    Private Sub SetRanges()
        tbX.Minimum = -_range
        tbX.Maximum = _range
        tbY.Minimum = -_range
        tbY.Maximum = _range
        tbZ.Minimum = -_range
        tbZ.Maximum = _range
        tbX.Increment = _step
        tbY.Increment = _step
        tbZ.Increment = _step
        tbRX.Minimum = -_angleRange
        tbRX.Maximum = _angleRange
        tbRY.Minimum = -_angleRange
        tbRY.Maximum = _angleRange
        tbRZ.Minimum = -_angleRange
        tbRZ.Maximum = _angleRange
        tbRX.Increment = _angleStep
        tbRY.Increment = _angleStep
        tbRZ.Increment = _angleStep
    End Sub
    Public Property PositionX() As Single
        Get
            Return tbX.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbX.Value = value
            _notReady = False
        End Set
    End Property
    Public Property PositionY() As Single
        Get
            Return tbY.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbY.Value = value
            _notReady = False
        End Set
    End Property

    Public Property PositionZ() As Single
        Get
            Return tbZ.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbZ.Value = value
            _notReady = False
        End Set
    End Property
    Public Property RotationX() As Single
        Get
            Return tbRX.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbRX.Value = value
            _notReady = False
        End Set
    End Property
    Public Property RotationY() As Single
        Get
            Return tbRY.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbRY.Value = value
            _notReady = False
        End Set
    End Property
    Public Property RotationZ() As Single
        Get
            Return tbRZ.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbRZ.Value = value
            _notReady = False
        End Set
    End Property
    Public Property FOV() As Single
        Get
            Return tbFOV.Value
        End Get
        Set(ByVal value As Single)
            _notReady = True
            tbFOV.Value = value
            _notReady = False
        End Set
    End Property
    Private Sub Position3D_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SetRanges()
        _ready = True
    End Sub
    Public Event CoordinatesChanged()
    Private Sub ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbZ.ValueChanged, tbY.ValueChanged, tbX.ValueChanged, tbRZ.ValueChanged, tbRY.ValueChanged, tbRX.ValueChanged, tbFOV.ValueChanged
        If _ready And Not _notReady Then RaiseEvent CoordinatesChanged()
    End Sub
    Public Sub FromObject3D(ByVal object3d As Object3D)
        With object3d
            PositionX = .positionX
            PositionY = .positionY
            PositionZ = .positionZ
            RotationX = Tools.RadToDeg(.rotateX)
            RotationY = Tools.RadToDeg(.rotateY)
            RotationZ = Tools.RadToDeg(.rotateZ)
            lbFOV.Hide()
            tbFOV.Hide()
        End With
    End Sub
    Public Sub FromCamera3D(ByVal camera3d As Camera3D)
        With camera3d
            PositionX = .PositionX
            PositionY = .PositionY
            PositionZ = .PositionZ
            RotationX = Tools.RadToDeg(.Pitch)
            RotationY = Tools.RadToDeg(.Yaw)
            RotationZ = Tools.RadToDeg(.Roll)
            lbFOV.Show()
            tbFOV.Show()
            tbFOV.Value = .FOV
        End With
    End Sub
    Public Sub ToCamera3D(ByVal camera3d As Camera3D)
        With camera3d
            .PositionX = PositionX
            .PositionY = PositionY
            .PositionZ = PositionZ
            .Pitch = Tools.DegToRad(RotationX)
            .Yaw = Tools.DegToRad(RotationY)
            .Roll = Tools.DegToRad(RotationZ)
            .FOV = FOV
        End With
    End Sub
    Public Sub ToObject3D(ByVal object3D As Object3D)
        With object3D
            .positionX = PositionX
            .positionY = PositionY
            .positionZ = PositionZ
            .rotateX = Tools.DegToRad(RotationX)
            .rotateY = Tools.DegToRad(RotationY)
            .rotateZ = Tools.DegToRad(RotationZ)
        End With
    End Sub

    Public Sub New()

        ' Этот вызов необходим конструктору форм Windows.
        InitializeComponent()

        ' Добавьте все инициализирующие действия после вызова InitializeComponent().

    End Sub
End Class
