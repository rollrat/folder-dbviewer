'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Imports System.IO
Imports System.Collections.ObjectModel

Public Class frmPicviewer

    Private ControlKeyDown As Boolean = False
    Private AlternativeKeyDown As Boolean = False
    Private PerpectEnterKeyDown As Boolean = False
    Private ViewerWidth As Integer = 256
    Private SelectedViewer As ucViewer
    Private ViewerCollection As New List(Of ucViewer)
    Private IsFolderLoad As Boolean

    '
    '   LagePicPreview를 실행시키는 함수
    '
    Private Sub ImageViewShow()
        current_addr = Path.GetDirectoryName(SelectedViewer.Address)
        pic_addr = SelectedViewer.Address

        listbox_item_text.Clear()
        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(current_addr, FileIO.SearchOption.SearchTopLevelOnly, "*.*")
        For Each x In files
            listbox_item_text.Add(Path.GetFileName(x))
        Next

        frmLargePicPreview.Show()
    End Sub

    Private Sub frmPicviewer_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        PerpectEnterKeyDown = True
        ControlKeyDown = e.Control
        AlternativeKeyDown = e.Alt
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub
    Private Sub frmPicviewer_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        ControlKeyDown = False
        AlternativeKeyDown = False
        If e.KeyCode = Keys.Enter AndAlso PerpectEnterKeyDown Then ImageViewShow()
        PerpectEnterKeyDown = False
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or _
            e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Then
            SelectSpecificItem(e.KeyCode)
        End If
    End Sub

    '
    '   SelectItemAndFocus & SelectSpecificItem:
    '   키보드에 눌린 키에 따라 해당 아이템으로 포커스를 이동시킵니다.
    '   (단, 키보드에서 아래 키가 눌린 경우 높이에 따라 위 항목이 보이게 끔 좌표를 수정합니다.)
    '
    Private Sub SelectItemAndFocus(ByVal id As Integer, Optional ByVal spacing As Boolean = False)
        If Not IsNothing(SelectedViewer) Then
            SelectedViewer.SetSelectStatus = False
        End If
        SelectedViewer = ViewerCollection(id)
        SelectedViewer.SetSelectStatus = True

        '
        '   ScrollControlIntoView가 먹통이라 꼼수로 쓴거(ScrollToControl 구문 수정해도 안됨)
        '   현제 컨트롤이 뷰 범위에 없을 경우 스크롤을 그곳으로 옮김
        '
        If Not ((SelectedViewer.Location.Y + SelectedViewer.Height) < ScrollFixLayoutPanel1.Height _
                AndAlso SelectedViewer.Location.Y > 0) Then

            '
            '   레이아웃 패널의 왼쪽 위 꼭짓점으로부터의 선택된 뷰어에 상대적인 Y좌표를 계산함.
            '
            Dim setpoint As Integer = -ScrollFixLayoutPanel1.AutoScrollPosition.Y + SelectedViewer.Location.Y

            '
            '   ScrollFixLayoutPanel안에 몇 줄의 뷰어가 포함되어있나?
            '
            Dim item_max_height As Integer = (ScrollFixLayoutPanel1.Height) \ SelectedViewer.Height

            '
            '   Keys.Down이 눌린 경우 현재 폼의 높이가 선택된 항목의 크기의 두 배보다 클 경우
            '   위 항목 일부를 보여주기위해 현제 선택한 뷰어의 높이를 감산함. 
            '
            Dim point As Point
            If spacing AndAlso ScrollFixLayoutPanel1.Height > SelectedViewer.Height * 2 Then
                'point = New Point(0, setpoint - SelectedViewer.Height * (item_max_height - 1))
                point = New Point(0, setpoint - ScrollFixLayoutPanel1.Height + SelectedViewer.Height + 5)
            Else
                point = New Point(0, setpoint)
            End If
            ScrollFixLayoutPanel1.AutoScrollPosition = point
        End If
    End Sub
    Private Sub SelectSpecificItem(ByVal index As Keys)
        If IsNothing(SelectedViewer) Then Exit Sub

        ' 끄트머리에 걸쳐있어 다음 항목이 추가되지 않으면 오차가 생깁니다.
        Dim item_max As Integer = (ScrollFixLayoutPanel1.Width - 55) \ ViewerWidth ' 반올림 없는 나눗셈 연산
        If item_max <= 1 Then Exit Sub

        Dim i As Integer
        For i = 0 To ViewerCollection.Count - 1
            If ViewerCollection(i).Address = SelectedViewer.Address Then
                Exit For
            End If
        Next

        If index = Keys.Up Then
            If i >= item_max Then SelectItemAndFocus(i - item_max)
        ElseIf index = Keys.Down Then
            If i + item_max < ViewerCollection.Count Then SelectItemAndFocus(i + item_max, True) _
                Else SelectItemAndFocus(ViewerCollection.Count - 1, True)
        ElseIf index = Keys.Right Then
            If i Mod item_max <> (item_max - 1) AndAlso (i + 1) <> ViewerCollection.Count Then SelectItemAndFocus(i + 1)
        ElseIf index = Keys.Left Then
            If i Mod item_max <> 0 Then SelectItemAndFocus(i - 1)
        End If
    End Sub

    Private Sub ucViewer_MouseClick(sender As Object, e As MouseEventArgs)
        If Not IsNothing(SelectedViewer) Then
            SelectedViewer.SetSelectStatus = False
        End If
        SelectedViewer = sender
        SelectedViewer.SetSelectStatus = True
    End Sub

    '
    '   뷰어 컨트롤에서 더블클릭 이벤트가 발생한 경우 소스코드의 
    '   If문 순서로 다음 네 가지 중 하나의 작업을 수행합니다.
    '
    '   1. 컨트롤키가 눌린 상태라면, 선택된 파일이 있는 폴더를 Main에 나타냅니다.
    '   2. 알트키가 눌린 상태고, 폴더안 폴더를 가져온 상태라면, 선택된 파일의 폴더에 포함된
    '       모든 요소를 나열하는 뷰를 실행시킵니다. 또한 Main에서도 해당 폴더가 선택됩니다.
    '   3. 알트키가 눌린 상태고, 폴더안 파일을 가져온 상태라면, 선택된 파일이 포함된 폴더의
    '       상위폴더에서 폴더안 폴더의 첫 번째 사진 파일을 나열하는 뷰를 실행시킵니다.
    '       또한 Main에서도 해당 상위 폴더가 선택됩니다.
    '   4. 위를 만족하는 작업이 없을 경우 LagePicPreview를 실행합니다.
    '
    Public Shared current_addr As String
    Public Shared listbox_item_text As New ArrayList
    Public Shared pic_addr As String
    Private Sub ucViewer_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        If ControlKeyDown Then
            frmMain.FindAndExpand(Path.GetDirectoryName(SelectedViewer.Address))
        ElseIf AlternativeKeyDown Then
            If IsFolderLoad Then
                frmMain.RequestFileViewAlternative(Path.GetDirectoryName(SelectedViewer.Address))
            Else
                frmMain.RequestFolderViewAlternative(Path.GetDirectoryName(SelectedViewer.Address))
            End If
        Else
            ImageViewShow()
        End If
        ControlKeyDown = False
        AlternativeKeyDown = False
    End Sub
    Private Sub ucViewer_MouseEnter(sender As Object, e As EventArgs)
        DirectCast(sender, ucViewer).SetMouseInside() = True
    End Sub
    Private Sub ucViewer_MouseLeave(sender As Object, e As EventArgs)
        DirectCast(sender, ucViewer).SetMouseInside() = False
    End Sub
    'Private Sub ViewEvent(sender As Object, e As EventArgs)
    '    Dim mex As Integer = sender.Location.X
    '    Dim mey As Integer = sender.Location.Y
    '    For i As Integer = 0 To 25
    '        sender.Location = New Point(mex + 1000 * (1 - frmMain.tanh_value(i)), mey)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(10)
    '    Next
    '    sender.Location = New Point(mex, mey)
    'End Sub

    Private Sub AddImageLabelFolder(ByVal addr As String)
        If InvokeRequired Then
            Invoke(AddImageLabelFolderDelegate, addr)
        Else
            Dim ucv As New ucViewer

            If IsFolderLoad Then
                Dim AddrSplit As String() = addr.Split("\")
                ucv.SetLabel(AddrSplit(AddrSplit.Length - 2), Me.Font)
            Else
                ucv.SetLabel(Path.GetFileNameWithoutExtension(addr), Me.Font)
            End If
            ucv.Dock = DockStyle.Bottom
            ucv.SetImageFromAddress(addr, 256, 256)
            ucv.Width = ViewerWidth
            ucv.Height = ucv.map.Height
            AddHandler ucv.MouseClick, New MouseEventHandler(AddressOf ucViewer_MouseClick)
            AddHandler ucv.MouseDoubleClick, New MouseEventHandler(AddressOf ucViewer_MouseDoubleClick)
            AddHandler ucv.MouseEnter, New EventHandler(AddressOf ucViewer_MouseEnter)
            AddHandler ucv.MouseLeave, New EventHandler(AddressOf ucViewer_MouseLeave)
            'AddHandler ucv.view_event, New EventHandler(AddressOf ViewEvent)

            ViewerCollection.Add(ucv)
            ScrollFixLayoutPanel1.Controls.Add(ucv)
            'ucv.view_event_start(New EventArgs)
            AdjustViewerHeight()
            Application.DoEvents()
        End If
    End Sub

    Private Delegate Sub DelegateAddImage(ByVal addr As String)
    Private AddImageLabelFolderDelegate As DelegateAddImage
    Dim nowThread As Threading.Thread
    Private Sub AddThread(ByVal ListOfString As List(Of String))
        Dim i As Integer = 1
        For Each addr As String In ListOfString
            AddImageLabelFolder(addr)
        Next
    End Sub

    '
    '   각 줄에 포함된 컨트롤 중 높이가 가장 큰 것을 해당 줄에 적용시킵니다.
    '
    Private Sub AdjustViewerHeight()
        ' ScrollFixLayoutPanel 한 줄에 몇 개의 아이템이 들어갈 수 있는가?
        Dim item_max As Integer = ScrollFixLayoutPanel1.Width \ ViewerWidth

        If item_max <= 1 Then
            Exit Sub
        End If

        Dim Max As Integer = 0
        For i As Integer = 0 To ViewerCollection.Count - 1
            Dim ucvw As ucViewer = ViewerCollection(i)
            Max = Math.Max(Max, ucvw.Height)
            If ((i + 1) Mod item_max) = 0 Then
                For j As Integer = i - item_max + 1 To i
                    ViewerCollection(j).Height = Max
                Next
                Max = 0
            ElseIf ViewerCollection.Count - 1 = i Then
                For j As Integer = (i - (i + 1) Mod item_max + 1) To i
                    ViewerCollection(j).Height = Max
                Next
                Max = 0
            End If
        Next
    End Sub

    Private Sub frmPicviewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IsFolderLoad = frmMain.IsFolderLoad
        Me.Show()
        AddImageLabelFolderDelegate = New DelegateAddImage(AddressOf AddImageLabelFolder)
        nowThread = New Threading.Thread(AddressOf AddThread)
        nowThread.IsBackground = True
        nowThread.Start(frmMain.PicViewerPictureList)

        '
        '   Very hard working
        '   Hyperbolic Tangent는 1로 수렴하는 함수이므로 적합함
        '
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Height = 0
        For i As Integer = 0 To 25
            Me.Location = New Point(custlocx - originw * frmMain.tanh_value(i) / 2, locationy)
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        Me.Location = New Point(locationx, locationy)
        Me.Width = originw
        For i As Integer = 0 To 22
            Me.Height = originh * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        Me.Height = originh

        '
        '   튕기는 효과 만들었는데 tanh랑 겹치는 부분에 생기는
        '   약간의 텀이 안없어져서 그냥 없애버림.
        '
        'Application.DoEvents()
        'For i As Integer = 17 To 0 Step -1
        '    Me.Height = originh - i * Math.Sin(i * 0.6) * 1.6
        '    Threading.Thread.Sleep(20)
        'Next
    End Sub
    Private Sub frmPicviewer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        nowThread.Abort()
        For Each ucvw As ucViewer In ViewerCollection
            ucvw.Dispose()
        Next
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        For i As Integer = 25 To 0 Step -1
            Me.Height = originh * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(custlocx - originw * frmMain.tanh_value(i) / 2, locationy)
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        frmMain.Focus()
    End Sub

    '
    '   컨트롤 키가 눌리고 마우스 휠이 돌아가는 상태인 경우 뷰를 확대 또는 축소합니다.
    '
    Private Sub frmPicviewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        AdjustViewerHeight()
    End Sub
    Private Sub ScrollFixLayoutPanel1_MouseWheel(sender As Object, e As MouseEventArgs) Handles ScrollFixLayoutPanel1.MouseWheel
        If ControlKeyDown Then
            If e.Delta > 0 Then
                ViewerWidth += 10
                For Each ucvw As ucViewer In ViewerCollection
                    ucvw.Width = ViewerWidth
                    ucvw.Height += 10
                    ucvw.Invalidate()
                    Application.DoEvents()
                Next
            Else
                If ViewerWidth >= 276 Then
                    ViewerWidth -= 10
                    For Each ucvw As ucViewer In ViewerCollection
                        ucvw.Width = ViewerWidth
                        ucvw.Height -= 10
                        ucvw.Invalidate()
                        Application.DoEvents()
                    Next
                ElseIf ViewerWidth = 266 Then
                    ' 원래 크기로
                    ViewerWidth = 256
                    For Each ucvw As ucViewer In ViewerCollection
                        ucvw.Width = ViewerWidth
                        ucvw.Height -= 10
                        ucvw.Invalidate()
                        Application.DoEvents()
                    Next
                End If
            End If
        End If
    End Sub

End Class