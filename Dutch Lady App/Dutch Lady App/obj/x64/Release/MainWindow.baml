   M S B A M L   `   `   `  ���� I  EDutch Lady App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Dutch_Lady_AppDutch_Lady_App  K*clr-namespace:Dutch_Lady_App.MyUserControlDutch_Lady_App.MyUserControl      Dutch_Lady_App.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:Dutch_Lady_App     9UC*clr-namespace:Dutch_Lady_App.MyUserControl     -     � WindowStyle$  ThreeDBorderWindow=� � WindowState$ 	Maximized=�$	��800��$
��1120�� � Title$ 
MainWindow��
����� �.���  � RowDefinitions �� $��3* ��� $	��46* � � ColumnDefinitions �� $
��977* ��� $
��323* �.+��� -    ��Name$ 
TabControl��$��0��$��2��$��0��$��2��.@��� $
��Home��$��SemiBold"�$��18(�.��� ���#F5DEB3 �� $
��1.2* ��� $��9* �.+��� ���#0a7e8c$��-16,12,12,12q� �� CornerRadius$ 10��._��  �� $
��0.2* ��� $
��0.3* ��� �� $
��0.3* ��� �� $
��0.3* ��� �� $
��0.3* ��� �� $
��0.3* ��� $
��0.6* ��� $
��1.5* �.+��� $ 10��$��3q�$��Center=�$��1��
���   ����#191E95$��15,0,0,0q�._��� $��18(� �� TextAlignment$ Center=�$��Center=�$��  Tổng NG  ��
��������� -   $��Number_NG_total��$��40(�$��2��
���  ��$ Center=�$��Center=�$
��Wrap=�$��Center=����� *count_NG_total+   
NG_Convert �� 	Converter# [  �� $ 10��$��3q�$��Center=�$��3��
���   ����#191E95$��15,0,0,0q�._��� $��18(�$ Center=�$��Center=�$��  Nắp xẹp  ��
��������� -   $��Number_NG_side��$��40(�$��4��
���  ��$ Center=�$��Center=�$
��Wrap=�$��Center=����� 	 �� ElementName$	 User1��
 �� Path$
 count_NG_side�# [  �� $ 10��$��3q�$��Center=�
���   ����#191E95$��5��$��15,0,0,0q�._��� $��18(�$ Center=�$��Center=�$��  Thiếu sữa  ��
��������� -   $��Number_NG_water��$��40(�$��6��
���  ��$ Center=�$��Center=�$
��Wrap=�$��Center=����� $	 User1��$
 count_NG_water�# [  �� $ 10��$��3q�$��Center=�
���   ����#191E95$��7��$��15,0,0,0q�._��� $��18(�$ Center=�$��Center=�$��  Nắp nhăn  ��
��������� -   $��Number_NG_top��$��40(�$��8��
���  ��$ Center=�$��Center=�$��Center=�$
��Wrap=����� $	 User2��$
 count_NG_top�# [  �� $ 10��$��3q�$��Center=�
���   ����#191E95$��9��$��15,0,0,0q�._��� $��18(�$ Center=�$��Center=�$��  Đồ họa  ��
�������d� -    d�Name$ 
btnToggle1��  ToggleButton#��[ $��40��$��	35,0,15,0q�$��Bottom=�$��10���� -   $��RESET��$��20(�$
��Bold"�$��11��$��20,20q�
���   �$��3q� � 	Resources  �� 
TargetType �� Property �� Value��%:   '��       �� " ��.���  �@��CornerRadius$ 10���� $��1��._��  �� $��6* ��� $
��0.6* ��� $
��0.6* ��� $
��0.6* � �� �� �� �� �� �� .+��� 
���   �$��2��$��0,0,12,0q�}�. ._��� -	    ��Name$ canvasControl1������ -
   pictureBox1�� j� Stretch$
 Fill=��� 
���   �$��2��$��2��$��0,0,12,0q�}�. ._��� -   $ canvasControl2������ -   pictureBox2��$
 Fill=��� 
���   �$��4��$��2��$��0,0,12,0q�}�. ._��� -   $ canvasControl3������ -   pictureBox3��$
 Fill=��� $��Nắp xẹp��$ Center=�$��25(�$��Center=�
����������#191E95$��1��$��2��$��0,0,12,0q��� $��Thiếu sữa��$ Center=�$��25(�$��Center=�
����������#191E95$��1��$��2��$��2��$��0,0,12,0q��� $��Nắp nhăn��$ Center=�$��25(�$��Center=�
����������#191E95$��1��$��4��$��2��$��0,0,12,0q��� -   $��
Status_lid��$��Center=�
�������$��2��$��2��$��	0,0,12,12q�$��Center=�$��60(�$
��Bold"����� $	 User1��$
 lid_OK_color�  String_Convert# [ ���� $	 User1��$
 lid_OK_color�  Color_Convert# [ �� -   $��Status_water��$��Center=�
�������$��2��$��2��$��2��$��	0,0,12,12q�$��Center=�$��60(�$
��Bold"����� $	 User1��$
 water_OK_color�# [ ���� $	 User1��$
 water_OK_color�# [ �� -   $��
Status_top��$��Center=�
�������$��2��$��4��$��2��$��	0,0,12,12q�$��Center=�$��60(�$
��Bold"����� $	 User2��$
 top_OK_color�# [ ���� $	 User2��$
 top_OK_color�# [ �� $��1��$��2��$��	0,0,12,12q�
�������
���   �$��2q�$ 15��._��� -   $��lid_NG_value��
���  ��$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User1��$
 lid_NG_value�  Value_Convert# [ �� $��1��$��3��$��	0,0,12,12q�
�������
���   �$��2q�$ 15��._��� -   $��lid_OK_value��
��� � �$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User1��$
 lid_OK_value�# [ �� $��3��$��2��$��	0,0,12,12q�
�������
���   �$��2q�$ 15��._��� -   $��water_NG_value��
���  ��$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User1��$
 water_NG_value�# [ �� $��3��$��3��$��	0,0,12,12q�
�������
���   �$��2q�$ 15��._��� -   $��water_OK_value��
��� � �$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User1��$
 water_OK_value�# [ �� $��2��$��5��$��2��.+��  �� ��  �� �� .+��� $��0,0,3,12q�
�������
���   �$��2q�$ 15��._��� -   $��top_NG_value��
���  ��$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User2��$
 top_NG_value�  Value_Convert_Int# [ �� $��1��$��0,0,3,12q�
�������
���   �$��2q�$ 15��._��� -   $��top_OK_value��
��� � �$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User2��$
 top_OK_value�# [ �� $��1��$��0,0,3,12q�
�������
���   �$��2q�$ 15��._��� -   $��form_NG_value��
���  ��$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User2��$
 top_NG_form_value�# [ �� $��1��$��1��$��0,0,3,12q�
�������
���   �$��2q�$ 15��._��� -   $��form_OK_value��
��� � �$ Center=�$��Center=�$
��Bold"�$��30(����� $	 User2��$
 top_OK_form_value�# [ �� $��Camera 1��$��SemiBold"�$��18(�.��/   )Dutch_Lady_App.MyUserControl.UserControl1  -     Name$ User1��$��0��$��1��$��0,0,0,0q��� $��Camera 2��$��SemiBold"�$��18(�.��/   )Dutch_Lady_App.MyUserControl.UserControl2  -     Name$ User2��$��0��$��1��$��0,0,0,0q��� $��0��$��1��._��  �� �� .+��� -    ��Name$ 
Run_Button��$	��Run��  
ModeButton#��[ �� -   $ Edit_Button��$��1��$
��Edit��#��[ 