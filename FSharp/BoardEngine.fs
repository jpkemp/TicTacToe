namespace Game

module BoardEngine =
    type Position(row, col) =
        member this.Row: int = row
        member this.Col: int = col

    type Player =
        | NoPlayer
        | Player1
        | Player2

    let getPlayerValue player =
        match player with
            | NoPlayer -> 0
            | Player1 -> 1
            | Player2 -> 1000

    let getPlayerCharacter player =
        match player with
            | NoPlayer -> printf " "
            | Player1 -> printf "o"
            | Player2 -> printf "x"

    exception TakenException of string

    type Board() = // this is very OO
        let board=Array2D.create 3 3 NoPlayer
        let iterate everyAction lineAction=
            for row = 0 to 2 do
                for col = 0 to 2 do
                    everyAction row col

                lineAction()

        member this.Draw() =
                let action row col=
                    printf "|"
                    let player = board.[row, col]
                    getPlayerCharacter player

                let lineAction() = printfn "|"
                iterate action lineAction

        member this.FindEmptySpaces() =
            let mutable ret = []
            let action row col=
                if board.[row, col] = NoPlayer then
                    let pos = Position(row, col)
                    ret <- [pos] |> List.append ret

            let doNothing() = ()
            iterate action doNothing

            ret

        member this.GetStateValue() =
            let mutable value = 0
            for row = 1 to 3 do
                for col = 1 to 3 do
                    value <- value +  pown 2 ((row+3*(col-1)) - 1) * (getPlayerValue board.[row, col])

            value

        member this.Check (pos: Position) =
            board.[pos.Row, pos.Col]

        member this.Update(pos: Position)
                          (player: Player) =
            let cur = this.Check pos
            if cur <> NoPlayer then
                raise (TakenException "There's already a piece there!")

            board.[pos.Row, pos.Col] <- player
            let mutable result = false
            if board.[2, 2] = player then
                if (board.[1, 1] = player && board.[3, 3] = player) || (board.[3,1] = player && board.[1,3] = player) then
                    result <- true
            else
                let mutable ver = 0
                let mutable hor = 0
                for i = 0 to 2 do
                    if board.[i, pos.Col] = player then
                        ver <- ver + 1
                    if board.[pos.Row, i] = player then
                        hor <- hor + 1

                if ver = 3 || hor = 3 then result <- true

            result