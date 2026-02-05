; 15. Parašykite programą, kuri įvestoje simbolių eilutėje pakeičia ASCII raidžių skyrių
; (mažąsias didžiosiomis ir atvirkščiai). Pvz.: įvedus aBcDEf54 turi atspausdinti AbCdeF54

.model small
.stack 100h
MAX_BUFFER = 200

.data
    input db ?
    req_msg db 0Dh, 0Ah, "Iveskit eilute: $"
    res_msg db 0Dh, 0Ah, "Pakeista eilute: $"
    buffer db MAX_BUFFER
    buffer_data db MAX_BUFFER dup (0)

.code
start:
    mov ax, @data
    mov ds, ax

    ; Isveda requesta
    mov ah, 09h
    mov dx, offset req_msg
    int 21h

ivestis:
    ; skaito eilute
    mov dx, offset buffer
    mov ah, 0Ah
    int 21h

    ; tikrina ar ivesta eilute tuscia, jei taip soka i pabaiga
    cmp byte ptr buffer[1], 0
    je pabaiga

    ; isveda ats
    mov ah, 09h
    mov dx, offset res_msg
    int 21h

    ; isvedineja pakeista eilute
    mov si, offset buffer_data
    xor ch, ch
    mov cl, buffer[1]

tikrina:
    lodsb
    mov ah, 02h
    ; jei mazoji tai pakeicia i didziaja, jei ne tai soka i kita funkcija

    cmp al, 'a'
    jb didzioji
    cmp al, 'z'
    ja didzioji
    xor al, 32
    jmp isveda

didzioji:
    ; jei didzioji tai pakeicia i mazaja
    cmp al, 'A'
    jb isveda
    cmp al, 'Z'
    ja isveda
    xor al, 32

isveda:
    mov dl, al
    mov ah, 02h
    int 21h

    loop tikrina

    jmp pabaiga

; baigia progama
pabaiga:
    mov ax, 4c00h
    int 21h

end start
