'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Imports System.IO

Public Class frmLargePicPreview

    Dim now_pic_addr As String
    Dim pure_title As String

    ' true: 1, false: 2
    Dim pictureboxswitch As Boolean = False

    Dim ShowPos As Integer = 12
    Dim SinkPos As Integer = 787

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        ' 왼쪽, 중앙, 오른쪽
        Dim pt As Point = DirectCast(e, MouseEventArgs).Location
        If pt.X < PictureBox1.Size.Width * 1 / 3 Then

            '
            '   그림의 왼쪽 부분을 클릭한 경우
            '
            set_prev_picture()
        ElseIf pt.X < PictureBox1.Size.Width * 2 / 3 Then

            '
            '   그림의 중앙 부분을 클릭한 경우
            '
            Me.Close()
        Else

            '
            '   그림의 오른쪽 부분을 클릭한 경우
            '
            set_next_picture()
        End If
    End Sub

    'Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
    '    ' 왼쪽, 중앙, 오른쪽
    '    Dim pt As Point = DirectCast(e, MouseEventArgs).Location
    '    If pt.X < PictureBox1.Size.Width * 1 / 3 Then

    '        '
    '        '   그림의 왼쪽 부분을 클릭한 경우
    '        '
    '        set_prev_picture()
    '    ElseIf pt.X < PictureBox1.Size.Width * 2 / 3 Then

    '        '
    '        '   그림의 중앙 부분을 클릭한 경우
    '        '
    '        Me.Close()
    '    Else

    '        '
    '        '   그림의 오른쪽 부분을 클릭한 경우
    '        '
    '        set_next_picture()
    '    End If
    'End Sub

    Private Sub set_next_picture()
        For i = 0 To frmPicviewer.listbox_item_text.Count - 1

            '
            '   해당 그림의 주소가 포함된 리스트의 오프셋을 가져옴
            '
            If frmPicviewer.listbox_item_text(i) = Path.GetFileName(now_pic_addr) Then
RE:
                '
                '   i가 마지막 부분에 도달하지 않았는지 확인
                '
                If i <> frmPicviewer.listbox_item_text.Count - 1 Then
                    now_pic_addr = frmPicviewer.current_addr & "\" & frmPicviewer.listbox_item_text(i + 1)

                    '
                    '   현재 파일이 그림의 확장자가 맞는지 확인
                    '
                    If frmMain.pic_extension.Contains(IO.Path.GetExtension(now_pic_addr)) Then
                        Me.Text = pure_title & Path.GetFileName(now_pic_addr)
                        'If pictureboxswitch Then
                        '    If Not IsNothing(PictureBox1.Image) Then
                        PictureBox1.Image.Dispose()
                        '    End If
                        PictureBox1.Image = Image.FromFile(now_pic_addr)
                        '    ShowPic1Next()
                        'Else
                        '    If Not IsNothing(PictureBox2.Image) Then
                        '        PictureBox2.Image.Dispose()
                        '    End If
                        '    PictureBox2.Image = Image.FromFile(now_pic_addr)
                        '    ShowPic2Next()
                        'End If
                        pictureboxswitch = Not pictureboxswitch ' 토글
                        Exit For
                    Else
                        i += 1
                        GoTo RE
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub set_prev_picture()
        For i = 0 To frmPicviewer.listbox_item_text.Count - 1

            '
            '   해당 그림의 주소가 포함된 리스트의 오프셋을 가져옴
            '
            If frmPicviewer.listbox_item_text(i) = Path.GetFileName(now_pic_addr) Then
