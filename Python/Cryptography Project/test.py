import functions as func
import random


def RunTest(n=15, k=5, p=0.05, trials=1000):
    G = func.RandonGeneruojantiM(n, k)
    H = func.Hmatrica(G)
    coset_leaders = func.CosetLeader(H)

    bit_errors_uncoded = 0
    bit_errors_coded = 0
    total_bits = trials * k

    for _ in range(trials):
        msg = [random.randint(0, 1) for _ in range(k)]

        # --- be kodo ---
        recv_uncoded = func.Kanalas(msg, p)[0]
        for a, b in zip(msg, recv_uncoded):
            if a != b:
                bit_errors_uncoded += 1

        # --- su kodu ---
        encoded = func.MkartM(msg, G)
        recv = func.Kanalas(encoded, p)
        decoded = func.StepByStep(coset_leaders, H, recv)
        decoded_msg = decoded[0][:k]

        for a, b in zip(msg, decoded_msg):
            if a != b:
                bit_errors_coded += 1

    return {
        "p": p,
        "total_bits": total_bits,
        "errors_uncoded": bit_errors_uncoded,
        "errors_coded": bit_errors_coded,
        "ber_uncoded": bit_errors_uncoded / total_bits,
        "ber_coded": bit_errors_coded / total_bits
    }



print("\nBIT ERROR STATISTICS\n")
print(" p   | be kodo (%)    | su kodu (%) ")
print("------------------------------------")

for p in [0.01, 0.05, 0.1, 0.2, 0.25]:
    s = RunTest(p=p, trials=1000)
    print(f"{p:0.2f} |"
          f" {s['ber_uncoded']*100:7.2f}        |"
          f" {s['ber_coded']*100:7.2f}   "
          )

