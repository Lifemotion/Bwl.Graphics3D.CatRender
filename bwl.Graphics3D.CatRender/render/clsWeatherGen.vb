
Public Class WeatherGen
    Private particles() As Point3D
    Private intParticlesCount As Integer
    Public render As Render3D
    Public floorLevel As Integer
    Public ceilLevel As Integer
    Public sprite As Sprite
    Public areaLeft As Integer
    Public areaRight As Integer
    Public areaTop As Integer
    Public areaBottom As Integer
    Public moveSpeed As Integer
    Public Sub Move()
        Dim i As Integer
        Dim needCreate As Integer = -1
        For i = 0 To intParticlesCount - 1
            With particles(i)
                .Y = .Y + moveSpeed
                If .Y < floorLevel Then CreateParticle(i) ' needCreate = i
            End With
        Next
        If needCreate >= 0 Then CreateParticle(needCreate)
    End Sub
    Private Sub CreateParticle(ByVal index As Integer)
        With particles(index)
            .X = areaLeft + (areaRight - areaLeft) * Rnd()
            .Z = areaTop + (areaBottom - areaTop) * Rnd()
            .Y = ceilLevel
        End With
    End Sub
    Public Sub Restart()
        Dim i As Integer
        For i = 0 To intParticlesCount - 1
            With particles(i)
                .X = areaLeft + (areaRight - areaLeft) * Rnd()
                .Z = areaTop + (areaBottom - areaTop) * Rnd()
                .Y = floorLevel + (ceilLevel - floorLevel) * Rnd()
            End With
        Next
    End Sub
    Private Sub Defaults()
        moveSpeed = -10
        areaLeft = -100
        areaRight = 100
        areaTop = -100
        areaBottom = 100
        ceilLevel = 100
    End Sub
    Public Sub Draw()
        If Not (sprite Is Nothing Or render Is Nothing) Then
            Dim i As Integer
            For i = 0 To intParticlesCount - 1
                With particles(i)
                    render.SceneDraw.DrawSprite(sprite, .X, .Y, .Z)
                End With
            Next
        End If
    End Sub
    Sub New()
        Defaults()
    End Sub
    Public Property ParticlesCount() As Integer
        Get
            Return intParticlesCount
        End Get
        Set(ByVal value As Integer)
            intParticlesCount = value
            If value = 0 Then ReDim particles(0)
            If value > 0 Then
                ReDim particles(value - 1)
                Restart()
            End If
        End Set
    End Property

End Class
