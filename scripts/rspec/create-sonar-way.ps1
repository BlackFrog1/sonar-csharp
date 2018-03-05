$lang = "cs"
$sonaranalyzerPath = "${PSScriptRoot}\\..\\..\\sonaranalyzer-dotnet"

$ruleapiLanguageMap = @{
    "cs" = "c#";
    "vbnet" = "vb.net";
}

# Returns the path to the folder where the RSPEC html and json files for the specified language will be downloaded.
function GetRspecDownloadPath() {
    $rspecFolder = "${sonaranalyzerPath}\\rspec\\${lang}"
    if (-Not (Test-Path $rspecFolder)) {
        New-Item $rspecFolder | Out-Null
    }
    return $rspecFolder
}

function CreateStringResources($rules) {
    $rspecFolder = GetRspecDownloadPath $lang

    $sonarWayRules = Get-Content -Raw "${rspecFolder}\\Sonar_way_profile.json" | ConvertFrom-Json

"<?xml version=""1.0"" encoding=""utf-8""?>
<RuleSet Name=""SonarWay"" Description=""Default enabled rules for SonarC#."" ToolsVersion=""14.0"">
  <Rules AnalyzerId=""SonarAnalyzer.CSharp"" RuleNamespace=""SonarAnalyzer.CSharp"">"
  
  foreach ($rule in $rules) {
      if ($sonarWayRules.ruleKeys -Contains $rule)
      {
        "<Rule Id=""${rule}"" Action=""Warning"" />"
      }
      else {
        "<Rule Id=""${rule}"" Action=""None"" />"
      }

    }
    
  "</Rules>
  </RuleSet>"

}

# Returns a string array with rule keys for the specified language.
function GetRules() {
    $suffix = $ruleapiLanguageMap.Get_Item($lang)
    $htmlFiles = Get-ChildItem "$(GetRspecDownloadPath $lang)\\*" -Include "*.html"
    foreach ($htmlFile in $htmlFiles) {
        if ($htmlFile -Match "(S\d+)_(${suffix}).html") {
            $Matches[1]
        }
    }
}

$rr = GetRules
CreateStringResources($rr)