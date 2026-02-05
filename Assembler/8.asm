.model small
.stack 100h

JUMPS

.data
    apie db "Programa spausdina zodziu 'hexdump'a ' i pasirinkta faila. Rasyti reikia- [output failas] [[pirmas input failas] ... [paskutinis input failas]]", 10, 13, "rasykite '/?' del HELP", 10, 13, '$'

    too_long db "File name is too long. DOS supports file names where the file name is maximum 8 symbols, followed by a dot, and extension of maximum 3 symbols (overall - 12 symbols).", 10, 13, "First 12 symbols of incorrect file name: ", '$'

    dest_error db "Nepavyksta atidaryti destination failo. $"
    sour_error db "Nepavyksta atidaryti source failo. $"

    nl db 10, 13, '$'                 ; Naujos eilutės simboliai

    destF db 12 dup (0), 0, '$'        ; Destinaton failo pavadinimas
    destFhandle dw 0                   ; Destinaton failo perkelimui

    sourceF db 12 dup (0), 0, '$'      ; source failo pavadinimas
    sourceFhandle dw 0                 ; source failo perkelimui

    ivestFail db 0                     ; Įvestų failų skaičius
    baitNusk db 0                      ; Baitų skaičius

    arPos dw 0                         ; Komandos eilutės argumento pozicija
    simb dw 0                          ; Simbolių kiekis

    kiekis db "00000000", 10, 13       ; Formatuotas simbolių kiekis

    sourceBuffer db 16 dup (0)         ; Buferis sourcef duomenims
    destBuffer db "00000000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 |                |", 10, 13    ; Buferis

.code

start:

    mov ax, @data                   ; Inicializuojamas duomenų segmentas
    mov ds, ax

ivesti_failai:
    mov si, 81h                     ; Nustatomas source failo pavadinimo adresas
    mov al, es:[80h]                ; Nuskaitomas source failo pavadinimo ilgis
    mov ivestFail, al               ; Įrašomas source failo pavadinimo ilgis

nera_ivest_failu:
    cmp [ivestFail], 0              ; Tikrinama, ar yra įvestų failų
    jne help                        ; Jei nėra, eiti į pagalbos dalį
    mov dx, offset apie             ; Įrašomas pranešimas apie programą
    mov ah, 9                       ; 9 - "spausdinti žodžiu"
    int 21h                         ; 21h dos interuptas
    jmp exit                        ; Baigti programą

destFailas:
    mov di, offset destF            ; Nustatomas dest failo pavadinimo adresas
    call failoPav                   ; Iškviečiama procedūra failo pavadinimui gauti

sourceFailas:
    call skip_spaces                ; Iškviečiama procedūra praleisti tarpus
    mov di, offset sourceF          ; Nustatomas source failo pavadinimo adresas
    call failoPav                   ; Iškviečiama procedūra failo pavadinimui gauti

destFsukurti:
    mov ah, 3ch                     ; 3ch - sukurti failą
    mov cx, 0                       ; Nenustatyti atributų
    mov dx, offset destF            ; Nustatomas dest failo pavadinimas
    int 21h

    mov destFhandle, ax             ; Išsaugoma dest failo handle
    jnc sourcePaz                   ; Jei nėra klaidų, eiti į šaltinio failo patikrinimą

    mov ah, 9                       ; 9 - "spausdinti žodžius"
    mov dx, offset dest_error       ; Nustatomas klaidos pranešimas dėl paskirties failo
    int 21h
    mov dx, offset destF            ; Nustatomas dest failo pavadinimas
    int 21h

    mov ah, 4ch                     ; 4ch - baigti programą su kodu
    mov al, byte ptr [destFhandle]  ; Nustatomas klaidos kodas
    int 21h

sourcePaz:
    cmp [sourceF], 0                ; Tikrinama, ar yra source failas
    je prog                         ; Jei nėra, eiti į pagrindinę dalį

