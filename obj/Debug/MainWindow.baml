   M S B A M L   `   `   `  ����D  @FtpClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:FtpClient	FtpClient  5   	       FtpClient.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:FtpClient     5   	     � Title$  
MainWindow��5   	   $
��1000��6   $
��1200��6*   .��5	      � .+�5
   
   ��  �� Orientation$ Vertical=�6   $��30q�6,   .+�5      �� $ 
Horizontal=�6   .+�5      �� -   5         ��Name$ uriValue��5      $��30��6o   $	��200��6{   ��6,   �� *BaseUriValue+ �� UpdateSourceTrigger$ PropertyChanged=�6{   �� 5      -   5        $ username��5      $��30��6k   $	��200��6w   ��6,   �� *
Username+$ PropertyChanged=�6w   �� 5      -   5        $ password��5      $��30��6k   $	��200��6w   ��6,   �� *
Password+$ PropertyChanged=�6w   �� 5      $��Connect��6   $��30��6V   $	��200��6b   ��6+   �� *ConnectToServerCommand+6b   5      �� 5      -   5        $	��300��5   =   ��6   �� *ItemsToDisplay+ �� SelectedItem 6o   �� *SelectedFile+ �� Mode$ TwoWay=�5      �� 5      $��Download��6   $��30��6P   $	��100��6\   ��6(   �� *DownloadFileCommand+6\   �� 5       �� 
IsReadOnly . $��Stretch=�6M    �� TextWrapping$
 Wrap=�6k   $	��300��6   $ �� HorizontalScrollBarVisibility$
 Auto=�6�   ��6(   �� *Output+$ TwoWay=�6�   �� 5      $ 
Horizontal=�6   .+�5      �� -   5        $ SavingPathBox��5       . $��30��6l   $	��200��6x   ��6C   �� *
SavingPath+$ TwoWay=�6x   �� 5      -   5        $��Change��5      6*   5      5      5      5      