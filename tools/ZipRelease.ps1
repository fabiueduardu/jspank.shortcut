Param(
     [string]$FolderToZip,
	 [string]$FolderZipName
)

echo $FolderToZip
echo $FolderZipName
Compress-Archive -Path $FolderToZip -DestinationPath $FolderZipName
