param($configxml)

if(-not (Test-Path ".\7z.exe"))
{
    Clear-Host
    Write-Host "Please execute this script only from the folder, where this script is present" -ForegroundColor Magenta
    return;
}

## .\Deployment.ps1 -configxml <Path-to-xml>";

if($configxml -eq "")
{
    throw "Deployment Config path Cannot be blank"; 
    "Execution Command: .\Deployment.ps1 -configxml <Path-to-xml>";
    return; 
}

function Prepare-Package($app)
{
    if( -not(Test-Path $app.Package))
    {
        throw ("Prepare-Package : " + $app.Name + " package not found at " + $app.Package)    
    }
    Write-Host "------------------------------" -ForegroundColor Cyan
    Write-Host ("Preparing package: " + $app.Name) -ForegroundColor Yellow
    Write-Host ("Preparing Web Config") -ForegroundColor Yellow

    foreach($appConfig in $app.AppConfig)
    {
        $config = Join-Path $app.Package $appConfig.file
        Write-Host ("config path: " + $config) -ForegroundColor Yellow
        write-host ("writing "+ $appConfig.Name + " in " + $appConfig.file) -ForegroundColor Yellow
        Edit-XMLNodeArrayElement -file $config -path $appConfig.Path -type Attribute -value $appConfig.Value -Attr "value" -key "key" -keyValue $appConfig.Name
    }
    Write-Host ("Finished preparing Web Config") -ForegroundColor Yellow

    if(($app.ReplaceFile -ne $null) -and ($app.ReplaceFile.Count -ne $null))
    {
        Write-Host ("preparing files") -ForegroundColor Yellow
        foreach($file in $app.ReplaceFile)
        {
            $path = join-path $app.Package $file.fileName
            write-host ("writing "+ $file.name + " > $path" ) -ForegroundColor Yellow
            #Replace-Token -file $path -token $file.token -value $file.value

            Edit-XMLNodeArrayElement -file $path -path $file.Path -type Attribute -value $file.Value -Attr $file.attr -key $file.key -keyValue $file.keyValue
        }
        Write-Host ("Finished preparing files") -ForegroundColor Yellow
    }
    Write-Host ("Finished preparing package") -ForegroundColor Yellow
}

