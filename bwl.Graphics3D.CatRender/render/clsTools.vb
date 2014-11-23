Public Class Tools
    Public Shared Function DegToRad(ByVal degress As Single)
        Return Math.PI * degress / 180
    End Function
    Public Shared Function RadToDeg(ByVal radian As Single)
        Return radian * 180 / Math.PI
    End Function
End Class
