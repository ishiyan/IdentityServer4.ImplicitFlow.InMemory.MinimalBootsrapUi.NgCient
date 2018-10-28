[CmdletBinding()]

$password = "f1cfe113-66bb-47ff-8488-0933e35fe3c4"

# must run as admin
if( -not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Warning "Failed, must run as Administrator."
    Exit
}

$cwd = Convert-Path .
$pfxFile = "$cwd\token_signing.pfx"
$cerFile = "$cwd\token_signing.cer"

# abort if files do not exist
if(!(Test-Path($pfxFile)) -or !(Test-Path($cerFile)))
{
    Write-Warning "Failed, no token_signing.pfx or token_signing.cer in current directory."
    Exit
}

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cert.Import($cerFile)

$certStore = 'Cert:\CurrentUser\My\' + ($cert.ThumbPrint)  
$securePass = ConvertTo-SecureString -String $password -Force -AsPlainText

# try/catch and -erroraction because password failure is a possibility
Try
{
    Import-PfxCertificate -FilePath $pfxFile Cert:\LocalMachine\My -Password $securePass -Exportable -ErrorAction Stop
}
Catch
{
    Write-Warning "Failed, error importing token_signing.pfx. Is the password correct?"
    Exit
}

# this launches a wizard
Import-Certificate -FilePath $cerFile -CertStoreLocation Cert:\CurrentUser\Root
