<Serializable()> _
Public Class Camera3D
    Private intPositionX, intPositionY, intPositionZ As Single
    Private intTargetX, intTargetY, intTargetZ As Single
    Private intRoll, intPitch, intYaw As Single
    Public FOV As Single
    Sub New()
        intPositionZ = -10
        FOV = 75
    End Sub
    Public Property PositionX() As Single
        Get
            Return intPositionX
        End Get
        Set(ByVal value As Single)
            intPositionX = value
            'Target2Angle()
        End Set
    End Property
    Public Property PositionY() As Single
        Get
            Return intPositionY
        End Get
        Set(ByVal value As Single)
            intPositionY = value
            ' Target2Angle()
        End Set
    End Property
    Public Property PositionZ() As Single
        Get
            Return intPositionZ
        End Get
        Set(ByVal value As Single)
            intPositionZ = value
            'Target2Angle()
        End Set
    End Property
    Public Property Pitch() As Single
        Get
            Return intPitch
        End Get
        Set(ByVal value As Single)
            intPitch = value
        End Set
    End Property
    Public Property Yaw() As Single
        Get
            Return intYaw
        End Get
        Set(ByVal value As Single)
            intYaw = value
        End Set
    End Property
    Public Property Roll() As Single
        Get
            Return intRoll
        End Get
        Set(ByVal value As Single)
            intRoll = value
        End Set
    End Property
    Public Sub Move(ByVal moveFront As Single, ByVal moveStrafe As Single)
        Dim x, y, z As Single
        z = PositionZ
        x = PositionX
        y = PositionY
        z = z + Math.Cos(Yaw) * moveFront
        x = x - Math.Sin(Yaw) * moveFront
        z = z + Math.Sin(Yaw) * moveStrafe
        x = x + Math.Cos(Yaw) * moveStrafe
        y = y - Math.Sin(Pitch) * moveFront
        PositionZ = z
        PositionX = x
        PositionY = y
    End Sub

End Class
