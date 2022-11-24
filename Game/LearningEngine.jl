module LearningEngine
    import Combinatorics

    function get_initial_policy()::Tuple{Dict{Int, Dict{String, Float16}}, Dict{Int, Dict{String, Float16}}}
        states = get_all_possible_state_values()
        policy = Dict()
        for state in states
            merge!(policy, Dict(state => Dict("n" => 1, "value" => 0)))
            state_digits = digits(state)
            for n in [7 056 224 77 146 282 84 273]
                x = digits(n)
                is_winner = true
                for i = 1:minimum([3 length(state_digits)])
                    if length(x) >= i
                        if state_digits[i] != x[i]
                            is_winner = false
                            break
                        end
                    else
                        if state_digits[i] != 0
                            is_winner = false
                            break
                        end
                    end
                end

                if is_winner policy[state]["value"] = 1 end
            end
        end

        policy_2 = copy(policy)
        for state in keys(policy_2)
            policy_2[state]["value"] *= -1
        end

        return (policy, policy_2)
    end

    function update_policy!(policy::Dict, states, expected_value)
        for i in reverse(1:length(states))
            policy[states[i]]["n"] += 1
            policy[states[i]]["value"] = policy[states[i]]["value"] + (((expected_value / i) - policy[states[i]]["value"]) / policy[states[i]]["n"])
        end
    end

    function select_best_action(board::Array, policy::Dict{Int, Dict{String, Float16}}, empty_spaces::Array, player_character::Int, training_mode::Bool)
        current_state_value = get_state_value(board)
        next_state_values = get_next_possible_action_values(empty_spaces, player_character)
        probabilities = zeros(0)
        occurences = zeros(0)
        for val in next_state_values
            next_state = current_state_value + val
            x = policy[next_state]["value"]
            y = policy[next_state]["n"]
            append!(probabilities, x)
            append!(occurences, y)
        end

        max_probability = maximum(probabilities)
        indices = [i for (i, x) in enumerate(probabilities) if x == max_probability]
        idx = indices[rand(1:length(indices))]

        if training_mode
            if (rand(1:10)) == 1
                idx = rand(1:length(empty_spaces))
            end
        end

        return empty_spaces[idx]
    end

    function get_state_value(board)::Int
        value = 0
        for row = 1:3
            for col = 1:3
                value += 2^((row+3*(col-1)) - 1) * board[row, col]
            end
        end

        return value
    end

    function get_next_possible_action_values(empty_spaces, player_character)
        possible_actions = Int[]
        for (row, col) in empty_spaces
            append!(possible_actions, 2^((row+3*(col-1)) - 1) * player_character)
        end

        return possible_actions
    end

    function get_all_possible_state_values()    
        i = [1 2 4 8 16 32 64 128 256]
        sums = Int[]
        combos = Combinatorics.combinations(i)
        for combo in combos
            perms = Combinatorics.permutations(combo)
            for perm in perms
                current_sum = 0 
                for n = 1:length(combo)
                    if n%2 == 1
                        current_sum += perm[n]
                    else
                        current_sum += 1000*perm[n]
                    end
                end
                
                append!(sums, current_sum)
            end
        end

        unique!(sums)

        return sums
    end
end