RE:
                '
                '   i가 처음 부분에 도달하지 않았는지 확인
                '
                If i <> 0 Then
                    now_pic_addr = frmPicviewer.current_addr & "\" & frmPicviewer.listbox_item_text(i - 1)

                    '
                    '   현재 파일이 그림의 확장자가 맞는지 확인
                    '
                    If frmMain.pic_extension.Contains(IO.Path.GetExtension(now_pic_addr).ToLower) Then
                        Me.Text = pure_title & Path.GetFileName(now_pic_addr)
                        'If pictureboxswitch Then
                        '    If Not IsNothing(PictureBox1.Image) Then
                        PictureBox1.Image.Dispose()
                        '    End If
                        PictureBox1.Image = Image.FromFile(now_pic_addr)
                        '    ShowPic1Prev()
                        'Else
                        '    If Not IsNothing(PictureBox2.Image) Then
                        '        PictureBox2.Image.Dispose()
                        '    End If
                        '    PictureBox2.Image = Image.FromFile(now_pic_addr)
                        '    ShowPic2Prev()
                        'End If
                        pictureboxswitch = Not pictureboxswitch
                        Exit For
                    Else
                        i -= 1
                        GoTo RE
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub frmPicPreview_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Or e.KeyCode = Keys.Enter Then Me.Close()
        If e.KeyCode = Keys.Left Then
            set_prev_picture()
        ElseIf e.KeyCode = Keys.Right Then
            set_next_picture()
        End If
    End Sub

    Private Sub frmPicPreview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        now_pic_addr = frmPicviewer.pic_addr
        If Not now_pic_addr Is Nothing Then
            PictureBox1.Image = Image.FromFile(now_pic_addr)
            pure_title = Me.Text
            PictureBox1.Image = Image.FromFile(now_pic_addr)
            Me.Text = pure_title & Path.GetFileName(now_pic_addr)
        Else
            Close()
        End If
        '/////////////////////
        Me.Show()
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Height = 0
        For i As Integer = 0 To 25
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        Me.Width = originw
        For i As Integer = 0 To 25
            Me.Height = originh * frmMain.tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(10)
        Next
        Me.Height = originh
        '/////////////////////
    End Sub

    Private Sub frmLargePicPreview_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        '/////////////////////////
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        For i As Integer = 25 To 0 Step -1
            Me.Height = originh * frmMain.tanh_value(i)
            'Application.DoEvents()
            Threading.Thread.Sleep(10)
        Next
        For i As Integer = 25 To 0 Step -1
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        '/////////////////////////
    End Sub

    'Private Sub ShowPic1Next()
    '    Dim originpic1x As Integer = PictureBox1.Location.X - 600
    '    Dim originpic2x As Integer = PictureBox2.Location.X
    '    PictureBox2.SendToBack()
    '    For i As Integer = 0 To 10
    '        PictureBox1.Location = New Point(ShowPos + originpic1x * Math.Tanh((10 - i) / 3), 12)
    '        PictureBox2.Location = New Point(ShowPos - SinkPos / 5 * 4 * Math.Tanh(i / 3), 12)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(1)
    '    Next
    '    PictureBox2.Location = New Point(SinkPos, 12)
    'End Sub

    'Private Sub ShowPic2Next()
    '    Dim originpic1x As Integer = PictureBox1.Location.X
    '    Dim originpic2x As Integer = PictureBox2.Location.X - 600
    '    PictureBox1.SendToBack()
    '    For i As Integer = 0 To 10
    '        PictureBox1.Location = New Point(ShowPos - SinkPos / 5 * 4 * Math.Tanh(i / 3), 12)
    '        PictureBox2.Location = New Point(ShowPos + originpic2x * Math.Tanh((10 - i) / 3), 12)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(1)
    '    Next
    '    PictureBox1.Location = New Point(SinkPos, 12)
    'End Sub

    'Private Sub ShowPic1Prev()
    '    Dim originpic1x As Integer = PictureBox1.Location.X - 600
    '    Dim originpic2x As Integer = PictureBox2.Location.X
    '    PictureBox2.SendToBack()
    '    For i As Integer = 0 To 10
    '        PictureBox1.Location = New Point(ShowPos - originpic1x * Math.Tanh((10 - i) / 3), 12)
    '        PictureBox2.Location = New Point(ShowPos + SinkPos / 5 * 4 * Math.Tanh(i / 3), 12)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(1)
    '    Next
    '    PictureBox2.Location = New Point(SinkPos, 12)
    'End Sub

    'Private Sub ShowPic2Prev()
    '    Dim originpic1x As Integer = PictureBox1.Location.X
    '    Dim originpic2x As Integer = PictureBox2.Location.X - 600
    '    PictureBox1.SendToBack()
    '    For i As Integer = 0 To 10
    '        PictureBox1.Location = New Point(ShowPos + SinkPos / 5 * 4 * Math.Tanh(i / 3), 12)
    '        PictureBox2.Location = New Point(ShowPos - originpic2x * Math.Tanh((10 - i) / 3), 12)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(1)
    '    Next
    '    PictureBox1.Location = New Point(SinkPos, 12)
    'End Sub

End Class