function Get-DeploymentConfig($xml)
{
    if(-not (Test-Path $xml))
    {
        write-host "Config file doesn't exists" -ForegroundColor Red
        return $null
    }

    [System.Xml.XPath.XPathDocument] $xp = New-Object -TypeName System.Xml.XPath.XPathDocument -ArgumentList @($xml)
    $nav = $xp.CreateNavigator()
    #[System.Xml.XPath.XPathExpression] $exp = $nav.Compile("DeploymentConfig/PackageRoot");
    [System.Xml.XPath.XPathNavigator] $itr = $nav.SelectSingleNode("/DeploymentConfig/PackageRoot");
    $buildPath = $itr.GetAttribute("path","")

    [System.Xml.XPath.XPathNavigator] $itr = $nav.SelectSingleNode("/DeploymentConfig/PublishProfile");
    $pubxml = $itr.GetAttribute("path","")

    [System.Xml.XPath.XPathNavigator] $itr = $nav.SelectSingleNode("/DeploymentConfig/PublishSite");
    $pubsite = $itr.GetAttribute("name","")

    [System.Xml.XPath.XPathNavigator] $itr = $nav.SelectSingleNode("/DeploymentConfig/Jobs/JobCollectionName");
    if($itr -ne $null)
    {
        $JobCollectionName = $itr.GetAttribute("name","")
    }

    $deploymentConfig = @{
        "PublishSite" = $pubsite;
        "BuildPath" = $buildPath;
        "Apps" = @();
        "PublishProfile" = $pubxml;
        "Jobs" = @();
        "JobCollectionName" = $JobCollectionName;
    }

    [System.Xml.XPath.XPathNodeIterator] $itr = $nav.Select("/DeploymentConfig/Apps/App");
    while($itr.MoveNext())
    {
        $appName = $itr.Current.GetAttribute("name","")
        $appDesc = $itr.Current.GetAttribute("description","")
        $VirtualDirectory = $itr.Current.SelectChildren("VirtualDirectory","").Value
        $Package = $itr.Current.SelectChildren("Package","").Value
        
        $subNodes = $itr.Current.CreateNavigator()
        $configs = $subNodes.Select("AppConfig/config")
        $config = @()
        while($configs.MoveNext())
        {
            $name = $configs.Current.GetAttribute("name","")
            $path = $configs.Current.GetAttribute("path","")
            $value = $configs.Current.GetAttribute("value","")
            $file = $configs.Current.GetAttribute("file","")
            $cfg = @{
                        "name" = $name;
                        "path" = $path;
                        "value" = $value;
                        "file" = $file;
                    }
            $config += $cfg
        }

        $files = $subNodes.Select("Replace/file")
        $file = @()
        while($files.MoveNext())
        {
            $name = $files.Current.GetAttribute("name","")
            $path = $files.Current.GetAttribute("path","")
            $fileName = $files.Current.GetAttribute("fileName","")
            $value = $files.Current.GetAttribute("value","")
            $key = $files.Current.GetAttribute("key","")
            $keyValue = $files.Current.GetAttribute("keyValue","")
            $attr = $files.Current.GetAttribute("Attr","")
            $f = @{
                        "name" = $name;
                        "fileName" = $fileName;
                        "value" = $value;
                        "path" = $path;
                        "key" = $key;
                        "keyValue" = $keyValue;
                        "attr" = $attr;
                  }
            $file += $f
        }

        $app = @{
            "Name" = $appName;
            "Description" = $appDesc;
            "VirtualDirectory" = $VirtualDirectory;
            "Package" = Join-Path $buildPath $Package;
            "ReplaceFile" = $file;
            "AppConfig" = $config;
        }
        $deploymentConfig.Apps += $app
    }

    [System.Xml.XPath.XPathNodeIterator] $itr = $nav.Select("/DeploymentConfig/Jobs/Job");
    while($itr.MoveNext())
    {
        $name = $itr.Current.GetAttribute("name","")
        $path = $itr.Current.GetAttribute("path","")
        $frequency = $itr.Current.GetAttribute("frequency","")
        $interval = $itr.Current.GetAttribute("interval","")

        $xnj = $itr.Current.CreateNavigator()
        $files = $xnj.SelectDescendants("file","",$false)
        $job = @{
            "name" = $name;
            "path" = $path;
            "frequency" = $frequency;
            "interval" = $interval;
            "config" =  @();
        }

        foreach($file in $files)
        {
            $obj = @{
                "name" = $file.GetAttribute("name","")
                "filename" = $file.GetAttribute("fileName","")
                "path" = $file.GetAttribute("path","")
                "key" = $file.GetAttribute("key","")
                "keyValue" = $file.GetAttribute("keyValue","")
                "Attr" = $file.GetAttribute("Attr","")
                "value" = $file.GetAttribute("value","")
            }
            $job.config += $obj
        }

        $deploymentConfig.jobs += $job
    }

    return $deploymentConfig
}

function Replace-Token($file = (throw "please provide a file"), $token = (throw "token can't be empty"), $value)
{
    if(-not (Test-Path $file))
    {
        throw "Replace-Token : File not found";
    }
    [string]$content = [System.IO.File]::ReadAllText($file)
    $content = $content.Replace($token,$value)
    [System.IO.File]::WriteAllText($file,$content)
}

