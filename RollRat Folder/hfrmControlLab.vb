'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Public Class hfrmControlLab

    Dim label1pos As Point
    Dim label2pos As Point

    Private Function sigmoid_test(ByVal val As Double) As Double
        Return 1 / (1 + Math.Exp(-val))
    End Function

    Private Function double_tanh_test(ByVal val As Double) As Double
        If val <= 0 Then
            Return Math.Tanh(val + 4) / 4 + 0.5
        ElseIf val > 0 Then
            Return Math.Tanh(val - 4) / 4 + 1.5
        End If
    End Function

    Private Sub hfrmControlLab_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim custlocy As Integer = Me.Location.Y + Me.Height / 2
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(locationx, custlocy - originh * Math.Tanh(i / 7) / 2)
            Me.Height = originh * frmMain.tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(custlocx - originw * Math.Tanh(i / 7) / 2, custlocy)
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
    End Sub

    Private Sub hfrmControlLab_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Show()
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim custlocy As Integer = Me.Location.Y + Me.Height / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Location = New Point(locationx, custlocy)
        Me.Height = 0
        For i As Integer = 0 To 25
            Me.Location = New Point(custlocx - originw * frmMain.tanh_value(i) / 2, custlocy)
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

        label1pos = Label1.Location
        label2pos = Label2.Location
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Label1.Location = label1pos
        Label2.Location = label2pos

        Dim label1dest As Integer = (Label3.Location.Y + Label1.Location.Y) / 2 - Label1.Location.Y
        Dim label1originx As Integer = Label1.Location.X
        Dim label1originy As Integer = Label1.Location.Y
        Dim label2dest As Integer = (Label4.Location.X + Label2.Location.X) / 2 - Label2.Location.X
        Dim label2originx As Integer = Label2.Location.X
        Dim label2originy As Integer = Label2.Location.Y
        For i As Integer = 0 To 25
            Label1.Location = New Point(label1originx, label1originy + label1dest * Math.Tanh(i / 9))
            Label2.Location = New Point(label2originx + label2dest * Math.Tanh(i / 9), label2originy)
            Application.DoEvents()
            Threading.Thread.Sleep(30)
        Next
        label1originy = Label1.Location.Y
        label2originx = Label2.Location.X
        For i As Integer = 25 To 0 Step -1
            Label1.Location = New Point(label1originx, label1originy + label1dest * (1 - Math.Tanh(i / 9)))
            Label2.Location = New Point(label2originx + label2dest * (1 - Math.Tanh(i / 9)), label2originy)
            Application.DoEvents()
            Threading.Thread.Sleep(30)
        Next
    End Sub

End Class