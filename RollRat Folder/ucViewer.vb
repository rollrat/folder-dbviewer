'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Public Class ucViewer

    Public map As Image
    Private selected As Boolean = False
    Private label As String = ""
    Private Shadows font As Font
    Public Address As String
    Private Mouse_Enter As Boolean = False

    'Public Event view_event As EventHandler
    'Public Sub view_event_start(e As EventArgs)
    '    RaiseEvent view_event(Me, e)
    'End Sub

    Private Sub ucViewer_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics

        ' 크기 조정 메서드
        Dim calcWidth As Integer = Me.Width - 38
        Dim calcHeight As Integer = Me.Height - 38

        Dim sizeCust As Double
        If (calcWidth / Convert.ToDouble(map.Width)) <= (calcHeight / Convert.ToDouble(map.Height)) Then
            sizeCust = (calcWidth / Convert.ToDouble(map.Width))
        Else
            sizeCust = (calcHeight / Convert.ToDouble(map.Height))
        End If

        ' 배율 설정
        Dim custWidth As Integer = map.Width * sizeCust
        Dim custHeight As Integer = map.Height * sizeCust

        ' 테두리 보정값
        Dim custX As Integer = 4 + (calcWidth - custWidth) / 2
        Dim custY As Integer = 4 + (calcHeight - custHeight) / 2

        ' 썸네일 그림자
        g.FillRectangle(Brushes.DarkGray, custX + 3, custY + 3, custWidth, custHeight)

        ' 선택되었을 때 박스 그리기
        If selected Then
            g.DrawRectangle(New Pen(Color.LightSkyBlue, 2), custX - 2, custY - 2, custWidth + 8, custHeight + 35)
        End If

        g.DrawImage(map, custX, custY, custWidth, custHeight)

        ' 라벨 삽입
        If label <> "" Then
            Dim rectF1 As New RectangleF(custX, custY + custHeight + 2, custWidth, custHeight + 34)
            g.DrawString(label, font, Brushes.Black, rectF1)
        End If

        ' 마우스가 그림위에 있을때
        If Mouse_Enter Then
            Dim basicBrushes As SolidBrush = New SolidBrush(Color.FromArgb(100, 203, 226, 233))
            g.FillRectangle(basicBrushes, custX - 3, custY - 3, custWidth + 9, custHeight + 36)
        End If

    End Sub

    Public Overloads Sub Dispose()
        map.Dispose()
    End Sub

    Public Property SetSelectStatus() As Boolean
        Get
            Return selected
        End Get
        Set(value As Boolean)
            selected = value
            Me.Invalidate()
        End Set
    End Property

    Public Property SetMouseInside() As Boolean
        Get
            Return Mouse_Enter
        End Get
        Set(value As Boolean)
            Mouse_Enter = value
            Me.Invalidate()
        End Set
    End Property

    Public Sub SetLabel(ByVal label As String, ByVal font As Font)
        Me.label = label
        Me.font = font
    End Sub

    Public Sub SetImage(ByVal map As Image)
        Me.map = map
    End Sub

    Public Sub SetImageFromAddress(ByVal addr As String, ByVal pannelw As Integer, ByVal pannelh As Integer)
        Dim imt As Image = Image.FromFile(addr)

        Address = addr

        Dim sizeCust As Double
        If (pannelw / Convert.ToDouble(imt.Width)) <= (pannelh / Convert.ToDouble(imt.Height)) Then
            sizeCust = (pannelw / Convert.ToDouble(imt.Width))
        Else
            sizeCust = (pannelh / Convert.ToDouble(imt.Height))
        End If

        ' 배율보정
        Dim custWidth As Integer = imt.Width * sizeCust
        Dim custHeight As Integer = imt.Height * sizeCust

        map = New Bitmap(custWidth, custHeight)

        Dim g As Graphics = Graphics.FromImage(map)
        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        g.DrawImage(imt, 0, 0, custWidth, custHeight)
        g.Dispose()

        imt.Dispose()
    End Sub

    Private Sub ucViewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Invalidate()
    End Sub

End Class