function Start-Deployment($xml)
{
    if( -not($dev))
    {
        Init-Deployment
    }

    $depConfig = Get-DeploymentConfig $xml
    
    if($depConfig -ne $null)
    {
        Write-Host ("Packages root path: "+ $depConfig.BuildPath) -ForegroundColor Yellow
    }

    foreach($app in $depConfig.Apps)
    {
        Write-Host ("Preparing app: " + $app.Name) -ForegroundColor Yellow
        Prepare-Package $app
    }

    Write-Host "Starting deployment" -ForegroundColor Yellow
    foreach($app in $depConfig.Apps)
    {
        Write-Host ("Starting deployment for " + $app.Name) -ForegroundColor Yellow
        if($app.VirtualDirectory -eq "")
        {
            Write-Host ("Publishing to site root: " + $app.Name) -ForegroundColor Yellow
            .\WawsDeploy.exe $app.Package $depConfig.PublishProfile /v /d
        }
        else
        {
            Write-Host ("Publishing to virtual directory: " + $app.VirtualDirectory + " - App Name: " + $app.Name) -ForegroundColor Yellow
            .\WawsDeploy.exe $app.Package $depConfig.PublishProfile /d /v /t $app.VirtualDirectory
        }
    }
    Write-Host ("Sites deployment Complete") -ForegroundColor Cyan
    Write-Host ("Starting WebJobs deployment") -ForegroundColor Yellow
    Publish-WebJobs $depConfig
}

function Init-Deployment()
{
    Write-Host ("Cleaning up cached accounts") -ForegroundColor Yellow
    $accounts = Get-AzureAccount | % Id
    foreach($account in $accounts)
    {
        Remove-AzureAccount -Name $account -Force -Confirm:$false
    }
    
    Write-Host ("Getting Azure subscripton") -ForegroundColor Yellow

    Write-Host ("A popup signin box will now popup, to login into your azure account") -ForegroundColor Green
    Start-Sleep -Seconds 2
    Add-AzureAccount
    $subscription = (Get-AzureAccount).Subscriptions.Split([Environment]::NewLine.ToCharArray())[0]
    if($subscription -eq $null)
    {
        Write-Host "unable to get azure subscription" -ForegroundColor Red
        Exit-PSSession
    }

    Select-AzureSubscription -Default -SubscriptionId $subscription

}