sourceAtidaryti:
    mov ah, 3dh                     ; 3dh - atidaryti failą
    mov al, 0                       ; Nustatyti režimą
    mov dx, offset sourceF          ; Nustatomas source failo pavadinimas
    int 21h
    mov sourceFhandle, ax           ; Išsaugoma source failo handle

    jnc prog                        ; Jei nėra klaidų, eiti į pagrindinę dalį

    mov ah, 9                       ; 9 - "spausdinti žodžius"
    mov dx, offset sour_error       ; Nustatomas klaidos pranešimas dėl source failo
    int 21h
    mov dx, offset sourceF          ; Nustatomas source failo pavadinimas
    int 21h

    mov ah, 4ch                     ; 4ch - baigti programą su kodu
    mov al, byte ptr [sourceFhandle]; Nustatomas klaidos kodas
    int 21h

prog:
    mov dx, offset nl               ; Nustatomas naujos eilutės simboliai
    mov cx, 1                       ; Nustatomas simbolių skaičius
    mov bx, [destFhandle]           ; Nustatoma dest failo handle
    mov ah, 40h                     ; 40h - rašyti į failą
    int 21h

    call hex_dump                   ; Iškviečiama konvertacija
    call kiekSimb                 ; Iškviečiama skaiciavimai kiek simboliu

    mov bx, [sourceFhandle]         ; Nustatoma source failo handle
    mov ah, 3eh                     ; 3eh - uždaryti failą
    int 21h

                                    ; Nuskaitomas kitas failas iš komandos eilutės argumentų
    mov [simb], 0                   ; Išvalomas simbolių skaičius
    mov di, offset sourceF          ; Nustatomas source failo pavadinimas
    mov al, 0                       ; Išvalomas registras al
    mov cx, 12                      ; Nustatomas maksimalus failo pavadinimo ilgis

clear:

    mov [di], al                    ; Išvalomas source failo pavadinimas
    inc di                          ; Perkeliama prie kito simbolio
    loop clear                      ; Pasikartojama, kol visi simboliai išvalyti

    mov si, [arPos]                 ; Nustatoma komandos eilutės argumento pozicija

    call skip_spaces                ; Iškviečiama procedūra praleisti tarpus
    mov di, offset sourceF          ; Nustatomas šaltinio failo pavadinimas
    call failoPav                   ; Iškviečiama procedūra failo pavadinimui gauti

    cmp [sourceF], 0                ; Tikrinama, ar yra source failas
    jne sourceAtidaryti             ; Jei yra, eiti atidaryti failą

exit:

    mov bx, [destFhandle]           ; Nustatoma destination failo handle
    mov ah, 3eh                     ; 3eh - uždaryti failą
    int 21h

    mov ax, 4c00h                   ; 4c00h - baigti programą
    int 21h

help:
    call skip_spaces                ; Iškviečiama procedūra skip spaces
    cmp word ptr es:[si], 3F2Fh     ; Tikrinama, ar yra '/?' (help)
    jne destFailas                  ; Jei nėra, eiti į dest failą
    mov dx, offset apie             ; Įrašomas pagalbos pranešimas
    mov ah, 9                       ; 9 - "spausdinti žodžiu"
    int 21h
    jmp exit

skip_spaces proc near

skip:

    cmp byte ptr es:[si], 13        ; Tikrinama, ar tai naujos eilutės simbolis
    jne space                       ; Jei ne, eiti prie kitos procedūros dalies
    jmp return                      ; Jei taip, grįžti iš procedūros

space:

    cmp byte ptr es:[si], ' '       ; Tikrinama, ar tai tarpas
    jne return                      ; Jei ne, grįžti iš procedūros
    inc si                          ; Jei taip, padidinti komandos eilutės poziciją
    jmp skip                        ; Ir pradėti dar kartą

return:
    ret                             ; Grįžti iš procedūros

skip_spaces endp                    ; Pabaigti procedūrą

failoPav proc near                  ; Procedūra, skirta gauti failo pavadinimą

    push di                         ; Įrašyti di registrą į steką
    xor ch, ch                      ; Išvalyti registrą ch
    mov cl, 12                      ; Nustatyti maksimalų failo pavadinimo ilgį

