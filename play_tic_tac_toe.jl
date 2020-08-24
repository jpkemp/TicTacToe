include("Game/TicTacToe.jl")
println("Enter number of training iterations:")
train = parse(Int, readline())
(player_1_policy, player_2_policy) = TicTacToe.train_computer(train)
while true
    finish = false
    while true
        println("Play as player 1 (X) or 2 (O)?")
        x = try
            player = parse(Int, readline())
            if player == 1
                TicTacToe.play_game(nothing, player_2_policy, false)
            elseif player == 2
                TicTacToe.play_game(player_1_policy, nothing, false)
            else
                throw(ArgumentError("Please enter 1 or 2"))
            end
            break
        catch x
            if typeof(x) != ArgumentError throw(x) end
            println(x)
            continue
        end
    end
    while true
        println("Play again (p), train model (t), or quit (q)?")
        x = try
            action = readline()
            println(action)
            if action == "p"
                break
            elseif action == "t"
                println("Enter number of training iterations:")
                n = parse(Int, readline())
                global player_1_policy
                global  player_2_policy
                (player_1_policy, player_2_policy) = TicTacToe.train_computer(n,
                    player_1_policy, player_2_policy)
            elseif action == "q"
                finish = true
                break
            else
                throw(ArgumentError("Please enter p, t or q"))
            end
        catch x
            if typeof(x) != ArgumentError throw(x) end
            println(x)
            continue
        end
    end

    if finish break end
end
println("Goodbye!")