   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   4    MoetoOro   MoetoOro.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   4     � Title$  Моето оро��6	   $	��752��6   $	��950��6(    � 
ResizeMode$ CanMinimize=�6�    � ShowInTaskbar .  � WindowStartupLocation$ CenterScreen=�6�    � Icon$+ %/MoetoOro;component/Images/igraci.jpg��6�    � 	Resources 5         InfoMessageAnimate �� 
TargetName �� TargetProperty g� Duration	 W� From	
 W� To��%�   &	        �� 5   
   $ InformationLabel��60   .��5      W� $ Opacity�6   $ 00:00:03D�6B   $	 0X�6V   $
 1X�6_   5      5	      .��5
      � .+�5   
   �� -   5        $��image1��5   I   $	��714��6   $
��Left=�6   $��0,0,0,0q�68   $
��Fill=�6W   $	��Top=�6f   $	��950��6~   $0��*/MoetoOro;component/Images/pozadina_21.jpg��6�    �� StretchDirection$
 Both=�6�   �� 5   
   $��40,177,356,369q�6   .+�5      ��  �� Left$ 6��6   
�������
 �� Top$ 6��6;   $��53��6J   $��18(�6W   $����Апликацијата моментално ги опфаќа следните ора: Рамното оро, Пајдушко оро, 6 напред 3 назад��6e   $	��520��6�   $
��Wrap=�6�   �� 5      $ 6��6   
�������$ 65��6;   $��53��6K   $��18(�6X   $����За крај на играњето излезете од полето на видик или кренете ги двете раце нагоре��6f   $	��520��6�   $
��Wrap=�6�   �� 5      $ 6��6   
�������$	 136��6;   $��53��6L   $��18(�6Y   $9��3Изработил: Дарко Мартиновиќ��6g   $	��520��6�   $
��Wrap=�6�   5      �� 5   
   $��635,160,0,0q�6   .+�5      �� -   5        $��InformationLabelTop��5   q    J� 	AllowDrop . $3��-Последно препознаено орo��6%   $��32��6H   $��Center=�6T   $	��Top=�6�   $��18(�6�   $	��268��6�   
�������$��Normal"�6�   $��Normal&�6�   $��0q�6�   $��Center=�6  $ 0��6*  $ 0��6:  6K  �� 5      -   5        $��InformationLabel��5   N    . $��31��6%   $��Center=�61   $	��Top=�6f   $��15(�6~   $	��200��6�   ���	#FFB70707$
��Bold"�6�   ���	#FFFFCB00$��Normal&�6�   $��0q�6�   $��Center=�6�   $ 33��6  $ 32��6.  $��Center=�6>  6b  �� 5      -   5        $��OnePeriodCanvas��5   P   $	��100��6    . $��Center=�63   $	��200��6g    � UseLayoutRounding . 
�������$��Center=�6�   $ 33��6�   $ 62��6�   6�   5      �� 5   
   $��337,398,0,0q�6   .+�5      �� $ 0��6   {�. $8��2Играчи во видик на сензорот��64   $ 0��6Z   $��35��6j   $	��500��6v   ���	#FFC7C5C2$��20(�6�   $��Center=�6�   $��Center=�6�   $��Center=�6�   $��Center=�6  ���	#FFBA00006>  �� 5      -   5        $��SkeletonCanvas��5   N    . $	��250��6&   $
��Left=�63   $	��Top=�6d   $	��500��6|    J� OpacityMask
 ����� � OverridesDefaultStyle .  .  $ 0��6�   $ 32��6�   ��5      �� 5       �� EndPoint$ 0.5,1&�6*    �� 
StartPoint$ 0.5,0&�6;   .��5      �  � Color$ 	#FFCE4D4D��6'    � Offset$ 0.148Q�69   � 5      $ 	#FFFC9898��6'   $ 1Q�69   5      5      �� 5       -   5        $��NoDancerPicture��5    $   $ 0��6   $ 32��6;   $2��,/MoetoOro;component/Images/poraka_senzor.jpg��6K   $	��500��6�   $	��250��6�   6�   5!      =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5"      -   5        $��kinectSensorChooser��5"   B    . $	��207��65   $	��363��6]   $	 125��6i   $	 126��6{   5#      5%      