   M S B A M L   `   `   `  ����G  CLoraCardGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:LoraCardGameLoraCardGame  5   	   G(clr-namespace:LoraCardGame.User_ControlsLoraCardGame.User_Controls  5   	       LoraCardGame.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:LoraCardGame     5   	   ;local1(clr-namespace:LoraCardGame.User_Controls     5   	   -   5          � Title$
  Lora��5	   	   $	��650��6   $
��1024��6#   $	��610��60   $	��910��6@   ���	#FB105120.��5
      � .+�5   
   ]� .+�5      o� $	��Top=�6   .@�5      m� $��_Game��6   .@�5      m� -   5        $��Start��5      $��_Start new game��6,   6E   m� 5      -   5        $
��Join��5      $��
_Join game��6+   6?   �� 5      m� 5      $��_Close��6   5      5      �� 5      6   5      �� 5   
   $��1q�6   
���   �$	��130��69   $	��460��6F    �� CornerRadius$ 5,5,5,5��6R   $��45,395,45,45q�6i   $��Bottom=�6   ._�5      �� $	��120��6   $��20,0,0,0q�6&   $��Center=�68   $��Center=�6U   .+�5      �� -   5        $��CardsItemsControl��5      ��68   ��  �� Path$ Cards���5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6)   $��20,0,0,0q�6B   5      ��5      �� 5       .�5!      ��  �� Property � �� �� Value$ 	-40,0,0,0q�6H   5"      5$      5%      5&      �� 5(   
   $��1q�6   
��� ���$	��130��68   $	��400��6E   $ 5,5,5,5��6Q   $��0,0,0,50q�6h   ._�5)      �� $��Center=�6   $	��120��66   $��Center=�6C   .+�5*      �� -   5        $��CardsOnTableItemsControl��5*      ��6?   �� $ CardsOnTable���5+      �� 5,      .R�5-      �� $ 
Horizontal=�6)   5.      ��50      �� 51      .�52      ��  � ��$ 5,0,0,0q�6H   53      55      56      57      �� 59   
   -   5         ��Name$ 	textBlock��59      $
��Left=�6'    �� TextAlignment$ Center=�6B   $��305,160,0,0q�6Y   $
��Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��400��6�   '   !LoraCardGame.User_Controls.Player  5;   
   -   5          Name$ Player1��5;      $
��Left=�6)   $��Center=�6D   $��	30,0,0,50q�6_     5<   
   -   5        $ Player2��5<      $��Right=�6)   $��Center=�6E   $��	0,0,30,50q�6`     5=   
   -	   5        $ Player3��5=      $	��Top=�6)   $��Center=�6A   $��0,30,0,0q�6^   �� 5?   
   -
   5        	 ��Name$	 buttonStart��5?      $��
Start game��6&   $
��Left=�6;   $��470,260,0,0q�6V   $	��Top=�6k   $	��100��6�   $��50��6�   $y�	Collapsed=�6�   5A      5B      