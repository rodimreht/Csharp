@echo off
Robocopy . D:\Projects\C#.NETs\oBrowser2\ /MIR /XF oBrowserSrcSync.cmd UpgradeLog.XML /XD _UpgradeReport_Files src_backup_* /NP /LOG+:.\%1-Detail.log
