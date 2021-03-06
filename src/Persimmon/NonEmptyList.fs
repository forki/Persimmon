﻿namespace global

type NonEmptyList<'T> = 'T * 'T list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module NonEmptyList =
  let head (xs: NonEmptyList<_>) = let head, _ = xs in head
  let make head tail : NonEmptyList<_> = (head, tail)
  let ofSeq xs = make (xs |> Seq.head) (xs |> Seq.skip(1) |> Seq.toList)
  let cons head tail : NonEmptyList<_> = let second, tail = tail in (head, second::tail)
  let singleton head : NonEmptyList<_> = (head, [])
  let append (xs: NonEmptyList<_>) (ys: NonEmptyList<_>) : NonEmptyList<_> =
    let x, xs = xs
    let y, ys = ys
    (x, (xs@(y::ys)))
  let appendSeq (xs: NonEmptyList<'T>) (ys: 'T seq) =
    let x, xs = xs
    (x, xs |> Seq.append ys |> Seq.toList)
  let reduce f (list: NonEmptyList<'T>) =
    let head, tail = list
    List.fold f head tail
  let map f (list: NonEmptyList<'T>) =
    let head, tail = list
    (f head, List.map f tail)
  let iter action (list: NonEmptyList<'T>) =
    let head, tail = list
    action head
    List.iter action tail
  let toSeq (list: NonEmptyList<'T>) = seq {
    let head, tail = list
    yield head
    yield! tail
  }
  let toList (list: NonEmptyList<'T>) =
    let head, tail = list
    [ yield head; yield! tail ]
  let forall pred (list:NonEmptyList<'T>) =
    let head, tail = list
    pred head && List.forall pred tail
