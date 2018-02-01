<Serializable()> _
Public Enum Object3DType
    model = 1
    sprite = 2
    light = 3
End Enum
<Serializable()> _
Public Class Object3D
    Public type As Object3DType
    <NonSerialized()> _
    Public model As Model
    Public modelMesh As Integer
    <NonSerialized()> _
   Public sprite As Sprite
    <NonSerialized()> _
    Public light As Lighter
    Public positionX, positionY, positionZ As Single
    Public rotateX, rotateY, rotateZ As Single
    Public scale As Single
    Public hidden As Boolean
    Public renderMode As RenderParameters
    Public objectName As String
    Public name As String
    Private myUID As Integer
    Private Shared UIDgen As Integer
    Private Shared UIDgenSync As New Object
    Sub New(ByVal objectName As String)
        Me.New()
        name = objectName
    End Sub
    Sub New()
        SyncLock UIDgenSync
            scale = 1
            modelMesh = -1
            UIDgen += 1
            myUID = UIDgen
        End SyncLock
    End Sub
    Private Sub Init()

      
    End Sub
    Public ReadOnly Property UID() As Integer
        Get
            Return myUID
        End Get
    End Property

    Public Shared Function CreateModelObject(model As Model, x As Single, y As Single, z As Single, rx As Single, ry As Single, rz As Single) As Object3D
        Dim obj As New Object3D
        obj.model = model
        obj.type = Object3DType.model
        obj.positionX = x
        obj.positionY = y
        obj.positionZ = z
        obj.rotateX = rx
        obj.rotateY = ry
        obj.rotateZ = rz
        Return obj
    End Function

    Public Shared Function CreateAmbientLightObject(color As Color) As Object3D
        Dim obj As New Object3D
        With obj
            .type = Object3DType.light
            .light = New Lighter
            .light.type = LighterTypeEnum.ambient
            .light.colorR = color.R
            .light.colorG = color.G
            .light.colorB = color.B
        End With
        Return obj
    End Function

    Public Shared Function CreatePointLightObject(color As Color, x As Single, y As Single, z As Single, intense As Single) As Object3D
        Dim obj As New Object3D
        With obj
            .type = Object3DType.light
            .light = New Lighter
            .light.type = LighterTypeEnum.point
            .light.intense = intense
            .light.colorR = color.R
            .light.colorG = color.G
            .light.colorB = color.B
            .positionX = x
            .positionY = y
            .positionZ = z
        End With
        Return obj
    End Function

    Public Shared Function CreateSpriteObject(sprite As Sprite, x As Single, y As Single, z As Single)
        Dim obj As New Object3D
        With obj
            .type = Object3DType.sprite
            .sprite = sprite
            .positionX = x
            .positionY = y
            .positionZ = z
        End With
        Return obj
    End Function

    Public Shared Function CreateSpriteObject(sprite As Sprite, x As Single, y As Single, z As Single,
                                               recolor As Color) As Object3D
        Dim obj As New Object3D
        With obj
            .type = Object3DType.sprite
            .sprite = sprite
            .positionX = x
            .positionY = y
            .positionZ = z
            'light не обязателен. Если он есть, будет использоваться для перекраски
            .light = New Lighter
            .light.colorR = recolor.R
            .light.colorG = recolor.G
            .light.colorB = recolor.B
        End With
        Return obj
    End Function

End Class