pav:

    cmp byte ptr es:[si], 13        ; Tikrinama, ar tai naujos eilutės simbolis
    je returnn                      ; Jei taip, grįžti iš procedūros
    cmp byte ptr es:[si], ' '       ; Tikrinama, ar tai tarpas
    je returnn                      ; Jei taip, grįžti iš procedūros
    mov al, es:[si]                 ; Įrašomas simbolis į registrą al
    mov ds:[di], al                 ; Įrašomas simbolis į atmintį
    inc si                          ; Padidinama komandos eilutės pozicija
    inc di                          ; Padidinama atminties pozicija

    loop pav                        ; Pasikartojama, kol visi simboliai įrašyti

    cmp byte ptr es:[si], 13        ; Tikrinama, ar tai naujos eilutės simbolis
    je returnn                      ; Jei neivestų simbolių, grįžtame į failo pavadinimą
    cmp byte ptr es:[si], ' '       ; Tikrinama, ar esantis simbolis yra tarpas
    je returnn                      ; Jei taip, grįžtame į failo pavadinimą
    mov ah, 9                       ; DOS tarnyba 9 - išvesti eilutę
    mov dx, offset too_long         ; Adresas, kuriame saugomas per ilgo failo pavadinimo pranešimas
    int 21h
    pop dx                          ; Išvaloma stekas
    int 21h                         ; Iškviečiama DOS tarnyba
    jmp exit                        ; Pereinama į programos pabaigą

returnn:                            ; Etiketė po sėkmingo nuskaitymo iš failo
    pop dx                          ; Išvaloma stekas
    mov [arPos], si                 ; Įrašomas nuskaityto simbolio skaičius
    ret                             ; Grįžtama iš procedūros

failoPav endp                       ; Baigiasi procedūra failo pavadinimui gauti

print_hex proc near                 ; Pradžia procedūros, atspausdinančios šešioliktainį skaičių

    cmp al, 0Ah                     ; Tikrinama, ar simbolis yra naujos eilutės simbolis
    jl sk                           ; Jei taip, peršokama į etiketę 'sk'
    add al, 87                      ; Konvertuojama į šešioliktai skaičių sistemą (mažosios raidės)
    jmp print                       ; Peršokama į etiketę 'print'

sk:                                 ; Etiketė, kai simbolis nėra naujos eilutės simbolis
    or  al, 00110000b               ; Konvertuojama į šešioliktai skaičių sistemą (didžiosios raidės)

print:                              ; Etiketė, kai simbolis paruoštas spausdinimui
    mov [di], al                    ; Simbolis įrašomas į atmintį
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    ret                             ; Grįžtama iš procedūros

print_hex endp                      ; Baigiasi procedūra šešioliktai skaičių spausdinimui

hex_dump proc near                  ; Pradžia procedūros, atspausdinančios baitus šešioliktainiu formatu

hexdump:                            ; Etiketė, žyminti procedūros pradžią

    mov dx, offset sourceBuffer     ; Įrašomas adresas, kuriame bus saugomas nuskaitytas tekstas
    mov cx, 16                      ; Nustatoma baitų skaičius, kurį ketinama nuskaityti iš failo
    mov bx, [sourceFhandle]         ; Įrašomas failo rankos dėžės numeris
    mov ah, 3fh                     ; DOS tarnyba 3Fh - skaityti iš failo
    int 21h

    cmp ax, 0                       ; Tikrinama, ar buvo nuskaityta bent viena baito
    je grizt                        ; Jei ne, grįžtama į etiketę 'grizt'

    mov [baitNusk], al              ; Įrašomas nuskaitytų baitų skaičius

    mov di, offset destBuffer       ; Įrašomas adresas, kuriame bus saugomas rezultatas
    mov cx, 8                       ; Nustatoma baitų skaičius viename žodyje (žodis - du baitai)
    mov al, '0'                     ; Baitas, kurį ketinama panaudoti rezultato formavimui

