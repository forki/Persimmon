﻿module TestRunner

open System
open System.IO
open System.Reflection

open Microsoft.FSharp.Reflection

open Persimmon
open Persimmon.ActivePatterns
open Persimmon.Output
open RuntimeUtil

type Result = {
  Errors: int
  ExecutedRootTestResults: ITestResult seq
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =
  let empty = { Errors = 0; ExecutedRootTestResults = Seq.empty }

let runTests (reporter: Reporter) (test: TestObject) =
  match test with
  | Context ctx -> ctx.Run(reporter.ReportProgress) :> ITestResult
  | TestCase tc ->
      let result = tc.Run()
      reporter.ReportProgress(result)
      result :> ITestResult

let rec countErrors = function
| ContextResult cr ->
    cr.Children |> List.sumBy countErrors
| TestResult tr ->
    match tr with
    | Break _ -> 1
    | Done (_, res) ->
        let typicalRes = AssertionResult.NonEmptyList.typicalResult res
        match typicalRes with
        | NotPassed (Violated _) -> 1
        | NotPassed (Skipped _) -> 0
        | Passed _ -> 0
| EndMarker -> 0

let runAllTests reporter (tests: TestObject list) =
  let rootResults = tests |> List.map (runTests reporter)
  let errors = rootResults |> List.sumBy countErrors
  { Errors = errors; ExecutedRootTestResults = rootResults }
