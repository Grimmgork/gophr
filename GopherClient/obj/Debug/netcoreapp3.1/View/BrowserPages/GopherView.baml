   M S B A M L   `   `   `  ����G  CGopherClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:GopherClient.View.BrowserPagesGopherClient.View.BrowserPages  5      Y1clr-namespace:GopherClient.ViewModel.BrowserPages#GopherClient.ViewModel.BrowserPages  5      /    )GopherClient.View.BrowserPages.GopherView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      >local,clr-namespace:GopherClient.View.BrowserPages     5      @rt1clr-namespace:GopherClient.ViewModel.BrowserPages     5      -   5        
���   �$
��True��5      $
|�True��6     J� InputBindings  5      �� 5   
   
 �� Key$ Up��6    �� Command 6   �� *SelectUpCommand+�� 5   
   $
 Down��6    6    �� *SelectDownCommand+�� 5   
   $ Up��6    �� 	Modifiers$ Shiftf�6    60   �� *!SelectNextInteractableUpCommand+�� 5   
   $
 Down��6   $ Shiftf�6     62   �� *#!SelectNextInteractableDownCommand+�� 5   
   $ Return��6   $ Shiftf�6"    64   �� *ExecuteSelectedElementCommand+�� 5   
   $ Return��6    6"   �� *ExecuteSelectedElementCommand+�� 5   
   $ Return��6   $	 Altf�6"    62   �� *&$DownloadSelectedElementToDiskCommand+ � 	Resources 5       �� DataType7   1GopherClient.ViewModel.BrowserPages.GopherElement J� SnapsToDevicePixels � ToolTip �� IsAsync	 �� Mode
 �� Orientation   
SymbolFont � ContextMenu �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value �� Value �� Binding �� Path��%

  (��       5      *, +)7   �� 6
   "  6   .R�5      �� $
��back��6   -   5        
���   �$ False��5   �    6^   �� *url+$
 True��$	 OneTime=�._�5      �� $
 
Horizontal=�6   .+�5      �� $
��icon��6    $��15��6,   $��Center=�67   8��  $��15��6{   $��20,0,5,0q�6�   
����=H�6�   �� 5       $
��text��6    
�������$��15(�6q   ��6,   �� *text+$
 True��$	 OneTime=�6q   5!       5"      �� 5#      .@�5$      m� -   5        $��Download to File��5$   #   6=   5%      5'       5)      P� 5*       �@J�IsMouseOver$
 True��61   . �5+      �� $
 back��6    � ��
 ��  �6E   5,      �� 5.      $
 true��6P    6   �� $ 
IsSelected�$
 True��.a�5/      �� $
 back��6    � ��
 ��  �6E   50      �� 52      $ 1��6J    6   �� $
 type�$
 True��.a�53      �� $
 icon��6    � ��$	 ��6I   54      �� 56      $ 0��6J    6   �� $
 type�$
 True��.a�57      �� $
 icon��6    � ��$	 ��6I   58      �� 5:      $ g��6J    6   �� $
 type�$
 True��.a�5;      �� $
 icon��6    � ��$	 ��6I   5<      �� 5>      $ 9��6J    6   �� $
 type�$
 True��.a�5?      �� $
 icon��6    � ��$	 ��6I   5@      �� 5B      $ I��6J    6   �� $
 type�$
 True��.a�5C      �� $
 icon��6    � ��$	 ��6I   5D      �� 5F      $ 7��6J    6   �� $
 type�$
 True��.a�5G      �� $
 icon��6    � ��$	 ��6I   5H      �� 5J      $ h��6J    6   �� $
 type�$
 True��.a�5K      �� $
 icon��6    � ��$	 ��6I   5L      �� 5N      $ .��6J    6   �� $
 type�$
 True��.a�5O      �� $
 icon��6    � ��$	 ��6I   5P      �� 5R      $ false��6T    6   �� $ IsInteractable�$
 True��.a�5S      �� $
 text��6    � ��
 �����6O   5T      �� 5V      $ i��6J    6   �� $
 type�$
 True��.a�5W      �� $
 icon��6    � y�$ Hidden=�6E   �� 5X      $
 text��6    � ��
 �����6O   �� 5Y      $
 back��6    6I   Z�  �@�ToolTip5Z      �� 5\      $ 3��6J    6   �� $
 type�$
 True��.a�5]      �� $
 icon��6    � ��$	 ��6I   �� 5^      $
 icon��6    � ��
 �  ��6O   �� 5_      $
 text��6    � ��
 �  ��6O   �� 5`      $
 back��6    6I   �� *source+$
 True�� �@�ToolTip5a      5c      5d      .��5f      ]� .+�5h   
   �� -   5        $��GopherElements��5h       %� IsVirtualizing$
 True��6\   $��Visible=�6�   ��6-   �� *
Elements+$
 True����5i      �� 5j      .R�5k      $� $*��$GopherElementsVirtualizingStackpanel��6-   $
 True��6Y    %� VirtualizationMode$ 	Recycling=�5l      5m      5n      ��5q      �� 5r       �� 
TargetType" ��6"   .R�5s      �� #��z��6   #��z��5t      #��z��5u      ._�5v      �� $
��True��6'   #��z��5w      $|�False��5x      .��5y      �� 5z      5{      5|      5~      5      5�      