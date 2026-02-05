import random

def I_M(n):
    M = []
    for i in range(n):
        row = []
        for j in range(n):
            if i == j:
                row.append(1)
            else:
                row.append(0)
        M.append(row)

    return M

def R_M(n, k):
    M = []
    for i in range(n):
        row = []
        for j in range(k):
            sk = random.randint(0, 1)
            row.append(sk)
        M.append(row)

    return M

def MkartM(m, G):
    m = toMatrix(m)
    G = toMatrix(G)

    result = []

    for i in range(getEil(m)):
        row = []

        for j in range(getStulp(G)):
            s = 0

            for k in range(getEil(G)):
                s += m[i][k] * G[k][j]

            row.append(s % 2)

        result.append(row)
    return result

def trans(H):
    H = toMatrix(H)
    eil = getEil(H)
    stulp = getStulp(H)
    T = []
    for j in range(stulp):
        row = []

        for i in range(eil):
            row.append(H[i][j])

        T.append(row)

    return T

def toMatrix(A):
    if isinstance(A[0], list):
        return A
    else:
        return [A]

def getEil(A):
    A = toMatrix(A)
    return len(A)

def getStulp(A):
    A = toMatrix(A)

    return len(A[0])

def intToBinaryVector(i, length):
    binary_string = format(i, f"0{length}b")
    binary_vector = []

    for i in binary_string:
        bit = int(i)
        binary_vector.append(bit)

    return binary_vector

def Counteris1(A):
    A = toMatrix(A)
    count = 0
    for row in A:
        for val in row:
            if val == 1:
                count += 1
    return count

def Syndrome(H, r):
    r = toMatrix(r)
    r_T = trans(r)
    product = MkartM(H, r_T)
    product_T = trans(product)
    syndrome = tuple(product_T[0])

    return syndrome

def CosetLeader(H):
    coset_leaders = {}

    for i in range(2 ** getStulp(H)):
        cosetuKlaidos = intToBinaryVector(i, getStulp(H))
        weight = Counteris1(cosetuKlaidos)
        syndrome = Syndrome(H, cosetuKlaidos)
        if syndrome not in coset_leaders:
            coset_leaders[syndrome] = weight
        else:
            if weight < coset_leaders[syndrome]:
                coset_leaders[syndrome] = weight

    return coset_leaders

def Kanalas(c, p):

    c = toMatrix(c)
    r = [c[0].copy()]

    for i in range(len(r[0])):
        if random.random() < p:
            r[0][i] ^= 1

    return r


def RandonGeneruojantiM(n, k):
    I = I_M(k)
    A = R_M(k, n - k)
    G = []
    rows = getEil(I)

    for i in range(rows):
        row_I = I[i]
        row_A = A[i]
        new_row = row_I + row_A
        G.append(new_row)

    return G


def Hmatrica(G):
    k = getEil(G)
    n = getStulp(G)

    A = [row[k:] for row in G]  # paimam A iš G = [I | A]
    At = trans(A)     # A^T
    I = I_M(n - k)

    return [At[i] + I[i] for i in range(n - k)]

def StepByStep(coset_leaders, H, r):
    for i in range(getStulp(r)):
        syndrome = Syndrome(H, r)
        if coset_leaders[syndrome] == 0:
            return r
        else:
            e = Keitimas(getStulp(r), i)
            new_r = addMod2(r, e)
            new_syndrome = Syndrome(H, new_r)
            if coset_leaders[new_syndrome] < coset_leaders[syndrome]:
                r = new_r

    return r


def addMod2(A, B):
    A = toMatrix(A)
    B = toMatrix(B)
    return [[(A[i][j] + B[i][j]) % 2 for j in range(getStulp(A))] for i in range(getEil(A))]

def splitWithPadding(A, size):
    result = []
    for i in range(0, len(A), size):
        chunk = A[i:i+size]
        if len(chunk) < size:
            chunk += [0] * (size - len(chunk))
        result.append(chunk)
    return result


    
def Keitimas(n, i):
    return [1 if j == i else 0 for j in range(n)]


def EncodeDecode(m, G, p):
    messages = splitWithPadding(m, getEil(G))

    H = Hmatrica(G)
    coset_leaders = CosetLeader(H)

    msg_prokanala = []
    msg_decoded = []

    for message in messages:
        encoded = MkartM(message, G)
        prkanala = Kanalas(encoded, p)
        msg_prokanala.extend(prkanala[0][:getEil(G)])
        decoded = StepByStep(coset_leaders, H, prkanala)
        msg_decoded.extend(decoded[0][:getEil(G)])

    return msg_prokanala[:len(m)], msg_decoded[:len(m)]


def stringToBinList(string):
    bits = []

    for char in string:
        ascii_code = ord(char)
        binary_string = format(ascii_code, '08b')

        for bit in binary_string:
            bits.append(int(bit))
    return bits


def binListToString(binary_list):
    chars = []
    for i in range(0, len(binary_list), 8):
        byte = binary_list[i:i+8]
        if len(byte) == 8:
            ascii_val = sum(bit * (2 ** (7-j)) for j, bit in enumerate(byte))
            chars.append(chr(ascii_val))
    return ''.join(chars)