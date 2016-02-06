'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Imports System.IO

Public Class frmSearch

    Private cutting_grid As List(Of String)
    Private cutting_indexing As List(Of Integer)
    Private contains_list As List(Of String)
    Private contains_proc As Boolean = False

    Private Sub frmSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Close()
        If e.KeyCode = Keys.F1 Then
            RadioButton3.Checked = True
        ElseIf e.KeyCode = Keys.F2 Then
            RadioButton1.Checked = True
        ElseIf e.KeyCode = Keys.F3 Then
            RadioButton2.Checked = True
        End If
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Or RadioButton4.Checked Then
            ListView1.Items.Clear()

            '
            '   이 세 개의 선택은 미리 indexing을 하지 않으므로
            '   직접 인덱싱해주어야함.
            '
            If RadioButton1.Checked Or RadioButton3.Checked Or RadioButton4.Checked Then
                If Not IsNothing(contains_list) Then
                    contains_list.Clear()
                End If
                contains_list = New List(Of String)
                If Not IsNothing(cutting_indexing) Then
                    cutting_indexing.Clear()
                End If
                cutting_indexing = New List(Of Integer)
                If RadioButton1.Checked Then
                    For i As Integer = 0 To cutting_grid.Count - 1
                        If cutting_grid(i).ToLower().StartsWith(ComboBox1.Text) Then
                            contains_list.Add(cutting_grid(i))
                            cutting_indexing.Add(i)
                        End If
                    Next
                Else
                    For i As Integer = 0 To cutting_grid.Count - 1
                        If cutting_grid(i).ToLower().Contains(ComboBox1.Text) Then
                            contains_list.Add(cutting_grid(i))
                            cutting_indexing.Add(i)
                        End If
                    Next
                End If
            End If
            For i As Integer = 0 To contains_list.Count - 1
                ListView1.Items.Add(New ListViewItem(New String() _
                        {cutting_indexing(i) + 1, contains_list(i)}))
            Next
        End If
    End Sub

    Private Sub ComboBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyUp
        If ComboBox1.Text.Length > 0 AndAlso contains_proc Then
            If Not IsNothing(contains_list) Then
                contains_list.Clear()
            End If
            contains_list = New List(Of String)
            If Not IsNothing(cutting_indexing) Then
                cutting_indexing.Clear()
            End If
            cutting_indexing = New List(Of Integer)
            For i As Integer = 0 To cutting_grid.Count - 1
                If cutting_grid(i).ToLower().Contains(ComboBox1.Text) Then
                    contains_list.Add(cutting_grid(i))
                    cutting_indexing.Add(i)
                End If
            Next

            Application.DoEvents()

            ' 이거 없으면 키보드 누를 때마다 커서가 자꾸 앞으로가서 글씨가 뒤로 써짐
            While ComboBox1.Items.Count > 0
                ComboBox1.Items.RemoveAt(0)
            End While
            ComboBox1.DroppedDown = True
            ComboBox1.Items.AddRange(contains_list.ToArray())
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub frmSearch_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim custlocy As Integer = Me.Location.Y + Me.Height / 2
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(locationx, custlocy - originh * frmMain.tanh_value(i) / 2)
            Me.Height = originh * frmMain.tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(custlocx - originw * frmMain.tanh_value(i) / 2, custlocy)
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
    End Sub

    Private Sub frmSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cutting_grid = New List(Of String)
        For Each Str As String In frmMain.FolderGrid
            cutting_grid.Add(Path.GetFileName(Str))
            ComboBox1.AutoCompleteCustomSource.Add(Path.GetFileName(Str))
        Next

        Me.Show()
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim custlocy As Integer = Me.Location.Y + Me.Height / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Location = New Point(locationx, custlocy)
        Me.Height = 0
        For i As Integer = 0 To 20
            Me.Location = New Point(custlocx - originw * Math.Tanh(i / 6) / 2, custlocy)
            Me.Width = originw * frmMain.tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        Me.Focus()
        Me.Location = New Point(locationx, custlocy)
        Me.Width = originw
        For i As Integer = 0 To 25
            Me.Location = New Point(locationx, custlocy - originh * frmMain.tanh_value(i) / 2)
            Me.Height = originh * frmMain.tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        Me.Location = New Point(locationx, locationy)
        Me.Height = originh
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            ComboBox1.AutoCompleteMode = AutoCompleteMode.Suggest
            ComboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource
        Else
            ComboBox1.AutoCompleteMode = AutoCompleteMode.None
            ComboBox1.AutoCompleteSource = AutoCompleteSource.None
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        contains_proc = RadioButton2.Checked
    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedItems.Count <> 0 Then
            frmMain.FindAndExpand(frmMain.FolderGrid( _
                    Convert.ToInt32(ListView1.SelectedItems.Item(0).Text) - 1))
        End If
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked Then
            contains_proc = False
        End If
    End Sub

End Class