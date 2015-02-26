Public Module mdlStructures
    ''' <summary>
    ''' ����� �������� ��� �������� ������ ������.
    ''' ����� ���������� ����������� ��� ��������� � ���������
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Vertexes
        '���������
        Dim name As String
        '��������� ���������
        '���������� ������
        Dim pX() As Single
        Dim pY() As Single
        Dim pZ() As Single
        '���������� ����������
        Dim tU() As Single
        Dim tV() As Single
        '���������� ��������
        Dim nX() As Single
        Dim nY() As Single
        Dim nZ() As Single
        '���� �������
        Dim Color()() As Long
        '����� ���������
        Dim count As Integer
        '��������� ���������
        Dim temp() As Single
        '������ �� �������� (����������� ��� ����� ������)
        Dim material() As Material
        '������������� ��������� ��� ������� ��������
        Dim materialUID() As Integer
        Dim size() As Single
        Dim triangleNum() As Integer
        Dim id As Integer
        '���������� �� �������� ����������������� � ������ � �������������
        Dim rX() As Single
        Dim rY() As Single
        Dim rZ() As Single
    End Structure
    Public Structure VertexesDraw
        '��������� ���������, ����������� ��� ���������
        '���������� ��������
        Dim scrX() As Single
        Dim scrY() As Single
        'W-�����
        Dim w() As Single
        '�� �������� �������
        Dim hidden() As Boolean

    End Structure
    Public Structure TrianglesDraw
        '��������� ���������, ����������� ��� ���������
        Dim maxX(), minX() As Single
        Dim maxY(), minY() As Single
        Dim minW(), maxW() As Single
        Dim screenSize() As Single
    End Structure
    Public Structure Mesh
        ''' <summary>
        ''' ��� �����
        ''' </summary>
        ''' <remarks></remarks>
        Dim Name As String
        ''' <summary>
        ''' ��������� ������ ������
        ''' </summary>
        ''' <remarks></remarks>
        Dim Vertexes As Vertexes
        ''' <summary>
        ''' ������ �������������, ����������� �� �������
        ''' </summary>
        ''' <remarks></remarks>
        Dim Triangles As Triangles
        ''' <summary>
        ''' �������������� ��� ��������� ����� ������
        ''' </summary>
        ''' <remarks></remarks>
        Dim PreparedVertexes As Vertexes
    End Structure
    Public Structure Triangles
        '������ �������������� ������
        Dim v1() As Integer
        Dim v2() As Integer
        Dim v3() As Integer
        '���������� ������� � ������������
        Dim nX() As Single
        Dim nY() As Single
        Dim nZ() As Single
        '����� ��������� ������������
        Dim materialID() As Integer
        '������� ����������
        Dim textureSize() As Single

        'Dim material() As Material
        '������������� ��������� ��� ������� ��������
        ' Dim materialUID() As Integer
        Dim count As Integer
        '��� ��������� �������� ������������ (��� ���������) - ���������� ���-�� ;)
        '���������� �����������
        Dim renderSettings() As RenderParameters
    End Structure
    '���-���-���
    '������ ��� �������
    <Serializable()> _
    Public Structure Point2D
        Dim X As Single
        Dim Y As Single
    End Structure
    <Serializable()> _
    Public Structure Point3D
        Public Sub New(x As Single, y As Single, z As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub
        Dim X As Single
        Dim Y As Single
        Dim Z As Single
    End Structure
    Public Structure SpriteStruct
        Dim sprite As Sprite
        Dim lighter As Lighter
        Dim x, y, z As Single
        Dim px, py As Integer
        Dim UID As Integer
    End Structure
    'Public Structure SpriteDrawStruct
    'Dim x, y, z As Single
    ' End Structure
    Public Structure LighterStruct
        Dim x, y, z As Single
        Dim r, g, b As Integer
        Dim type As LighterTypeEnum
        Dim intense As Integer
        Dim attenutionA, attenutionB As Single
    End Structure
    Public Enum RenderMode As Byte
        byRender = 0
        fast = 1
        normal = 2
        special = 3
    End Enum
    Public Enum TexturingMode As Byte
        byRender = 0
        none = 1
        normal = 2
        bilinear = 3
        transparent = 4
    End Enum
    Public Enum LightingMode As Byte
        byRender = 0
        none = 1
        full = 2
        ambientOnly = 3
    End Enum
    Public Enum MipMapMode As Byte
        byRender = 0
        mipMapOff = 1
        mipMapOn = 2
    End Enum
    Public Enum NormalsInterpolationMode
        byRender = 0
        oneForTriangle = 1
        interpolateByMesh = 2
    End Enum
    Public Enum NonfacialCulling As Integer
        byRender = 0
        none = 2
        positive = 3
        negative = 1
    End Enum
    Public Enum SpecialMode
        none = 0
        nearW = 1
    End Enum
    <Serializable()> _
    Public Structure RenderParameters
        Dim special As SpecialMode
        Dim renderer As RenderMode
        Dim texturing As TexturingMode
        Dim lighting As LightingMode
        Dim mipMap As MipMapMode
        Dim normals As NormalsInterpolationMode
        Dim shadowedBy As Byte
        Dim shadowsOn As Byte
        Dim culling As NonfacialCulling
        Dim isSkybox As Boolean
    End Structure
    Public Structure RenderSettings
        Dim drawLines As Boolean
        Dim drawTriangles As Boolean
        Dim sortTriangles As Boolean
        Dim useFastCulling As Boolean
    End Structure
    Public Structure EnvironmentSettings
        Dim fog As Integer
        Dim fogColorR, fogColorG, fogColorB As Integer
        Dim distortionA As Single
        Dim distortionB As Single
        Dim distortionScale As Single
        Dim colorDown As Single
    End Structure
    Public Enum AxisType As Byte
        x = 1
        y = 2
        z = 3
    End Enum
    Public Structure ShadowPlane
        Dim axis As AxisType
        Dim distanse As Single
        Dim scale As Integer
        Dim map As PixelSurface
        Dim x1, x2, y1, y2 As Integer
        Dim used As Boolean
    End Structure
End Module
