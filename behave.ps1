

$BaseDir = $PSScriptRoot
$FrameworkInitFlag = ".framework-initialized"
$OldPath = $Env:Path
$Env:Path += ";$Env:AppData\Python\Python38\Scripts"

function Command-Available {
    param($Command)
    $OldPref = $ErrorActionPreference
    $ErrorActionPreference = 'stop'
    try {
        (Get-Command $Command)
        return $True
    }
    catch {
        return $False
    }
    finally {
        $ErrorActionPreference = $OldPref
    }
}

# ensure pipenv available
if (!(Command-Available pipenv)) {
    "installing 'pipenv'"
    pip3 install pipenv --user
}

try {
#     # set working dir
    Push-Location $BaseDir
    New-Item test.log

#     # initialize framework if needed
    if (!(Test-Path $FrameworkInitFlag)) {
        "installing framework"
        pipenv --version
        pipenv sync
        # New-Item -Name $FrameworkInitFlag -ItemType file
    }

#     # run samples
#     pipenv run behave $Args 2>&1 | %{ "$_" }

}
finally {
    Pop-Location
    $Env:Path = $OldPath
}

