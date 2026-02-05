
% Ugnius Padolskis Informatika 3k 1g
% Variantas 9, 14, 29, 31
% 9 trys_broliai(Brolis1, Brolis2, Brolis3)
% 14 senelis(Senelis, AnukasAnuke)
% 29 bendraamziai(Asmuo1, Asmuo2)
% 31 vpjz(Vyras) - Asmuo Vyras yra „vyras pačiame jėgų žydėjime“

% Duomenų bazė
asmuo(jonas, vyras, 30, sportuoti).
asmuo(petras, vyras, 31, sportuoti).
asmuo(antanas, vyras, 32, sportuoti).
asmuo(jurgis, vyras, 33, sportuoti).
asmuo(algis, vyras, 70, sportuoti).
asmuo(tadas, vyras, 70, sportuoti).
asmuo(ona, moteris, 68, sportuoti).
asmuo(inga, moteris, 65, sportuoti).
asmuo(rita, moteris, 10, sportuoti).
asmuo(ema, moteris, 32, sportuoti).

mama(ona, jonas).
mama(ona, petras).
mama(ona, antanas).
mama(ona, ema).
mama(inga, jurgis).
mama(ema, rita).

pora(algis, ona).
pora(tadas, inga).
pora(ema, jurgis).

trys_broliai(Brolis1, Brolis2, Brolis3) :-
    asmuo(Brolis1, vyras, _, _),
    asmuo(Brolis2, vyras, _, _),
    asmuo(Brolis3, vyras, _, _),
    mama(Mama, Brolis1),
    mama(Mama, Brolis2),
    mama(Mama, Brolis3),
    Brolis1 \= Brolis2,
    Brolis2 \= Brolis3,
    Brolis1 \= Brolis3.

senelis(Senelis, AnukasAnuke) :-
    (
% Patikriname tėčio pusę
        mama(Mama, AnukasAnuke),
        pora(Tetis, Mama),       
        mama(SenelioZmona, Tetis),
        pora(Senelis, SenelioZmona)
    ;
        % Patikriname mamos pusę
        mama(Mama, AnukasAnuke),
        mama(SenelioZmona, Mama),
        pora(Senelis, SenelioZmona)
    ).
bendraamziai(Asmuo1, Asmuo2) :-
    asmuo(Asmuo1, _, Amzius, _),
    asmuo(Asmuo2, _, Amzius, _),
    Asmuo1 \= Asmuo2.

vpjz(Vyras) :-
    asmuo(Vyras, vyras, Amzius, _),
    Amzius >= 30,
    Amzius =< 50.


% trys_broliai(jonas, petras, algis).
% trys_broliai(ema, petras, algis).
% senelis(algis, rita).
% senelis(ona, rita).
% bendraamziai(algis, tadas).
% bendraamziai(algis, rita).
% vpjz(jurgis).
% vpjz(tadas).
