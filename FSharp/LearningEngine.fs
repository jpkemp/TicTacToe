namespace Game

open System.Collections.Generic
open Combinatorics

module LearningEngine =
    type ComputerPolicy = Dictionary<int, Dictionary<string, float>>
    type PlayerTypes =
        | ComputerPlayer of ComputerPolicy
        | HumanPlayer

    let getAllPossibleStateValues() =
        let i = [for x in 0..8 -> pown 2 x]
        let mutable sums = [] // I think mutable lists are anti-patterns in F#
        let combos = combinations(i)
        for combo in combos do // there are probably better F# type ways to do this
            let perms = permutations(combo)
            for perm in perms do
                let mutable currentSum = 0
                for n = 1 to List.length combo do
                    if n % 2 = 1 then
                        currentSum <- currentSum + (BoardEngine.getPlayerValue BoardEngine.Player1) * perm.[n] // Should be using fold or scan or something here
                    else
                        currentSum <- currentSum + (BoardEngine.getPlayerValue BoardEngine.Player2) * perm.[n]

                sums <- [currentSum] |> List.append sums

        let sums = set sums

        sums

    let getDigitsFromNumber n =
        // returns digits with more significant digit at higher index
        // only implemented for positive numbers
        let rec getDigit x dgtLst=
            if x <= 0 then dgtLst
            else
                let nextLst = dgtLst @ [x % 10]
                let nextX = x / 10
                getDigit nextX nextLst

        getDigit n []

    let getInitialPolicies() =
        let states = getAllPossibleStateValues()
        let policyPlayer1 = new ComputerPolicy()
        let policyPlayer2 = new ComputerPolicy()
        let addInitial (d: ComputerPolicy) (state: int) =
             d.Add(state, new Dictionary<string, float>())
             d.[state].Add("n", 1.0)
             d.[state].Add("value", 0.0)

        for state in states do
            addInitial policyPlayer1 state
            addInitial policyPlayer2 state

            let stateDigits = getDigitsFromNumber state
            let init = [7; 056; 224; 77; 146; 282; 84; 273] // not sure where these come from, possibly random
            for n in init do
                let x = getDigitsFromNumber n
                let mutable isWinner = true
                for i = 0 to min 3 (List.length stateDigits) do
                    if List.length x >= i then // this really needs F#ing
                        if stateDigits.[i] <> x.[i] then
                            isWinner <- false
                    else
                        if stateDigits.[i] <> 0 then
                            isWinner <- false

                if isWinner then
                    policyPlayer1.[state].["value"] <- 1.0
                    policyPlayer2.[state].["value"] <- -1.0

        (policyPlayer1, policyPlayer2)

    let updatePolicy (policy: ComputerPolicy)
                     (states: list<int>)
                     (expectedValue: float) =
        for i = (List.length states) - 1 downto 0 do
            policy.[states.[i]].["n"] <- policy.[states.[i]].["n"] + 1.0
            policy.[states.[i]].["value"] <- policy.[states.[i]].["value"] + (((expectedValue / float i) - policy.[states.[i]].["value"]) / policy.[states.[i]].["n"])

    let getNextPossibleActionValues (board: BoardEngine.Board)
                                    (player: BoardEngine.Player) =
        let emptySpaces = board.FindEmptySpaces()
        let mutable possibleActions = []
        let playerValue = Game.BoardEngine.getPlayerValue player
        for position in emptySpaces do
            let row = position.Row
            let col = position.Col
            possibleActions <- possibleActions @ [(pown 2 ((row+3*(col-1)) - 1) * playerValue)]

        possibleActions

    let selectBestAction (board: BoardEngine.Board)
                         (policy: ComputerPolicy)
                         (player: BoardEngine.Player)
                         (emptySpaces)
                         (allowExploration: bool) =
        let currentStateValue = board.GetStateValue()
        let nextStateValues = getNextPossibleActionValues board player
        let probabilities = [|for value in nextStateValues -> policy.[currentStateValue + value].["value"]|]
        let maxProbability =
            let mutable m = 0.0
            for p in probabilities do
                m <- max m p

            m

        let actionIndices = [for i = 0 to probabilities.Length - 1 do if probabilities.[i] = maxProbability then i]
        let random = System.Random()
        let mutable idx = 0
        if allowExploration then
            if random.Next(9) = 0 then
                idx <- random.Next((List.length emptySpaces) - 1)
        else idx <- actionIndices |> Combinatorics.shuffleR random |> Seq.head

        emptySpaces.[idx]