function Publish-WebJobs($depConfig)
{
    $location = ""
    Write-Host "------------------------------" -ForegroundColor Cyan
    if($depConfig.jobs.Count -eq 0)
    {
        Write-Host "No jobs to deploy" -ForegroundColor Yellow
        return;
    }

    Write-Host "------------------------------" -ForegroundColor Cyan
    Write-Host ("Preparing Jobs package") -ForegroundColor Yellow

    foreach($job in $depConfig.Jobs)
    {
        Write-Host ("Preparing Jobs package: "+ $job.Name) -ForegroundColor Yellow
        
        foreach($cfg in $job.config)
        {
            $file = Join-Path $job.path $cfg.filename
            write-host "Editing $file" -ForegroundColor Green
            Edit-XMLNodeArrayElement -file $file -path $cfg.Path -type Attribute -key $cfg.key -keyValue $cfg.keyValue -Attr $cfg.Attr -value $cfg.Value
            write-host ("writing "+ $cfg.Attr + " in " + $cfg.Value) -Green
        }

        Write-Host ("Finished preparing job package: " + $job.Name) -ForegroundColor Yellow
        
        $path = Split-Path $job.path -Parent
        $zip = Join-Path $path ($job.Name+".zip")
        Write-Host ("Compressing file for " + $job.Name) -ForegroundColor Yellow
        Write-Host ("Target: " + $zip ) -ForegroundColor Yellow
        .\7z.exe a $zip ($job.path+"\*")
    }

    $startTime = [DateTime]::UtcNow.AddMinutes(10).ToString("s",[System.Globalization.CultureInfo]::InvariantCulture )
    $endTime = [DateTime]::UtcNow.AddYears(10).ToString("s",[System.Globalization.CultureInfo]::InvariantCulture )

    Write-Host ("Determining location") -ForegroundColor Yellow
    $site = Get-AzureWebsite -Name $depConfig.PublishSite;
    $ws = $site.WebSpace.ToLower().Replace("webspace","")
    $locations = Get-AzureLocation | % DisplayName
    foreach($loc in $locations)
    {
        if($loc.Replace(" ","").ToLower() -eq $ws)
        {
            $location = $loc
        }
    }
    if($location -ne "")
    {
        Write-Host ("Publishing location is : " + $location) -ForegroundColor Yellow
    }


    Write-Host ("Checking for existing Job collection and removing them.") -ForegroundColor Yellow
    Get-AzureSchedulerJobCollection | Remove-AzureSchedulerJobCollection -Force
    foreach($j in $depConfig.Jobs)
    {
            Write-Host ("Checking for existing scheduler job") -ForegroundColor Yellow
            $job = Get-AzureSchedulerJob -Location $location -JobCollectionName ($depConfig.JobCollectionName + "-" + $j.Name)
            if($job -ne $null)
            {
                Write-Host ("Removing existing scheduler job"  + $j.name ) -ForegroundColor Yellow
                Remove-AzureSchedulerJob -Force -JobCollectionName $jobCollection[0].JobCollectionName -Location $location
            }
            Get-AzureWebsiteJob -Name $site.Name -JobName $j.name -JobType Triggered | Remove-AzureWebsiteJob
    }

    foreach($j in $depConfig.Jobs)
    {
        Write-Host ("Creating Job: " + $j.Name)-ForegroundColor Yellow
        
        $path = Split-Path $j.path -Parent
        $zip = Join-Path $path ($j.Name+".zip")
        $job = New-AzureWebsiteJob -Name $site.Name -JobName $j.name -JobType Triggered -JobFile $zip
        
        if($job -ne $null)
        {
            Write-Host ("Setting scheduling Job: " + $depConfig.JobCollectionName + "-" + $j.Name) -ForegroundColor Yellow
        
            $jobCollection = New-AzureSchedulerJobCollection -Location $location -JobCollectionName ($depConfig.JobCollectionName + "-" + $j.Name)
    
            $authPair = "$($site.PublishingUsername):$($site.PublishingPassword)";
            $pairBytes = [System.Text.Encoding]::UTF8.GetBytes($authPair);
            $encodedPair = [System.Convert]::ToBase64String($pairBytes);

            New-AzureSchedulerHttpJob `
              -JobCollectionName $jobCollection[0].JobCollectionName `
              -JobName $j.Name `
              -Method POST `
              -URI "$($job.Url)\run" `
              -Location $location `
              -StartTime $startTime `
              -Interval $j.interval `
              -Frequency $j.frequency` `
              -EndTime $endTime `
              -Headers @{ `
                "Content-Type" = "text/plain"; `
                "Authorization" = "Basic $encodedPair"; `
              };

              Write-Host ("Job created: " + $j.Name) -ForegroundColor Green
          }
        else
        {
            Write-Host ("Failed to create job: " + $j.Name) -ForegroundColor Red
        }
      }
}

function Edit-XMLNodeArrayElement($file, [string] $path, [ValidateSet("Attribute","Value")]$type = 'Attribute', [Parameter(Mandatory=$true)]$value, $Attr, $key, $keyValue)
{
    if(-not(Test-Path -Path $file))
    {
        Write-Host "Edit- XML: File doesn't exist: $file" -ForegroundColor Red
        throw "File Not Found"
    }

    $xml = New-Object XML

    $xml.Load($file)
    $nodes=$xml.SelectNodes($path)

    foreach($node in $nodes) 
    {
        if($node.GetAttribute($key) -eq $keyValue)
        {
            $node.SetAttribute($Attr, $value);
        }
    }
    $xml.Save($file)
}

Clear-Host  #clean the screen
Start-Deployment $configxml

<# ## Dev code ##

$dev = $true # Set the development mode to true
$deploy = $false # If false, Do not deploy just test the Package configuration script


if( -not($dev))
{
    Start-Deployment $configxml
}


################# UNIT TESTS ################# 
if($dev -eq $true)
{
    $configxml = "E:\Deploy\Artifacts\PSScripts\DeploymentConfig.xml";
    Start-Deployment $configxml

    $config = Get-DeploymentConfig -xml $configxml
}
#>