   M S B A M L   `   `   `  ����G  Cwcommsixwrap, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null+clr-namespace:wcommsixwrapwcommsixwrap  5      &     wcommsixwrap.UpdateHandlerWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:wcommsixwrap     5      -   5        $��winUpdateHandler��5      $	��450��6&   $	��500��63     � 
ResizeMode$  CanMinimize=�6?    � Title$ App is updating��6X   $y�Visible=�6p    � WindowStyle$ ThreeDBorderWindow=�6�    � WindowStartupLocation$ CenterScreen=�6�    � Topmost .  � Icon$A ;pack://application:,,,/wcommsixwrap;component/download1.ico��6"  .��5	      � .+�5   
   � -   5        $��prgProgress��5      $��Center=�6)   $��19��6F   $��	0,230,0,0q�6R   $	��Top=�6e   $	��400��6}    � Maximum$ 1Q�6�   �� 5   
   -   5        $��
lblHeading��5      $��App is updating...��6"   $��Center=�6?   $��0,94,0,0q�6\   $	��Top=�6n   $��16(�6�   $
��Bold"�6�   $	��400��6�   �� 5   
   -   5        $��
lblMessage��5      $
��Wrap=�6&   $-��'You may start the app once we are done.��6:   $
��Left=�6i   $��55��6�   $��
50,130,0,0q�6�   $	��Top=�6�   $	��400��6�   {�.   �� TextAlignment$
 Left=�6�   5      5      