cclear:                             ; Etiketė, žyminti ciklą, per kurį išvalomas rezultatas
    mov [di], al                    ; Įrašomas simbolis į rezultatą
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    loop cclear                     ; Ciklas kartojamas 8 kartus

    mov di, offset destBuffer + 7   ; Nustatoma pozicija, kurioje bus laikoma simbolių suma
    mov ax, [simb]                  ; Įrašoma simbolių suma
    mov bx, 10                      ; Skaičius, iš kurio bus atliekama dalijimo operacija
    mov cx, 8                       ; Skaičius iteracijų cikle

looopas:                            ; Etiketė, žyminti ciklą, kuris paverčia skaičių šešioliktainiu formatu
    cmp ax, 0                       ; Tikrinama, ar skaičius lygus nuliui
    je hex                          ; Jei taip, peršokama į etiketę 'hex'
    xor dx, dx                      ; Valdomasis registras dx nustatomas į nulį
    div bx                          ; Dalijama iš 10
    add [di], dx                    ; Prie esamo skaičiaus pridedamas likutis (šešioliktainės skaičių sistemos skaitmuo)
    dec di                          ; Pernesiama į kitą poziciją atmintyje
    loop looopas                    ; Ciklas kartojamas 8 kartus

hex:                                ; Etiketė, žyminti, kad skaičius paverstas šešioliktainiu formatu
    mov al, ' '                     ; Nustatomas tarpas
    mov di, offset destBuffer + 9   ; Pernesiama į poziciją po šešioliktainiu formatu paversto skaičiaus
    xor cx, cx                      ; Valdomasis registras cx nustatomas į nulį
    mov cx, 47                      ; Nustatoma iteracijų skaičius cikle

lp:                                 ; Etiketė, žyminti ciklą, kuriuo į rezultatą įrašomas tarpas
    mov [di], al                    ; Įrašomas simbolis į rezultatą
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    loop lp                         ; Ciklas kartojamas 47 kartus

    mov di, offset destBuffer + 9   ; Pernesiama į poziciją po šešioliktainiu formatu paversto skaičiaus
    xor ch, ch                      ; Valdomasis registras ch nustatomas į nulį
    mov cl, [baitNusk]              ; Įrašomas nuskaitytų baitų skaičius
    mov si, offset sourceBuffer     ; Įrašomas adresas, kuriame saugomas nuskaitytas tekstas
l:

    mov al, [si]                    ; Įrašomas baitas iš nuskaityto failo į registrą al
    shr al, 4                       ; Pereinama į naują baito dalį (pirmąją keturių bitų dalį)
    call print_hex                  ; Iškviečiama procedūra, kuri atspausdina šešioliktą formatą

    mov al, [si]                    ; Įrašomas baitas iš nuskaityto failo į registrą al
    and al, 00001111b               ; Išsaugoma antroji keturių bitų dalis
    call print_hex                  ; Iškviečiama procedūra, kuri atspausdina šešioliktą formatą

    inc di                          ; Padidinama registras di, nes parašytas skaičius
    inc si                          ; Pernesiama į kitą baitą
    loop l                          ; Ciklas kartojamas 16 kartų

    mov al, ' '                     ; Nustatomas tarpas
    mov di, offset destBuffer + 58  ; Pernesiama į poziciją po šešioliktainiu formatu paversto skaičiaus
    mov cx, 16                      ; Skaičius iteracijų cikle

ll:                                 ; Etiketė, žyminti ciklą, kuriuo užpildomas likęs rezultato laukelis tarpais
    mov [di], al                    ; Įrašomas tarpas į rezultatą
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    loop ll                         ; Ciklas kartojamas 16 kartų

    mov si, offset sourceBuffer     ; Pernesiama į adresą, kuriame saugomas nuskaitytas tekstas
    mov di, offset destBuffer + 58  ; Pernesiama į poziciją po šešioliktainiu formatu paversto skaičiaus
    xor ch, ch                      ; Nustatomas valdomasis registras ch į nulį
    mov cl, [baitNusk]              ; Įrašomas nuskaitytų baitų skaičius

