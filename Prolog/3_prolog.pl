% UGNIUS PADOLSKIS
% 1.1 2.3 3.1 4.4

did([]) :- !.
did([_]) :- !.
did([X, Y | T]) :-
    X =< Y,
    !,
    did([Y | T]).

%did([4, 18, 23, 100]). True
%did([10, 5, 15]). False



dubl_trigub([], []) :- !.
dubl_trigub([H | T], R) :-
    H > 0,  % If the element is positive,
    dubl_trigub(T, Rest),
    !,
    R = [H, H | Rest].
dubl_trigub([H | T], R) :-
    H =< 0,
    dubl_trigub(T, Rest),
    !,
    R = [H, H, H | Rest].


%dubl_trigub([-3, 2, 0], R).



manomember(X, [X | _]).
manomember(X, [_ | T]) :-
    manomember(X, T).

ieina([], _) :- !.
ieina([H | T], R) :-
    manomember(H, R),
    !,
    ieina(T, R).

% ieina([1, 4], [3, 2, 4, 1, 5]). True
% ieina([1, 6], [3, 2, 4, 1, 5]). False





% Šešioliktainiai skaitmenys
hex_digit(0, 0). hex_digit(1, 1). hex_digit(2, 2). hex_digit(3, 3).
hex_digit(4, 4). hex_digit(5, 5). hex_digit(6, 6). hex_digit(7, 7).
hex_digit(8, 8). hex_digit(9, 9). hex_digit(a, 10). hex_digit(b, 11).
hex_digit(c, 12). hex_digit(d, 13). hex_digit(e, 14). hex_digit(f, 15).


convert_to_decimal([], _, 0).
convert_to_decimal([H | T], Pow, Value) :-
    hex_digit(H, Dec),
    convert_to_decimal(T, Pow * 16, RestValue),
    Value is Dec * Pow + RestValue.


decimal_to_list(0, [0]).
decimal_to_list(N, L) :-
    N > 0,
    decimal_to_list_helper(N, [], L).

decimal_to_list_helper(0, Acc, Acc).
decimal_to_list_helper(N, Acc, L) :-
    N > 0,
    Digit is N mod 10,
    N1 is N // 10,
    decimal_to_list_helper(N1, [Digit | Acc], L).


manoappend([], L, L).
manoappend([H | T], L, [H | R]) :-
    manoappend(T, L, R).

manoreverse([], []).
manoreverse([H | T], Reversed) :-
    manoreverse(T, RevT),
    manoappend(RevT, [H], Reversed).

is_sesiolik(Ses, Des) :-
    manoreverse(Ses, RevSes),
    convert_to_decimal(RevSes, 1, DecimalValue),
    decimal_to_list(DecimalValue, Des),
    !.


% is_sesiolik([7, c, 1], Des).
% Des = [1, 9, 8, 5].

% is_sesiolik([f, f], Des).
% Des = [2, 5, 5].
