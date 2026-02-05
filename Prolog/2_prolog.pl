%Ugnius Padolskis
%2.3
%5.3

studentas(jonas, 4).
studentas(tadas, 4).
studentas(mantas, 3).
studentas(matas, 3).
studentas(edvinas, 2).
studentas(lukas, 2).
studentas(ugnius, 1).
studentas(ugne, 1).

yraVyresnis(jonas, tadas).
yraVyresnis(tadas, mantas).
yraVyresnis(mantas, matas).
yraVyresnis(matas, edvinas).
yraVyresnis(edvinas, lukas).
yraVyresnis(lukas, ugnius).
yraVyresnis(ugnius, ugne).

vyresnis(X, Y) :- yraVyresnis(X, Y).
vyresnis(X, Y) :- yraVyresnis(X, Z), vyresnis(Z, Y).

vyresnisKitoKurso(A, B) :-
    studentas(A, KursasA),
    studentas(B, KursasB),
    KursasA \= KursasB,
    vyresnis(A, B).


% grazina visus studentus ir ju kursa
%?- studentas(n, k).

% patikrina ar yra toks studentas kurse
%?- studentas(jonas, 4).
%?- studentas(tadas, 3).

% patikrina kuris vyresnis
%?- yraVyresnis(jonas, tadas).

% isveda visus vyresnius rysius
%?- yraVyresnis(X, Y).

% tikrina transytyvuma
%?- vyresnis(jonas, mantas).

% tikrina transytyvuma
%?- vyresnis(jonas, ugne).

% turi false
%?- vyresnis(tadas, jonas).

% tikrina ar vyresnis uz kito kurso
%?- vyresnisKitoKurso(jonas, mantas).

% tam paciam kurse false
%?- vyresnisKitoKurso(jonas, tadas).

% isveda visus rysius vyresnio, kito kurso
%?- vyresnisKitoKurso(X, Y).



mod_positive(X, Y, X) :-
    X < Y.

mod_positive(X, Y, Mod) :-
    X >= Y,
    X1 is X - Y,
    mod_positive(X1, Y, Mod).


mod(X, Y, Mod) :-
    Y > 0,
    (X >= 0 -> mod_positive(X, Y, Mod);
               mod_positive(-X, Y, M), Mod is Y - M).

mod(X, Y, Mod) :-
    Y < 0,
    (X >= 0 -> mod_positive(X, -Y, M), Mod is -M - Y;
               mod_positive(-X, -Y, M), Mod is M).





%?- mod(3, 5, Mod)
%?- mod(8, 3, Mod).
%?- mod(10, 5, Mod).
%?- mod(-3, 5, Mod).
%?- mod(-3, -5, Mod).
%?- mod(0, 5, Mod).