lop:                                ; Etiketė, žyminti ciklą, kuriuo nuskaitytas tekstas įrašomas į rezultatą
    mov al, [si]                    ; Įrašomas simbolis iš nuskaityto failo į registrą al
    mov [di], al                    ; Įrašomas simbolis į rezultatą
    inc si                          ; Pernesiama į kitą baitą
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    loop lop                        ; Ciklas kartojamas tiek kartų, kiek yra nuskaitytų simbolių

    mov dx, offset destBuffer       ; Pernesiama į adresą, kuriame saugomas rezultatas
    mov cx, 76                      ; Nustatoma baitų skaičius rezultate (76 simboliai)
    mov bx, [destFhandle]           ; Įrašomas failo rankos dėžės numeris
    mov ah, 40h                     ; 40h - rašyti į failą
    int 21h

    xor ah, ah                      ; Valdomasis registras ah nustatomas į nulį
    mov al, [baitNusk]              ; Įrašomas nuskaitytų baitų skaičius
    add [simb], ax                  ; Nuskaitytų simbolių skaičius pridedamas prie bendro simbolių skaičiaus

    cmp [sourceFhandle], 0          ; Tikrinama, ar buvo nuskaitytas bent vienas simbolis
    jne hexdump                     ; Jei taip, ciklas kartojamas su nauju nuskaitytu failu
    dec si                          ; Dekrementuojamas si registras, nes paskutinis simbolis yra naujos eilutės simbolis
    mov al, [si]                    ; Įrašomas paskutinis simbolis į registrą al
    cmp al, 10                      ; Tikrinama, ar tai naujos eilutės simbolis
    je grizt                        ; Jei taip, grįžtama iš procedūros
    jmp hexdump                     ; Jei ne, ciklas kartojamas su paskutiniu nuskaitytu failu

grizt:
    ret

hex_dump endp

kiekSimb proc near

    mov di, offset kiekis           ; Pernesiama į adresą, kuriame bus saugoma simbolių suma
    mov cx, 8                       ; Skaičius iteracijų cikle
    mov al, '0'                     ; Pirmasis simbolis simbolių sumai priskirti

isv:                                ; Etiketė, žyminti ciklą, kuriuo išvalomas rezultatas
    mov [di], al                    ; Įrašomas simbolis į rezultatą
    inc di                          ; Pernesiama į kitą poziciją atmintyje
    loop isv                        ; Ciklas kartojamas 8 kartus

    mov di, offset kiekis + 7       ; Nustatoma pozicija, kurioje bus laikoma simbolių suma
    mov ax, [simb]                  ; Įrašoma bendra simbolių suma
    mov bx, 10                      ; Skaičius, iš kurio bus atliekama dalijimo operacija
    mov cx, 8                       ; Skaičius iteracijų cikle

lll:                                ; Etiketė, žyminti ciklą, kuriuo paverčiamas skaičius šešioliktainiu formatu
    cmp ax, 0                       ; Tikrinama, ar skaičius yra nulis
    je simbKiek                     ; Jei taip, pereinama į kitą etiketę
    xor dx, dx                      ; Valdomasis registras dx nustatomas į nulį
    div bx                          ; Dalinama iš 10
    add [di], dx                    ; Prie simbolių sumos pridedama dalmuo
    dec di                          ; Pernesiama į kitą poziciją atmintyje
    loop lll                        ; Ciklas kartojamas tiek kartų, kiek yra simbolių

simbKiek:                           ; Etiketė, žyminti simbolių kiekio nustatymą
    mov dx, offset kiekis           ; Pernesiama į adresą, kuriame laikoma simbolių suma
    mov cx, 9                       ; Skaičius iteracijų cikle
    mov bx, [destFhandle]           ; Įrašomas failo rankos dėžės numeris
    mov ah, 40h                     ; 40h - rašyti į failą
    int 21h
    ret

kiekSimb endp

end start