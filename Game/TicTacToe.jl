module TicTacToe
    include("./GameEngine.jl")
    include("./LearningEngine.jl")

    # let 0 represent empty space, 1 represent x and 1000 represent o. note that the learning engine relies on these values being constant
    player_1_character = 1
    player_2_character = 1000

    function computer_turn(board::Array, character::Int, policy::Dict, training_mode::Bool)::Bool # returns whether the game is won after the turn
        empty_spaces = GameEngine.empty_spaces(board)
        (row, col) = LearningEngine.select_best_action(board, policy, empty_spaces, character, training_mode)
        has_won = GameEngine.update_position!(board, row, col, character)

        return has_won
    end

    function human_turn(board, character)::Bool # returns whether the game is won after the turn
        GameEngine.draw_board(board, player_1_character, player_2_character)

        has_won = false

        row = 0
        col = 0
        while true
            while true
                println("Enter row number:")
                x = try
                    row = parse(Int, readline())
                    if row >3 || row < 1
                        throw(ArgumentError("position values must be between 1 and 3"))
                    end
                    break
                catch x
                    if typeof(x) != ArgumentError throw(x) end
                    println(x)
                    continue
                end
            end

            while true
                x = try
                    println("Enter column number:")
                    col = parse(Int, readline())
                    if col >3 || col < 1
                        throw(ArgumentError("position values must be between 1 and 3"))
                    end
                    break
                catch x
                    if typeof(x) != ArgumentError throw(x) end
                    println(x)
                    continue
                end
            end

            try
                has_won = GameEngine.update_position!(board, row, col, character)
                break
            catch BoundsError
                println("There's already a piece there!")
            end
        end

        return has_won
    end

    function play_game(policy_player_1 = nothing, policy_player_2 = nothing, training_mode = false)
        board_state = zeros(3, 3)
        track_state_values = zeros(0)

        player_1_turn() = if policy_player_1 == nothing
            human_turn(board_state, player_1_character)
        else
            computer_turn(board_state, player_1_character, policy_player_1, training_mode)
        end

        player_2_turn() = if policy_player_2 == nothing
            human_turn(board_state, player_2_character)
        else
            computer_turn(board_state, player_2_character, policy_player_2, training_mode)
        end

        player_1_wins = false
        draw = false
        while true
            player_1_wins = player_1_turn()
            append!(track_state_values, LearningEngine.get_state_value(board_state))
            if player_1_wins
                if player_1_turn != human_turn
                    GameEngine.draw_board(board_state, player_1_character, player_2_character)
                end

                break
            end

            if length(GameEngine.empty_spaces(board_state)) == 0
                draw = true
                if player_1_turn != human_turn
                    GameEngine.draw_board(board_state, player_1_character, player_2_character)
                end

                break
            end

            player_2_wins = player_2_turn()
            append!(track_state_values, LearningEngine.get_state_value(board_state))
            if player_2_wins
                if player_2_turn != human_turn
                    GameEngine.draw_board(board_state, player_1_character, player_2_character)
                end

                break
            end
        end

        if draw
            println("Draw!")
            player_1_expected_value = 0
        else
            if player_1_wins
                println("X wins!")
                player_1_expected_value = 1
            else
                println("O wins")
                player_1_expected_value = -1
            end
        end

        if policy_player_1 != nothing
            LearningEngine.update_policy!(policy_player_1, track_state_values, player_1_expected_value)
        end
        if policy_player_2 != nothing
            LearningEngine.update_policy!(policy_player_2, track_state_values, -1 * player_1_expected_value)
        end
    end

    function train_computer(n, policy_1=nothing, policy_2=nothing)
        println("Training...")
        (player_1_policy, player_2_policy) = LearningEngine.get_initial_policy()
        if policy_1 !== nothing
            player_1_policy = policy_1
        end
        if policy_2 !== nothing
            player_2_policy = policy_2
        end

        for i = 1:n
            play_game(player_1_policy, player_2_policy, true)
        end

        println("Training done!")
        return (player_1_policy, player_2_policy)
    end
end

