   M S B A M L   `   `   `  ����I  EDutch Lady App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Dutch_Lady_App.MyUserControlDutch_Lady_App.MyUserControl  5      /    )Dutch_Lady_App.MyUserControl.UserControl2   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:Dutch_Lady_App.MyUserControl     5      X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      -   5        ���#F5DEB3.��5
      �   � ColumnDefinitions  5   
   �� 5      $
��407* �6   �� 5      $
��654* �6   �� 5      $
��569* �6    � RowDefinitions 5   
   �� 5      $	��24* �6   �� 5      $
��204* �6   �� 5      $
��527* �6   .+�5   
   �� -   5         ��Name$ Section1��5      $��1��6#   $��0��60    �� CornerRadius$ 21��5      $��3q�6#   
���   ����#F7D282$��	18,0,18,0q�5      ._�5      �   5      �� 5      �� 5      �� 5        5"      �� 5#      $��2* �6$   �� 5$      $��* �6$   �� 5%      $��* �6$   .+�5'      �� -   5         ��Name$ OneShot_Button��5'      $��0��61   $��0��6A      cameraButton#��[  6N   $��10,15q�6t   .��5(      �� $��/Images/1.ico��6   5)      �� 5*      -   5        $ Continuous_Button��5*      $��1��64   $��0��6D   #��[  6Q   $��10,15q�6w   .��5+      �� $��/Images/Video.ico��6   5,      �� 5-      -   5        $ Stop_Button��5-      $��2��6.   $��0��6>   #��[  6K   $��10,15q�6q   .��5/      �� $��/Images/Stop.ico��6   50      �� 52      -   5         ��Name$ Camera_status��52      $��1��63   $��3��6@   ���#F7D282$��Center=�6i   $��21(�53       �� TextAlignment$ Center=�6)   $
��Wrap=�6@   �� 54      $��3��6   $��3��6'   $��	24,0,24,6q�6<   $ 10��6O   $��3q�6a   
���   �._�55      �� -   5        $��	Camera_id��55      
������� �� TextAlignment$ Center=�6B   $��30(�56      $��SemiBold"�6'   .��6=    � 	Resources 57      	 �� 
TargetType
 �� Property �� Value��%_   '��       �� 58      "	 ��6$   .�59   "   �� 
 �@��CornerRadius$ 10��6A   5:      5;      .��6-   5<      5=      5>      5?      �� 5C   
   -   5        $ Section2��5C      $��2��6#   $��0��60   $ 21��5D      $��3q�6#   
���   ����#F7D282$��18,18,18,18q�5F      ._�5G      �   5H      �� 5I      �� 5J      �� 5K       5M      �� 5N      $
��0.7* �6$   �� 5O      $
��0.9* �6$   �� 5P      $
��0.9* �6$   �� 5Q      $
��0.4* �6$   .+�5S      �� -	   5        $ Draw_Button��5S      $��0��6.   $��0��6>     
menuButton#��[ 6K   $��10,25,10,25q�6o   .��5T      �� $��/Images/Add.ico��6   5U      �� 5V      -
   5        $ Save_Button��5V      $��1��6.   $��0��6>   #��[ 6K   $��10,25,10,25q�6o   .��5W      �� $��/Images/Save.ico��6   5X      �� 5Y      -   5        $��2��5Y      $��0��6)   #��[ 66   $��10,25,10,25q�6Z   .��5Z      �� $��/Images/Clear.ico��6   5[      �� 5]      $��1��6   $��3��6&   $��2��6:   ._�5^      �   5_      �� 5`      �� 5a       5c      �� 5d      �� 5e      $��2* �6,   �� 5f      �� 5g      $��2* �6,   �� 5h      �� 5i      $��2* �6,   .+�5k      �� $��0��6!   $��0��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5l   K   ._�5m      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��	Min edges��6h   
