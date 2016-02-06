'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Public Class ScrollFixLayoutPanel

    Inherits FlowLayoutPanel

    Protected Overrides Function ScrollToControl(activeControl As Control) As Point
        Return DisplayRectangle.Location
    End Function

End Class
