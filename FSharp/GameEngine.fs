namespace Game

module GameEngine =
    let computerTurn (board: BoardEngine.Board)
                     (player: BoardEngine.Player)
                     (policy: LearningEngine.ComputerPolicy)
                     (allowExploration: bool) =

        let empties = board.FindEmptySpaces()
        let pos = LearningEngine.selectBestAction board policy player empties allowExploration
        let hasWon = board.Update pos player
        board.Draw()
        hasWon

    let getValidNumber str =
        let rec getN n =
            let recur =
                printfn "Enter %s number:" str
                let input = System.Console.ReadLine()
                getN input

            if n = "" then
                recur
            else
                try
                    let intN = n |> int
                    if intN < 1 || intN > 3 then
                        printfn "Value must be between 1 and 3"
                        recur
                    else
                       intN
                with
                    | :? System.FormatException -> printfn "Input must be a number"; recur

        let value = getN ""
        value

    let humanTurn (board: BoardEngine.Board)
                  (player: BoardEngine.Player) =
        board.Draw()
        let rec check() =
            let row = getValidNumber "row"
            let col = getValidNumber "col"
            let pos = BoardEngine.Position(row, col)
            try
                board.Update pos player
            with
                | BoardEngine.TakenException e -> printfn "%s" e; check()

        let result = check()
        board.Draw()
        result

    type GameResult =
    | Player1Wins
    | Player2Wins
    | Draw

    let printResult result =
        match result with
        | Player1Wins -> printfn "%s" "Player 1 wins!"
        | Player2Wins -> printfn "%s" "Player 2 wins!"
        | Draw -> printfn "%s" "Draw!"

    let playGame (player1: LearningEngine.PlayerTypes)
                 (player2: LearningEngine.PlayerTypes)
                 (trainingMode: bool) =

        let board = BoardEngine.Board()
        let mutable turnStates: int list = []

        let policyUpdate player =
            match player with
                | LearningEngine.HumanPlayer -> ignore
                | LearningEngine.ComputerPlayer policy -> LearningEngine.updatePolicy policy turnStates

        let finalise result =
            let expected =
                match result with
                | Player1Wins -> 1.0
                | Player2Wins -> 2.0
                | Draw -> 0.0

            policyUpdate player1 expected
            (policyUpdate player1) (-1.0 * expected)
            printResult result

        let playerTurn (playerType: LearningEngine.PlayerTypes) (playerN: BoardEngine.Player) (allow: bool) =
            match playerType with
            | LearningEngine.HumanPlayer -> humanTurn board playerN
            | LearningEngine.ComputerPlayer policy -> computerTurn board playerN policy allow

        let rec gameTurn =
            let player1Wins = (playerTurn player1 BoardEngine.Player1 trainingMode)
            turnStates <- List.append turnStates [board.GetStateValue()]
            if player1Wins then
                finalise Player1Wins

            let empty = board.FindEmptySpaces()
            if List.isEmpty empty then
                finalise Draw
            let player2Wins = (playerTurn player2 BoardEngine.Player2 trainingMode)
            turnStates <- List.append turnStates [board.GetStateValue()]
            if player2Wins then
                finalise Player2Wins

        gameTurn

    let getComputerPlayers =
        let player1Policy, player2Policy = LearningEngine.getInitialPolicies()
        let player1 = LearningEngine.ComputerPlayer player1Policy
        let player2 = LearningEngine.ComputerPlayer player2Policy

        player1, player2

    let trainComputer (n: int)
                      (computer1: LearningEngine.PlayerTypes)
                      (computer2: LearningEngine.PlayerTypes) =
        match computer1 with
        | LearningEngine.HumanPlayer -> failwith "Must be a computer player"
        | _ -> ()

        match computer2 with
        | LearningEngine.HumanPlayer -> failwith "Must be a computer player"
        | _ -> ()

        printfn "%s" "Training..."
        for i = 1 to n do
            playGame computer1 computer2 true

        printfn "%s" "Training done!"
        computer1, computer2
