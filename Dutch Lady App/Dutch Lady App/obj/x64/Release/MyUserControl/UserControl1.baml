   M S B A M L   `   `   `  ���� I  EDutch Lady App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Dutch_Lady_AppDutch_Lady_App  /    )Dutch_Lady_App.MyUserControl.UserControl1   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .localclr-namespace:Dutch_Lady_App     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -   ���#F5DEB3.���   � ColumnDefinitions  �� $
��407* ��� $
��612* ��� $
��612* � � RowDefinitions �� $	��24* ��� $
��204* ��� $
��527* �.+��� -    ��Name$ Section1��$��1��$��0�� �� CornerRadius$ 21��$��3q�
���   ����#F7D282$��	18,0,18,0q�._��   �� �� ��  �� $��2* ��� $��* ��� $��* �.+��� -    ��Name$ OneShot_Button��$��0��$��0��   cameraButton#��[  $��10,15q�.���� $��/Images/1.ico���� -   $ Continuous_Button��$��1��$��0��#��[  $��10,15q�.���� $��/Images/Video.ico���� -   $ Stop_Button��$��2��$��0��#��[  $��10,15q�.���� $��/Images/Stop.ico���� -    ��Name$ Camera_status��$��1��$��3�����#F7D282$��Center=�$��21(� �� TextAlignment$ Center=�$
��Wrap=��� $��3��$��3��$��	24,0,24,6q�$ 10��$��3q�
���   �._��� -   $��	Camera_id��
������� �� TextAlignment$ Center=�$��30(�$��SemiBold"�.�� � 	Resources 	 �� 
TargetType
 �� Property �� Value��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10��.���� -   $��Section2��$��2��$��0��$ 21��$��3q�
���   ����#F7D282$��18,18,18,18q�._��   �� �� ��  �� �� �� $��2* �.+��� -	   $ Draw_Button��$��0��$��0��  
menuButton#��[ $��10,25q�.���� $��/Images/Add.ico���� -
   $ Save_Button��$��1��$��0��#��[ $��10,25q�.���� $��/Images/Save.ico���� -   $ Clear_Button��$��2��$��0��#��[ $��10,25q�.���� $��/Images/Clear.ico���� $��1��$��3��._��   �� ��  �� �� .+��� $��Threshold 1��$��SemiBold"�$��18(�$��Center=�$��Center=��� $��Threshold 2��$��1��$��SemiBold"�$��18(�$��Center=�$��Center=�%  Xceed.Wpf.Toolkit.IntegerUpDown  -   $��
Threshold1��$��1��$��	12,0,12,0q��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X�   Maximum$	 255X�  -   $��
Threshold2��$��1��$��2��$��	12,0,12,0q�$ 0X�$	 255X��� $��2��$��3��._��  �� $
��0.2* ��� �� $
��0.8* ��� �� $
��0.8* ��� �� $
��0.2* �.+��� -   $��GetArea_Button��$��1��$��Get area��$��24(�$
��Bold"�
���   �$��3q�$
��30,0q� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 21���� -   $��Label_area1��$��3��$��2��$��18(�$��DemiBold"�$��Fristi lid area:���� $
��18,0q�$��3��$��3��$ 10��$��1q�
���   �
�������._��� -   $ get_area_lid�����#813E3E$��27(�$ Center=�$��DemiBold"��� -   $��Label_area2��$��3��$��4��$��18(�$��DemiBold"�$��Fristi water area:���� $
��18,0q�$��3��$��5��$ 10��$��1q�
���   �
�������._��� -   $ get_area_water��$��27(����#813E3E$ Center=�$��DemiBold"��� 
���   �$��2��$��1��$��
18,0,18,18q�}�. ._��� -    ��Name$ canvasControl1������ -   pictureBox1�� j� Stretch$
 Fill=��� 
���   �$��2��$��2��$��
18,0,18,18q�}�. ._��� -   $ canvasControl2������ -   pictureBox2��$
 Fill=��� -   $ Section4��$��1��$��1��$ 21��$��3q�
���   ����#F7D282$��24,0,150,18q�._��   �� ��  �� $	��78* ��� $	��66* ��� $	��55* ��� $	��32* �.+�  -   $��	delay_txt��$
��20,6q�,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$ Center=�
�������$��27(�$ 1X��� -   $ Open_Button��$��1��$
��Open��$��27(�$
��Bold"�
���   �$��3q�$
��12,6q� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10���� $��1��$��2��$��3q�
���   �$��
12,9,12,15q����#191E95._��� $��21(�$ Center=�$��Center=�$��Max lid difference��
��������� $��2��$��0��$��2��$��	12,0,12,0q�$ 10��$��3q�
���   �._��� -    ��Name$ Higher_score_lid��$ Center=�$��27(�.�� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10��.����  �� Orientation$ Vertical=�$��1��$��1��$��Right=�$��Bottom=�$��0,0,6,24q�.+��� -   $ Enable_1��$��Enable��
���2�2�$��25(�$
��Bold"�
���   �$��3q�$
��12,3q� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10���� $��3q�
���   ����#191E95$ 10��._��� $��18(�$ Center=�$��Center=�$	��Min��
��������� $ 10��$��1q�
���   �._��� -   $ Lower_score_lid��$��23(�$ Center=�$��DemiBold"�.�� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10��.���� $�� Fristi lid ��
�������$��Center=�$��Right=�$	��Top=�$��27(�$��SemiBold"��� -   $ Section5��$��2��$��1��$ 21��$��3q�
���   ����#F7D282$��24,0,150,18q�._��   �� ��  �� $	��78* ��� $	��66* ��� $	��55* ��� $	��32* �.+�d� -    d�Name$ 
btnToggle1��  ToggleButton#��[ $��40��$��6q�$��1���� -   $ Save_Button_1��$��1��$
��Save��$��27(�$
��Bold"�
���   �$��3q�$
��12,6q� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10���� $��1��$��2��$��3q�
���   �$��
12,9,12,15q����#191E95._��� $��21(�$ Center=�$��Center=�$��Min water difference��
��������� $��2��$��0��$��2��$��	12,0,12,0q�$ 10��$��3q�
���   �._��� -    $ Min_area_water��$ Center=�$��27(�.�� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10��.���� $ Vertical=�$��2��$��1��$��Right=�$��Bottom=�$��0,0,6,24q�.+��� -!   $ Enable_2��$��Enable��
���2�2�$��25(�$
��Bold"�
���   �$��3q�$
��12,3q� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10���� $��3q�
���   ����#191E95$ 10��._��� $��18(�$ Center=�$��Center=�$��	Sensitive��
��������� $ 10��$��1q�
���   �._��� -"   $ Ratio_otsu_water��$��23(�$ Center=�$��DemiBold"�.�� ��%:   '��       �� "	 ��.��� 
 �@��CornerRadius$ 10��.���� $��
Milk level��
�������$��Center=�$��Right=�$	��Top=�$��Center=�$��27(�$��SemiBold"