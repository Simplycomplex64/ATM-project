   M S B A M L   `   `   `  ����E  AAtmProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:AtmProject
AtmProject  5   	       AtmProject.depositWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:AtmProject     5   	   -   5          �Name$  depositWindow1��5   	    � Title$ depositWindow��5   	   $	��450��6   $	��806��6,    � 
ResizeMode$ NoResize=�68    � WindowStartupLocation$ CenterScreen=�6N    � WindowStyle$
 None=�6s   $��5,5,5,5q�6�   ��5	      �� 5
   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��5      �  � Color$ 	#FFCFD2D5��6   � 5      $ 	#FF5394D6��6    � Offset$ 0.559Q�6-   5      .��5      � .+�5   
   �� -   5        	 ��Name$	 label��5      $��	Transfert��6   $
��Left=�63   $��84��6N   $��	282,0,0,0q�6Z   $	��Top=�6m   $	��250��6�   $��60(�6�   $%��Segoe UI Variable Text Semibold)�6�   � 5   
   -   5        
 �Name$
 formGrid��5      $
��Left=�6!   $	��279��6<   $��	98,99,0,0q�6I   $	��Top=�6\   $	��618��6t   ��5      
� 5      .��5      � $ 	#FFE2E8EA��6#   � 5      $ 	#FF82B2C9��6#   $ 1Q�65   5      .+�5      �� -   5        $	 fromLbl��5      $��From : ��6%   $
��Left=�67   $��37��6R   $��	22,71,0,0q�6^   $	��Top=�6q   $��80��6�   $��20(�6�   $&�� Segoe UI Variable Small Semibold)�6�   �� 5      -   5        $	 toLbl��5      $��To : ��6#   $
��Left=�63   $��37��6N   $��22,0,0,0q�6Z   $��Center=�6l   $��80��6�   $��20(�6�   $&�� Segoe UI Variable Small Semibold)�6�   �� 5      -   5        $	 fromLbl_Copy��5      $��Please select accounts��6*   $��Center=�6K   $��37��6h   $��0,10,0,0q�6t   $	��Top=�6�   $	��220��6�   $��20(�6�   $&�� Segoe UI Variable Small Semibold)�6�   �� 5      -   5         ��Name$ 	submitBtn��5      $��Submit transfer��6(   $��Center=�6B   $��2��6_   $��	0,221,0,0q�6j   $	��Top=�6}   $��44��6�   $��15(�6�   $��2q�6�   ���	#FF100E0E$��Palatino Linotype)�6�   $��1,1,1,1q�6�   $
��Bold"�6   � FocusVisualStyle 5      �� 5      ��5      �� 5        �� 
TargetType" ��6   .�5!      ��  �� Property � �� �� Value 5"      �� 5#   "    �� 
TargetType" ��62   .R�5$   &   ��  ��Name$ border��6-   #��z��6=   #��z��6q   #��z��6�    �� CornerRadius$ 5��6�   $��0,0,-80,-31q�6�   ._�5%   *   �� $��Center=�6;   $��Center=�6X   5&   '    �� Triggers 5'   &   P� 5(   *    P� Property �@J�IsMouseOver P� Value . . �5)   .   ��  �� 
TargetName$ border��65    � ��
 � � �6_   �� 5*   .   $ border��65    � ��
 �����6k   5+   +   5-   #   5/      50      52      �� 53      -   5        $ 	cancelBtn��53      $��Cancel��6(   $
��Left=�69   $��2��6T   $��472,220,0,0q�6_   $	��Top=�6t   $��14��6�   $��15(�6�   $��2q�6�   ���	#FF100E0E$��Palatino Linotype)�6�   $��1,1,1,1q�6�   $
��Bold"�6   54      �� 55      ��57      �� 58      " ��6   .�59      ��  � �� 5:      �� 5;   "   " ��62   .R�5<   &   �� $ border��6-   #��z��6=   #��z��6q   #��z��6�   $ 5��6�   $��0,0,-80,-31q�6�   ._�5=   *   �� $��Center=�6;   $��Center=�6X   5>   '    5?   &   P� 5@   *    �@J�IsMouseOver . . �5A   .   �� $ border��65    � ��
 �  ��6_   �� 5B   .   $ border��65    � ��
 �����6k   5C   +   5E   #   5G      5H      5J      �� 5K      -	   5         ��Name$ amountTextBox��5K      $
��Left=�6-   $��30��6H   $��149,172,0,0q�6T    �� TextWrapping$
 Wrap=�6i   $	��Top=�6}   $��91��6�   $��17(�6�   $��Segoe UI Variable Small)�6�   $��Italic$�6�   �� 5L      -
   5        $	 amountLabel��5L      $��Amount :   $��6)   $
��Left=�6@   $��37��6[   $��
22,169,0,0q�6g   $	��Top=�6{   $	��127��6�   $��20(�6�   $&�� Segoe UI Variable Small Semibold)�6�   5M      �� 5N   
   -   5         ��Name$ DepositFromComboBox��5N      $
��Left=�60   $��28��6K   $��240,176,0,0q�6W   $	��Top=�6l   $	��311��6�   6�   �� 5O   
   -   5        $ DepositToComboBox��5O      $
��Left=�6.   $��28��6I   $��240,225,0,0q�6U   $	��Top=�6j   $	��311��6�   6�   5Q      5R      