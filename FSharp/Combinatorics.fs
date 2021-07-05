namespace Game

module Combinatorics =
    let combinations lst=
    // from http://www.fssnip.net/2z/title/All-combinations-of-list-elements
        let rec comb accLst elemLst =
            match elemLst with
            | h::t ->
                let next = [h]::List.map (fun el -> h::el) accLst @ accLst
                comb next t
            | _ -> accLst

        comb [] lst

    let permutations xs =
    // from https://rosettacode.org/wiki/Permutations#F.23
        let rec insert x = function
            | [] -> [[x]]
            | head :: tail -> (x :: (head :: tail)) :: (List.map (fun l -> head :: l) (insert x tail))

        List.fold (fun s e -> List.collect (insert e) s) [[]] xs

    let shuffleR (r : System.Random) xs = xs |> Seq.sortBy (fun _ -> r.Next())
    // from https://stackoverflow.com/questions/33312260/how-can-i-select-a-random-value-from-a-list-using-f