�������6y   5n      �� 5o      $��0��6!   $��1��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5p   K   ._�5q      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��	Max edges��6h   
�������6y   5r      �� 5s      $��2��6!   $��0��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5t   K   ._�5u      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��
Min radius��6h   
�������6z   5v      �� 5w      $��2��6!   $��1��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5x   K   ._�5y      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��
Max radius��6h   
�������6z   5z      %  Xceed.Wpf.Toolkit.IntegerUpDown  5{      -   5        $��	Min_edges��5{   -   $��1��6?   $
��12,3q�6L   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X�5|   -      Maximum$	 300X�69     5}      -   5        $��	Max_edges��5}   -   $��1��6?   $��1��6L   $
��12,3q�6\   $ 0X�5~   -   $	 300X�69     5      -   5        $��Min_ra��5   -   $��3��6<   $
��12,3q�6I   $ 1X�5�   -     5�      -   5        $��Max_ra��5�   -   $��3��6<   $��1��6I   $
��12,3q�6Y   $ 2X�5�   -   �� 5�      $��4��6!   $��0��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5�   K   ._�5�      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��Shrink seal��6h   
�������6{   5�      �� 5�      $��5��6!   $
��12,3q�6.   $ 10��6<   $��3q�6N   
���   �._�5�      �� -   5         ��Name$ Shrink_seal��5�   &   $ Center=�6;   $��30(�6R   .��6a    5�   "   ��%_   '��       �� 5�   &   "	 ��6,   .�5�   *   �� 
 �@��CornerRadius$ 10��6I   5�   '   5�   #   .��65   5�      5�      �� 5�      $��4��6!   $��1��6.   $ 9��6>   $��3q�6O   
���   ����#191E95$	��9,3q�5�   K   ._�5�      �� $��18(�6(   $ Center=�66   $��Center=�6M   $��Inner circle��6h   
�������6|   5�      �� 5�      $��5��6!   $��1��6.   $
��12,3q�6>   $ 10��6L   $��3q�6^   
���   �._�5�      �� -   5        $ Min_cir_ratio��5�   &   $ Center=�6=   $��30(�6T   .��6c    5�   "   ��%_   '��       �� 5�   &   "	 ��6,   .�5�   *   �� 
 �@��CornerRadius$ 10��6I   5�   '   5�   #   .��65   5�      5�      5�      5�      �� 5�      $��4��6   $��3��6&   $��3q�6:   
���   �$��30,10,30,10q�6b   $ 24��6w   ._�5�      �� -   5        $��Execute��5�      $��27(�6/   $��Center=�6=   $��DemiBold"�6_    5�      ��%_   '��       �� 5�      "	 ��6$   .�5�   "   �� 
 �@��CornerRadius$ 21��6A   5�      5�      5�      5�      5�      5�      �� 5�   
   
���   �$��1��6$   $��1��61   $��2��6A   $��
18,0,18,18q�6R   }�. ._�5�      �� -   5         ��Name$ canvasControl1��5�      ��5�      �� 5�      -   5        pictureBox1�� j� Stretch$
 Fill=�5�   6   5�      5�      �� 5�   
    �� Orientation$ Vertical=�6   $��2��6,   $��2��6<   $
��Left=�6I   $	��Top=�6d   $��0,0,10,0q�6|   .+�5�      �� -   5        $ Enable_3��5�      $��Enable��6'   
���2�2�$��25(�6O   $
��Bold"�6]   
���   �$��3q�5�      $
��12,3q�6-    5�      ��%_   '��       �� 5�      "	 ��6   .�5�      �� 
 �@��CornerRadius$ 10��69   5�      5�      5�      �� 5�      $��
Fristi top��6   
�������$��Center=�6<   $��Center=�5�      $��30(�6:   $��SemiBold"�6H   �� 5�      $ 10��6   $��1q�6'   
���   �
�������._�5�      �� -   5        $ get_form_top��5�      $��27(�62   $ Center=�6@   $��DemiBold"�6W   5�      �� 5�      $ 10��6   $��1q�6'   
���   �
�������$	��0,6q�6b   ._�5�      �� -   5        $ get_area_top��5�      $��27(�62   $ Center=�6@   $��DemiBold"�6W   5�      5�      �� 5�   
   -   5        $ Section5��5�      $��2��6#   $��1��63   $ 10��6@   $��3q�6R   
���   ����#F7D282$��
0,0,120,18q�5�   :   ._�5�      �   5�      �� 5�      �� 5�       5�      �� 5�      $	��78* �6$   �� 5�      $	��66* �6$   �� 5�      $	��55* �6$   �� 5�      $	��32* �6$   .+�5�      d� -   5         d�Name$ 
btnToggle1��5�        ToggleButton#��[ 63   $��40��6Y   $
��12,6q�6e   $��1��6�   �� 5�      -   5        $ Save_Button_1��5�      $��1��60   $
��Save��6@   $��27(�6O   $
��Bold"�6]   
���   �$��3q�5�      $
��12,6q�6-    5�      ��%_   '��       �� 5�      "	 ��6    .�5�      �� 
 �@��CornerRadius$ 10��6=   5�      5�      5�      �� 5�      $��1��6   $ 9��6&   $��3q�67   
���   �$��
12,9,12,15q�5�   -   ���#191E95._�5�      �� $��18(�6    $ Center=�6.   $��Center=�6E   $��
Nhăn seal��6`   
�������6q   5�      �� 5�      $��1��6   $��1��6&   $ 9��66   $��3q�6G   
���   �$��
12,9,12,15q�5�   -   ���#191E95._�5�      �� $��18(�6    $ Center=�6.   $��Center=�6E   $��Độ tròn��6`   
�������6o   5�      �� 5�      -   5        $ Min_accuracy_lid_txb��5�      $��2��67   $��0��6D   $��	12,0,12,0q�6T   $ 10��6g   $��3q�6y   
���   �._�5�      �� -   5        $ Lim_area_top��5�      $ Center=�64   $��30(�6K   .��6Z    5�      ��%_   '��       �� 5�      "	 ��6$   .�5�   "   �� 
 �@��CornerRadius$ 10��6A   5�      5�      .��6-   5�      5�      �� 5�      $��2��6   $��1��6&   $��	12,0,12,0q�66   $ 10��6I   $��3q�6[   
���   �._�5�      �� -   5        $ Lim_form_top��5�      $ Center=�64   $��30(�6K   .��6Z    5�      ��%_   '��       �� 5�      "	 ��6$   .�5�   "   �� 
 �@��CornerRadius$ 10��6A   5�      5�      .��6-   5�      5�      5�      5�      �� 5�   
   $ Vertical=�6   $��2��6,   $��1��6<   $��Right=�6I   $	��Top=�6e   $��0,0,6,0q�6}   $��0��6�   $��0��6�   5      5     