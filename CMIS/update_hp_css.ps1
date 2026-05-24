$path = 'c:\Users\user\OneDrive\Documents\CODES\CMIS\CMIS\Components\Pages\Head_Pastor\Collection-Donation\CollectionOversight.razor.css'
$content = Get-Content $path -Raw
$newContent = $content -replace '(?m)^([\.#][a-zA-Z0-9_-]+)', '::deep $1'
$newContent | Set-Content $path -Encoding UTF8
