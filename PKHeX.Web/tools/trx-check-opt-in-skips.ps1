#!/usr/bin/env pwsh
<#
.SYNOPSIS
Checks a VSTest .trx file from a run that was not opted in to any Web test tier, as upstream's Azure VsTest step runs.

.DESCRIPTION
Fails unless:
- the run completed (a crashed or aborted test host does not) and no test failed, errored, timed out or was aborted;
- every result belongs to a test definition in the file;
- every assembly named in -Assemblies passed at least one test;
- every test of -OptInAssembly that was not executed was skipped by the opt-in (TierFact/TierTheory) and nothing else;
- -OptInAssembly skipped at least one test, so the opt-in tiers were really left off and the check was not vacuous.
Skips in other assemblies (Core has one) are not checked.

.EXAMPLE
pwsh PKHeX.Web/tools/trx-check-opt-in-skips.ps1 results.trx -Assemblies PKHeX.Core.Tests.dll, PKHeX.Web.Tests.dll
#>
param(
    [Parameter(Mandatory)] [string] $Path,
    [Parameter(Mandatory)] [string[]] $Assemblies,
    [string] $OptInAssembly = 'PKHeX.Web.Tests.dll'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# pwsh -File passes "a,b" as one string rather than an array.
$Assemblies = @($Assemblies | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })

if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
    Write-Error "No test results at $Path; did the test run start?"
}

[xml] $trx = Get-Content -LiteralPath $Path -Raw
$ns = @{ t = 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010' }

# TRX keeps the assembly on the test definition, and the outcome on the result that refers to it.
$assemblyOf = @{}
foreach ($node in Select-Xml -Xml $trx -XPath '//t:TestDefinitions/t:UnitTest' -Namespace $ns) {
    $assemblyOf[$node.Node.id] = [IO.Path]::GetFileName(($node.Node.storage -replace '\\', '/'))
}

# The skip reason set by TestEnvironment.SkipUnlessOptedIn.
$optInReason = '^Opt-in tier; set PKHEX_WEB_TEST_TIERS=\w+ to run it\.$'

$problems = [Collections.Generic.List[string]]::new()
$summary = Select-Xml -Xml $trx -XPath '//t:ResultSummary' -Namespace $ns | Select-Object -First 1
if ($null -eq $summary -or $summary.Node.outcome -ne 'Completed') {
    $problems.Add("The run outcome is '$(if ($summary) { $summary.Node.outcome })', not Completed: a test failed, or the test host crashed or aborted.")
}
$passed = @{}
$optInSkips = 0
foreach ($node in Select-Xml -Xml $trx -XPath '//t:Results/t:UnitTestResult' -Namespace $ns) {
    $result = $node.Node
    $assembly = $assemblyOf[$result.testId]
    if ($null -eq $assembly) {
        $problems.Add("$($result.testName) has no test definition, so its assembly is unknown.")
        continue
    }
    $isOptInAssembly = $assembly -ieq $OptInAssembly
    switch ($result.outcome) {
        'Passed' {
            $passed[$assembly.ToLowerInvariant()] = $true
        }
        'NotExecuted' {
            if (-not $isOptInAssembly) {
                continue
            }
            $message = Select-Xml -Xml $result -XPath 't:Output/t:ErrorInfo/t:Message' -Namespace $ns | Select-Object -First 1
            if ($null -ne $message -and $message.Node.InnerText.Trim() -match $optInReason) {
                $optInSkips++
            }
            else {
                $problems.Add("$($result.testName) was not executed, but not because of the tier opt-in.")
            }
        }
        default {
            $problems.Add("$($result.testName) in $assembly has outcome $($result.outcome).")
        }
    }
}

foreach ($assembly in $Assemblies) {
    if (-not $passed.ContainsKey($assembly.ToLowerInvariant())) {
        $problems.Add("No test in $assembly passed; was it found by the test pattern?")
    }
}
if ($optInSkips -eq 0) {
    $problems.Add("No test in $OptInAssembly was skipped by the opt-in; was PKHEX_WEB_TEST_TIERS set?")
}

if ($problems.Count -gt 0) {
    $problems | ForEach-Object { Write-Host "::error::$_" }
    exit 1
}
Write-Host "No failures; $optInSkips opt-in tests in $OptInAssembly were skipped and nothing else in it was."
