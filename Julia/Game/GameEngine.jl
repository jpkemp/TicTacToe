module GameEngine
    function draw_board(board, player_1_character, player_2_character)  
        for row = 1:3
            for col = 1:3
                print('|')
                x = board[row, col]
                if x == player_2_character
                    print('o')
                elseif x == player_1_character
                    print('x')
                else
                    print(' ')
                end
            end

            print("|\n")
        end
    end

    function update_position!(board, row_position, col_position, character)::Bool
        if board[row_position, col_position] != 0
            error(BoundsError("There's already a piece there!"))
        end

        board[row_position, col_position] = character

        # check diagonals
        if board[2, 2] == character
            if (board[1, 1] == character && board[3, 3] == character) || (board[3,1] == character && board[1,3] == character)
                return true
            end
        end

        # check verticals and horizonals
        ver = 0
        hor = 0
        for i = 1:3
            if board[i, col_position] == character
                ver += 1
            end

            if board[row_position, i] == character
                hor += 1
            end
        end

        if ver == 3 || hor == 3
            return true
        end
        
        return false
    end

    function empty_spaces(board)
        positions = Tuple[]
        for i = 1:3
            for j = 1:3
                if board[i, j] == 0
                    push!(positions, (i, j))
                end
            end
        end

        return positions
    end
end